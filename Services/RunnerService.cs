using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Dapper;
using slingshotx.DTO;
using System;

namespace slingshotx.Services
{
    public class RunnerService
    {
        private string _connStr;

        public RunnerService(string connectionString)
        {
            _connStr = connectionString;
        }

        // fetch runners by raceid
        public List<RunnerDTO> getByRaceId(string raceId)
        {
            var sql = @"SELECT
                ru.RaceId as RaceId,
		        ru.Id as UniqueContestantId,
		        ru.Number as ContestantNumber,
		        ru.Name as ContestantName,
		        ru.Barrier as Barrier,
		        ru.[Emergency] as [Emergency],
		        ru.RiderDriver as Rider,
		        ru.Trainer as Trainer,
		        ru.Blinkers as Blinkers,
		        CAST(CASE
                    WHEN rf.ColourId is not null
                       THEN 1
                       ELSE 0
                    END as bit) as HasColours,
		        ru.RunnerCode,
		        ru.ProviderScratchReason,
		        ru.ProviderScratchUpdateTime,
		        ru.BravoScratchingStatus
                FROM Runner ru
                INNER JOIN Race ra ON ra.id = ru.RaceId
                LEFT JOIN RunnerForm rf ON rf.RunnerId = ru.Id
                WHERE ra.Id = @raceId
                AND ru.ExtractDelete = 0
                ORDER BY ContestantNumber
            ";

            var data = new List<RunnerDTO>();
            using (IDbConnection db = new SqlConnection(_connStr))
            {
                data = db.Query<RunnerDTO>(sql,
                    new { raceid = new[] { raceId } }
                ).ToList();
            }

            return data;
        }

        public IEnumerable<RunnerDTO> getRunnersForEvents(string[] eventIds)
        {
            var racelist = "'" + string.Join("','", eventIds) + "'";
            var sql = @"SELECT
                ru.RaceId as RaceId,
		        ru.Id as UniqueContestantId,
		        ru.Number as ContestantNumber,
		        ru.Name as ContestantName,
		        ru.Barrier as Barrier,
		        ru.[Emergency] as [Emergency],
		        ru.RiderDriver as Rider,
		        ru.Trainer as Trainer,
		        ru.Blinkers as Blinkers,
		        CAST(CASE
                    WHEN rf.ColourId is not null
                       THEN 1
                       ELSE 0
                    END as bit) as HasColours,
		        ru.RunnerCode,
		        ru.ProviderScratchReason,
		        ru.ProviderScratchUpdateTime,
		        ru.BravoScratchingStatus
                FROM Runner ru
                INNER JOIN Race ra ON ra.id = ru.RaceId
                LEFT JOIN RunnerForm rf ON rf.RunnerId = ru.Id
                WHERE ra.Id in (" + racelist + @")
                AND ru.ExtractDelete = 0
                ORDER BY ContestantNumber
            ";
            var data = new List<RunnerDTO>();
            using (IDbConnection db = new SqlConnection(_connStr))
            {
                data = db.Query<RunnerDTO>(sql,
                    new { }
                ).ToList();
            }

            return data;

        }
    }
}
