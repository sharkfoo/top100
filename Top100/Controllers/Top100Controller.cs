//
// © Copyright 2017 Kevin Pearson
//

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Top100Common;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Top100.Controllers
{
    public class Top100Controller : Controller
    {
        private IStore client;

        public Top100Controller(IStore client)
        {
            this.client = client;
        }

        [Route("/API/v1/Top100/{year:int}/{number:int}/Create")]
        [HttpPost]
        public async Task<IActionResult> Create(int year, int number, [FromBody]SongRequest songRequest)
        {
            var ret = await client.Insert(new Song
            {
                Title = songRequest.Title,
                Artist = songRequest.Artist,
                Year = year,
                Number = number,
                Own = false
            });

            if (ret != null)
                return Ok(new SongResult(ret));

            return BadRequest();
        }

        [Route("/API/v1/Top100/{year:int}/{number:int}/Create")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Task.Delay(100);
            return Ok();
        }

    }
}
