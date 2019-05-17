using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Dapper;
using slingshotx.DTO;
using System;

namespace slingshotx.Services
{
    public class PodService
    {
        private string _connStr;

        const string RACE_ABANDONED = "ABANDONED";
        const string BRAVO_STATUS_OPEN = "OPEN";
        const string BRAVO_STATUS_CLOSED = "CLOSED";
        const string BRAVO_STATUS_INTERIM = "INTERIM";
        const string BRAVO_STATUS_PROTEST = "PROTEST";
        const string BRAVO_STATUS_FINAL = "FINAL";
        const string CONTROLLER_ROLE = "CONTROLLER";

        public PodService(string connectionString)
        {
            _connStr = connectionString;
        }

        // fetch podAllocations by meetingid
        public IEnumerable<PodRaceAllocationDTO> GetAllocationsForUser(DateTime meetingDate, string userId)
        {
            var endOfDay = meetingDate.ToString("yyyy-MM-dd 19:00:00");
            var sql = @"
                SELECT DISTINCT
				    r.Id as RaceId,
				    r.CalendarMeetingId as MeetingId,
				    r.RaceNumber as EventNumber,
				    r.TABName as EventName,
				    CAST(ISNULL(r.BravoStartTimeUTC, r.StartTime_UTC) AS datetimeoffset) as StartTimeUTC,
				    r.NumberOfStarters as NumberOfStarters,
				    r.NumberOfAcceptors as NumberOfAcceptors,
				    r.Stage as Stage,
				    cmv.MeetingName as MeetingName,
				    cmv.Location as VenueLocation,
				    cmv.BravoVenueName,
				    mtt.TypeCodeMnc1 as MeetingType,
                    vc.VenueCodeMnc2 + st.typecodemnc1 as SellCode,
                    ISNULL(prr.PodNumber, p.DisplayOrder) as PodNumber,
                    ISNULL(prr.SeatNumber, pa.SeatNumber) as SeatNumber,
		            r.BravoStatus,
		            r.BravoResults,
                    ISNULL(convert(varchar(2), cm.Landline), '--') as Landline,
                    r.Distance,
                    cm.SurfaceCondition as Track,
                    cm.WeatherCondition as Weather,
                    pu.Role as Role,
                    u.Id as UserId,
                    ISNULL(u.initials, ISNULL(u1.initials, '?')) as Initials,
                    null as FirstRaceStartTimeUTC
			    FROM Race r
			    INNER JOIN CalendarMeeting cm ON (r.CalendarMeetingId= cm.Id)
			    INNER JOIN MeetingTypeType mtt ON (mtt.Id = cm.MeetingTypeCodeId)
	            INNER JOIN meetingtypetype st ON (st.id = cm.scheduledTypeCodeId)
			    INNER JOIN CalendarMeetingVenue cmv ON (cmv.id = cm.CalendarMeetingVenueId)
	            INNER JOIN venuecode vc ON vc.Id = cm.VenueCodeId
	            INNER JOIN podallocation pa ON cm.Id = pa.CalendarMeetingId
	            INNER JOIN pod p ON pa.PodId = p.Id
	            LEFT JOIN PodRaceReallocation prr ON r.Id = prr.RaceId
                LEFT JOIN poduser pu ON (pu.meetingdate=cm.meetingdate
                        and pu.UserId = @userid
                        and pu.PodNumber = ISNULL(prr.PodNumber, p.DisplayOrder)
                        and pu.SeatNumber = ISNULL(prr.SeatNumber, pa.SeatNumber)
                        and CAST(ISNULL(r.BravoStartTimeUTC, r.StartTime_UTC) AS datetimeoffset) between pu.start and pu.finish)
                LEFT JOIN users u ON (pu.userid=u.id)
            ";
            // fetch initials of race controller for race has been assigned to other user
            sql += @"
                LEFT JOIN poduser pu1 ON (
                    pu1.meetingdate=cm.meetingdate AND pu1.podnumber=ISNULL(prr.PodNumber,p.displayorder) AND pu1.seatnumber=ISNULL(prr.SeatNumber,pa.SeatNumber) AND pu1.role='" + CONTROLLER_ROLE + @"'
                    AND CAST(ISNULL(r.BravoStartTimeUTC, r.StartTime_UTC) AS datetimeoffset) between pu1.start AND pu1.finish
                )
                LEFT JOIN users u1 ON (pu1.userid=u1.id)
            ";
            sql += @"
                WHERE convert(date, cm.MeetingDate) = @meetingdate
                        and cm.StatusId >= 70
                        and r.FixedOddsOnly = 0
                        and r.ExtractDelete = 0
                        and (r.BravoStatus is Null or r.BravoStatus NOT IN ('" + BRAVO_STATUS_FINAL + @"','" + RACE_ABANDONED + @"'))
                        and CAST(ISNULL(r.BravoStartTimeUTC, r.StartTime_UTC) AS datetimeoffset) < @endofday
                ORDER BY CAST(ISNULL(r.BravoStartTimeUTC, r.StartTime_UTC) AS datetimeoffset);
            ";

            var data = new List<PodRaceAllocationDTO>();
            using (IDbConnection db = new SqlConnection(_connStr))
            {
                data = db.Query<PodRaceAllocationDTO>(sql,
                    new {
						meetingdate = new[] { meetingDate.ToString("yyyy-MM-dd") },
						userid = new[] { userId },
						endofday = new[] { endOfDay }
					 }
                ).ToList();
            }

            return data;
        }

    }
}
