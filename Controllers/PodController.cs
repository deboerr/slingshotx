using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using slingshotx.DTO;
using slingshotx.Services;

namespace slingshotx.Controllers
{
    [Route("slingshot/v1/pod")]
    public class PodController : Controller
    {
        PodService _podService;

        public PodController(PodService podService)
        {
             _podService = podService;
        }

        // GET slingshot/v1/pod/bydate/yyyy-mm-dd
        [HttpGet("getallocationsforuser/{meetingDate}/{userId}")]
        public JsonResult GetAllocationsForUser(DateTime meetingDate, string userId)
        {
            var data = _podService.GetAllocationsForUser(meetingDate, userId);
            return Json(data);
        }

    }
}
