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
    [Route("slingshot/v1/meeting")]
    public class MeetingController : Controller
    {
        MeetingService _meetingService;

        public MeetingController(MeetingService meetingService)
        {
             _meetingService = meetingService;
        }

        // GET slingshot/v1/meeting/bydate/yyyy-mm-dd
        [HttpGet("bydate/{meetingDate}")]
        public JsonResult GetByDate(DateTime meetingDate)
        {
            var meetings = _meetingService.getByDate(meetingDate);
            return Json(meetings);
        }

    }
}
