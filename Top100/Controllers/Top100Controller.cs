using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Top100.Controllers
{
    public class Top100Controller : Controller
    {
        [Route("/API/v1/Top100/{year:int}/{number:int}/Create")]
        [HttpPost]
        public async Task<IActionResult> Create(int year, int number, [FromBody]Song song)
        {
            await Task.Delay(100);
            return Ok();
        }
    }
}
