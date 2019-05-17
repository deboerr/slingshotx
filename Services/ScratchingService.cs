using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Dapper;
using slingshotx.DTO;
using System;

namespace slingshotx.Services
{
    public class ScratchingService
    {
        private string _connStr;
		private RunnerService runnerService;

		const int EARLY_SCRATCHING = 2;
		const int LATE_SCRATCHING = 3;
		const string BRAVO_LATE_SCRATCHING = "LATESCRATCHED";

        const string NORMAL_STATUS = "NORMAL";
		const string SYNTHETIC = "SYNTHETIC";
		const string SAND = "SAND";
		const string AWT = "AWT";

        public ScratchingService(string connectionString)
        {
            _connStr = connectionString;
			this.runnerService = new RunnerService(_connStr);
        }

         // fetch scratchings by date
        public object getByDate(DateTime meetingDate)
        {
            var sql = @"
                SELECT
	                cm.id as MeetingId, cmv.Location, cmv.MeetingName as MeetingName, cmv.BravoVenueName as VenueName,
                        st.typecodemnc1 as ScheduledType, mtt.typecodemnc1 as MeetingType,
                        vc.VenueCodeMnc2 + st.typecodemnc1 as VenueCode, cmv.BravoVenueMnc3 as BravoMnemonic,
                        CASE
							when upper(cm.TrackRatingName) = '" + SYNTHETIC + "' then '" + AWT + @"'
							when upper(cm.TrackRatingName) = '" + SAND + "' then '" + AWT + @"'
							when upper(tst.Description) = '" + SYNTHETIC + "' then '" + AWT + @"'
							when upper(tst.Description) = '" + SAND + "' then '" + AWT + @"'
							else cm.TrackRatingName
						END as TrackRatingName,
						cm.TrackRatingNumeric, cm.Weather, convert(varchar(10), cm.MeetingDate, 120) as MeetingDate,
                        (select top 1 r1.StartTime_UTC from race r1 where cm.id=r1.calendarmeetingid and r1.FixedOddsOnly = 0 and r1.ExtractDelete = 0 order by r1.StartTime_UTC) as MeetingStartTimeUTC,
                        (select count(*) as cnt from race r2 where cm.id=r2.calendarmeetingid and r2.FixedOddsOnly = 0 and r2.ExtractDelete = 0) as NumberOfRaces,
	                r.id as RaceId, r.RaceNumber, r.Fullname as RaceName, CAST(ISNULL(r.BravoStartTimeUTC, r.StartTime_UTC) AS datetimeoffset) as StartTimeUTC,
                    r.NumberOfStarters, r.NumberOfAcceptors, 0 as NumberOfScratchings,
                    ru.Id as RunnerId, ru.Number as RunnerNumber, ru.Name as RunnerName, ru.Barrier, ru.RiderDriver,
                    ru.RunnerCode, ru.ProviderScratchStatus, ru.ProviderScratchReason, ru.ProviderScratchUpdateTime,
                    ru.BravoScratchingStatus, ru.RiderUpdateRequired as RiderNameChange,
                    cm.NotificationValueAdded
                FROM calendarmeeting cm
                    inner join calendarmeetingvenue cmv on (cm.calendarMeetingVenueId=cmv.id)
	                inner join meetingtypetype mtt on (mtt.id = cm.meetingTypeCodeId)
	                inner join meetingtypetype st on (st.id = cm.scheduledTypeCodeId)
	                inner join venuecode vc on vc.Id = cm.VenueCodeId
                    inner join race r on (cm.id=r.calendarMeetingId)
                    inner join runner ru on (r.id=ru.raceId)
					inner join TrackSurfaceType tst on (cm.TrackSurafceTypeId=tst.Id)
                WHERE (convert(date, cm.MeetingDate) >= @meetingdate
                    and cm.StatusId = 20
                    and r.FixedOddsOnly = 0
                    and r.ExtractDelete = 0)
                AND (
                      (ru.BravoScratchingStatus IS NOT NULL and ru.BravoScratchingStatus <> '" + NORMAL_STATUS + @"')
                       or ru.ProviderScratchUpdateTime IS NOT NULL
                       or ru.RiderUpdateRequired = 1
                    )
            ";

            var scratchings = new List<ScratchingDTO>();
            using (IDbConnection db = new SqlConnection(_connStr))
            {
                scratchings = db.Query<ScratchingDTO>(sql,
                    new { meetingdate = new[] { meetingDate } }
                ).ToList();
            }

			if (scratchings == null) {
				return null;
			}

            var runners = scratchings
                   .Select(c => new { c.RunnerId, c.RaceId, c.MeetingId, c.RunnerNumber, c.RunnerName, c.Barrier, c.RunnerCode,
                       c.ProviderScratchStatus, c.ProviderScratchReason, c.ProviderScratchUpdateTime, c.BravoScratchingStatus,
                       c.RiderDriver, c.RiderNameChange })
                   .ToList();

            var races = scratchings
                   .Select(r => new
                   {
                       r.RaceId,
                       r.MeetingId,
                       r.RaceNumber,
                       r.RaceName,
                       r.StartTimeUTC,
                       EarlyPendingCnt = runners.Count(c => c.RaceId == r.RaceId && ((c.BravoScratchingStatus == null || c.BravoScratchingStatus == NORMAL_STATUS) && (c.ProviderScratchStatus == EARLY_SCRATCHING))),
					   LatePendingCnt = runners.Count(c => c.RaceId == r.RaceId && ((c.BravoScratchingStatus == null || c.BravoScratchingStatus == NORMAL_STATUS) && (c.ProviderScratchStatus == LATE_SCRATCHING))),
				   })
                   .Distinct()
                   .ToList();

            var meetings = scratchings
                   .Select(m => new
                   {
                       m.MeetingId,
                       m.Location,
                       m.NumberOfRaces,
                       m.MeetingName,
                       m.VenueName,
                       m.MeetingType,
                       m.ScheduledType,
                       m.VenueCode,
                       m.MeetingStartTimeUTC,
					   EarlyPendingCnt = runners.Count(c => c.MeetingId == m.MeetingId && ((c.BravoScratchingStatus == null || c.BravoScratchingStatus == NORMAL_STATUS) && (c.ProviderScratchStatus == EARLY_SCRATCHING))),
					   LatePendingCnt = runners.Count(c => c.MeetingId == m.MeetingId && ((c.BravoScratchingStatus == null || c.BravoScratchingStatus == NORMAL_STATUS) && (c.ProviderScratchStatus == LATE_SCRATCHING))),
					   BravoLateScratchedCnt = runners.Count(c => c.MeetingId == m.MeetingId && ((c.BravoScratchingStatus == BRAVO_LATE_SCRATCHING))),
					   m.BravoMnemonic,
                       m.TrackRatingName,
                       m.TrackRatingNumeric,
                       m.Weather,
                       m.MeetingDate,
                       RiderNameChangeCnt = runners.Count(c => c.MeetingId == m.MeetingId && ((c.RiderNameChange))),
                       m.NotificationValueAdded
                   })
                   .Distinct()
                   .ToList();

            return new
            {
                TimeStamp = DateTime.Now,
                MeetingDate = meetingDate.ToString("yyyy-MM-dd"),
                Meetings = meetings,
                Races = races,
                Runners = runners
            };


        }

        private long GetSlaSortKey(DateTimeOffset dtoIn, int slaTarget)
        {
            return dtoIn.ToUnixTimeSeconds() + slaTarget;
        }

    }


}
