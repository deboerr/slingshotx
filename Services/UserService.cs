using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Dapper;
using slingshotx.DTO;
using System;

namespace slingshotx.Services
{
    public class UserService
    {
        private string _connStr;
		private string[] ROLES = new string[] { "POD MANAGER", "CONTROLLER", "SHIFT LEADER" };

        public UserService(string connectionString)
        {
            _connStr = connectionString;
        }

        // fetch users by name
        public List<UserDTO> getByName(string userName)
        {
            var sql = @"SELECT
                    u.*,
		            r.Id as RoleId,
		            r.RoleName, c.Id as ContactId
	            FROM Users u
	            INNER JOIN UsersInRoles uir ON (u.Id = uir.UserId)
	            INNER JOIN Roles r ON (uir.RoleId = r.Id)
                INNER JOIN Contact c ON (c.UserId = u.Id)
	            WHERE u.UserName = @username";

            var data = new List<UserDTO>();
            using (IDbConnection db = new SqlConnection(_connStr))
            {
                data = db.Query<UserDTO>(sql,
                    new { username = new[] { userName } }
                ).ToList();
            }

            return data;
        }

        // fetch all users
        public List<UserDTO> getAll()
        {
            var sql = @"SELECT
                    u.*, r.rolename, c.Id as ContactId
	            FROM Users u
	            INNER JOIN UsersInRoles uir ON (u.Id = uir.UserId)
	            INNER JOIN Roles r ON (uir.RoleId = r.Id)
				INNER JOIN Contact c ON (c.UserId = u.Id)
                WHERE u.active = 1
                AND r.rolename in ('" + string.Join("','", ROLES) + @"')
                ORDER BY LastName, FirstName";

            var data = new List<UserDTO>();
            using (IDbConnection db = new SqlConnection(_connStr))
            {
                data = db.Query<UserDTO>(sql,
                    new { }
                ).ToList();
            }

            return data;
        }
    }
}
