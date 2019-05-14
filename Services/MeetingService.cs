using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Dapper;
using slingshotx.DTO;
using System;

namespace slingshotx.Services
{
    public class MeetingService
    {
        private string _connStr;

        public MeetingService(string connectionString)
        {
            _connStr = connectionString;
        }

        // fetch meetings by date
        public List<MeetingDTO> getByDate(DateTime meetingDate)
        {
            var sql = @"SELECT
				    cm.id as MeetingId, cmv.Location, cmv.MeetingName as MeetingName, cmv.BravoVenueName as VenueName,
					st.typecodemnc1 as ScheduledType, mtt.typecodemnc1 as MeetingType,
                    vc.VenueCodeMnc2 + st.typecodemnc1 as VenueCode, cmv.BravoVenueMnc3 as BravoMnemonic,
                    (select top 1 r1.StartTime_UTC from race r1 where cm.id=r1.calendarmeetingid and r1.FixedOddsOnly = 0 and r1.ExtractDelete = 0 order by r1.StartTime_UTC) as MeetingStartTimeUTC, 
                    (select count(*) as cnt from race r2 where cm.id=r2.calendarmeetingid and r2.FixedOddsOnly = 0 and r2.ExtractDelete = 0) as NumberOfRaces
				FROM CalendarMeeting cm
				INNER JOIN VenueCode vc ON vc.Id = cm.VenueCodeId
                LEFT OUTER JOIN meetingtypetype mtt on (mtt.id = cm.meetingTypeCodeId)
				INNER JOIN meetingtypetype st ON st.Id = cm.ScheduledTypeCodeId
				INNER JOIN CalendarMeetingVenue cmv ON cmv.id = cm.CalendarMeetingVenueId
				WHERE Convert(date, cm.meetingdate) = @meetingdate
				AND cm.NumberOfRaces is Not Null
                AND cm.StatusId >= 90
				ORDER BY VenueName";

            var data = new List<MeetingDTO>();
            using (IDbConnection db = new SqlConnection(_connStr))
            {
                data = db.Query<MeetingDTO>(sql, 
                    new { meetingdate = new[] { meetingDate } }
                ).ToList();
            }

            return data;
        }
    }
}
