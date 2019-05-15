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
    [Route("slingshot/v1/race")]
    public class RaceController : Controller
    {
        RaceService _raceService;

        public RaceController(RaceService raceService)
        {
             _raceService = raceService;
        }

        // GET slingshot/v1/race/bymeeting/111-aaa-222-bbb
        [HttpGet("bymeeting/{meetingId}")]
        public JsonResult GetByMeeting(string meetingId)
        {
            var races = _raceService.getByMeetingId(meetingId);
            return Json(races);
        }

    }
}
