using Halcyon.HAL;
using Halcyon.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

using Microsoft.AspNetCore.Mvc;

namespace slingshotx.Controllers
{
    [Route("slingshot/v1")]
    public class DefaultController : Controller
    {

        [HttpGet("")]
        public IHttpActionResult Get()
        {
            DateTime date = DateTime.Now;

            var d2 = date.ToString("yyyy-MM-dd");
            var d1 = date.AddDays(-1).ToString("yyyy-MM-dd");
            var d3 = date.AddDays(1).ToString("yyyy-MM-dd");
            return this.HAL(
                new { },
                new Link[] {
                    // new Link("yesterdays meetings", $"v1/meeting/bydate/{d1}"),
                    new Link("todays meetings", $"v1/meeting/bydate/{d2}"),
                    // new Link("tomorrows meetings", $"v1/meeting/bydate/{d3}"),
                    // new Link("yesterdays races", $"v1/race/bydate/{d1}"),
                    new Link("todays races", $"v1/race/bydate/{d2}"),
                    // new Link("tomorrows races", $"v1/race/bydate/{d3}"),
                    new Link("race counts", $"v1/race/countbyhour/{d2}"),
                    new Link("all scratchings", $"v1/scratching/getall/{d2}"),
                    new Link("all pods", $"v1/pod/getall"),
                    new Link("pod meeting allocations", $"v1/pod/getallocations/{d2}"),
                    new Link("pod race allocations", $"v1/pod/getraceallocations/{d2}/1"),
                    new Link("pod seat user assignments", $"v1/pod/getusers/{d2}?podnumber=1&seatnumber=1"),
                    // new Link("next-to-jump", $"v1/nexttojump/bydate/{d2}/22"),
                    new Link("next-to-jump by status", $"v1/nexttojump/bydateandstatus/{d2}?count=22&status=OPEN,CLOSED"),
                    new Link("race user assignments", $"v1/race/userassignments/{d2}"),
                    new Link("all users", $"v1/user/getall"),
                    new Link("ready to pay", $"v1/race/readytopay/{d2}/10"),
                }
            );
        }
    }
}
