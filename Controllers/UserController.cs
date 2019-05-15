using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Dapper;
using slingshotx.DTO;
using slingshotx.Services;

namespace slingshotx.Controllers
{
    [Route("slingshot/v1/user")]
    public class UserController : Controller
    {
        UserService _userService;

        public UserController(UserService userService)
        {
             _userService = userService;
        }

        // GET slingshot/v1/user/byname/galeg
        [HttpGet("byname/{userName}")]
        public JsonResult GetByName(string userName)
        {
            var users = _userService.getByName(userName);
            return Json(users);
        }

        // GET slingshot/v1/user/getall
        [HttpGet("getall")]
        public JsonResult GetAll()
        {
            var users = _userService.getAll();
            return Json(users);
        }

    }
}
