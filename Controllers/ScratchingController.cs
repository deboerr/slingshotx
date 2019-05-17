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
    [Route("slingshot/v1/scratching")]
    public class ScratchingController : Controller
    {
        ScratchingService _scratchingService;

        public ScratchingController(ScratchingService scratchingService)
        {
             _scratchingService = scratchingService;
        }

        // GET slingshot/v1/scratching/bydate/yyyy-mm-dd
        [HttpGet("bydate/{scratchingDate}")]
        public JsonResult GetByDate(DateTime scratchingDate)
        {
            var scratchings = _scratchingService.getByDate(scratchingDate);
            return Json(scratchings);
        }

    }
}
