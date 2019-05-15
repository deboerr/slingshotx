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

        public RaceService(string connectionString)
        {
            _connStr = connectionString;
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
    }
}
