using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Dapper;
using slingshotx.DTO;
using System;

namespace slingshotx.Services
{
    public class RaceService
    {
        private string _connStr;
		private RunnerService runnerService;

        const string RACE_ABANDONED = "ABANDONED";
        const string BRAVO_STATUS_OPEN = "OPEN";
        const string BRAVO_STATUS_CLOSED = "CLOSED";
        const string BRAVO_STATUS_INTERIM = "INTERIM";
        const string BRAVO_STATUS_PROTEST = "PROTEST";
        const string BRAVO_STATUS_FINAL = "FINAL";
        const string CONTROLLER_ROLE = "CONTROLLER";

        public RaceService(string connectionString)
        {
            _connStr = connectionString;
			this.runnerService = new RunnerService(_connStr);
        }

        // fetch races by meetingid
        public List<RaceDTO> getByMeetingId(string meetingId)
        {
            var sql = @"
	            SELECT
				    r.Id as RaceId,
				    r.CalendarMeetingId as MeetingId,
				    r.RaceNumber as EventNumber,
				    r.TABName as EventName,
				    CAST(ISNULL(r.BravoStartTimeUTC, r.StartTime_UTC) AS datetimeoffset) as StartTimeUTC,
				    r.NumberOfStarters as NumberOfStarters,
				    r.NumberOfAcceptors as NumberOfAcceptors,
				    r.Stage as Stage,
				    cm.RUPMeeting as MeetingName,
				    cmv.Location as VenueLocation,
				    mtt.TypeCodeMnc1 as MeetingType,
                    vc.VenueCodeMnc2 + st.typecodemnc1 as SellCode,
                    ISNULL(prr.PodNumber, p.DisplayOrder) as PodNumber,
                    ISNULL(prr.SeatNumber,pa.SeatNumber) as SeatNumber,
		            r.BravoStatus,
		            r.BravoResults
	            FROM Race r
			    INNER JOIN CalendarMeeting cm ON (r.CalendarMeetingId= cm.Id)
			    INNER JOIN MeetingTypeType mtt ON (mtt.Id = cm.MeetingTypeCodeId)
	            INNER JOIN meetingtypetype st ON (st.id = cm.scheduledTypeCodeId)
			    INNER JOIN CalendarMeetingVenue cmv ON (cmv.id = cm.CalendarMeetingVenueId)
	            INNER JOIN venuecode vc on vc.Id = cm.VenueCodeId
	            LEFT JOIN podallocation pa on cm.Id = pa.CalendarMeetingId
	            LEFT JOIN pod p on pa.PodId = p.Id
	            LEFT JOIN PodRaceReallocation prr ON r.Id = prr.RaceId
	            WHERE r.CalendarMeetingId = @meetingid
	            AND r.extractDelete = 0
	            ORDER BY CAST(ISNULL(r.BravoStartTimeUTC, r.StartTime_UTC) AS datetimeoffset)
            ";

            var data = new List<RaceDTO>();
            using (IDbConnection db = new SqlConnection(_connStr))
            {
                data = db.Query<RaceDTO>(sql,
                    new { meetingid = new[] { meetingId } }
                ).ToList();
            }

            return data;
        }


        // fetch races ready to pay
        public List<ReadyToPayDTO> getReadyToPay(DateTime meetingDate, int count)
        {
            var endOfDay = meetingDate.ToString("yyyy-MM-dd 19:00:00");
            var sql = @"
                SELECT TOP " + count + @"
				    r.Id as RaceId,
				    r.CalendarMeetingId as MeetingId,
				    r.RaceNumber as EventNumber,
				    r.TABName as EventName,
                    CAST(IsNull(r.BravoResultsTimeStamp, GETUTCDATE()) AS datetimeoffset) as StartTimeUTC,
				    r.NumberOfStarters as NumberOfStarters,
				    r.NumberOfAcceptors as NumberOfAcceptors,
				    r.Stage as Stage,
				    cmv.MeetingName as MeetingName,
				    lo.LocationCode as VenueLocation,
				    cmv.BravoVenueName,
				    mtt.TypeCodeMnc1 as MeetingType,
                    vc.VenueCodeMnc2 + st.typecodemnc1 as SellCode,
		            r.BravoStatus,
		            r.BravoResults,
					CAST(CASE
						WHEN (rp1.Protest is not null) THEN 1
						ELSE 0
					END as bit) as HasProtest,
					rp1.ProtestShort as Protest,
					rp2.ProtestShort as Outcome,
                    ISNULL(
                        (SELECT s.WarningSeconds AS Expr1
                            FROM SLAWarning AS s
						    INNER JOIN SLAWarningLocation sl ON (sl.SLAWarningId = s.Id)
                            WHERE (s.MeetingTypeId = cm.ScheduledTypeCodeId and lo.Id = sl.LocationId)), -9999) AS SLATarget
			    FROM Race r
			    INNER JOIN CalendarMeeting cm ON (r.CalendarMeetingId= cm.Id)
			    INNER JOIN MeetingTypeType mtt ON (mtt.Id = cm.MeetingTypeCodeId)
	            INNER JOIN meetingtypetype st ON (st.id = cm.scheduledTypeCodeId)
			    INNER JOIN CalendarMeetingVenue cmv ON (cmv.id = cm.CalendarMeetingVenueId)
	            INNER JOIN venuecode vc on vc.Id = cm.VenueCodeId
				INNER JOIN Location lo on (lo.Id = cmv.LocationId)
				LEFT OUTER JOIN RaceProtest rp1 on (rp1.RaceId = r.id  and rp1.ProtestType = 1)
				LEFT OUTER JOIN RaceProtest rp2 on (rp2.RaceId = r.id  and rp2.ProtestType = 2)

                WHERE cm.MeetingDate = @meetingdate
                    and cm.StatusId >= 70
                    AND (r.BravoStatus IN ('" + BRAVO_STATUS_INTERIM + @"','" + BRAVO_STATUS_PROTEST + @"')
                        OR r.BravoStatus = '" + BRAVO_STATUS_CLOSED + @"' AND r.BravoResults is not null)
                    and r.FixedOddsOnly = 0
                    and r.ExtractDelete = 0
                    and r.StartTime_UTC < '" + endOfDay + @"'
            ";

            var races = new List<RaceDTO>();
            using (IDbConnection db = new SqlConnection(_connStr))
            {
                races = db.Query<RaceDTO>(sql,
                    new { meetingdate = new[] { meetingDate } }
                ).ToList();
            }

			// fetch scratched runners
			string[] ids = races.Select(r => r.RaceId.ToString()).ToArray();
			var runners = runnerService.getRunnersForEvents(ids);

			// build response payload
			return races.Select(r => new ReadyToPayDTO
			{
				RaceId = r.RaceId,
				MeetingId = r.MeetingId,
				MeetingName = r.MeetingName,
				RaceNumber = r.EventNumber,
				SellCode = r.SellCode,
				BravoResults = r.BravoResults,
				BravoResultsTimeStamp = r.StartTimeUTC,
				Scratchings = string.Join(",", runners.Where(ru => ru.RaceId == r.RaceId && ru.BravoScratchingStatus != null && ru.BravoScratchingStatus != "NORMAL")
					.Select(ru => ru.ContestantNumber.ToString())
					.ToArray()),
				HasProtest = r.HasProtest,
				Protest = r.Protest,
				OutCome = r.OutCome,
				SLATarget = r.SLATarget,
				SlaSortKey = this.GetSlaSortKey((DateTimeOffset)r.StartTimeUTC, (int)r.SLATarget)
			})
			.OrderBy(x => x.SlaSortKey)
			.ThenBy(x => x.BravoResultsTimeStamp)
			.ThenBy(x => x.RaceId)
			.ToList();

        }

        private long GetSlaSortKey(DateTimeOffset dtoIn, int slaTarget)
        {
            return dtoIn.ToUnixTimeSeconds() + slaTarget;
        }

    }
}
