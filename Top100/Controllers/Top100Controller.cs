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
        public async Task<IActionResult> CreateAsync(int year, int number, [FromBody]SongRequest songRequest)
        {
            var ret = await client.InsertAsync(new Song
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

        [Route("/API/v1/Top100/Get/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetAsync(string id)
        {
            var ret = await client.GetAsync(id);
            if ( ret != null)
            {
                return Ok(new SongResponse(ret));
            }
            return NotFound();
        }

        [Route("/API/v1/Top100/Find")]
        [HttpGet]
        public async Task<IActionResult> FindAsync()
        {
            var titleFilter = Request.Query["title"];
            var artistFilter = Request.Query["artist"];
            var yearFilter = Request.Query["year"];
            var numberFilter = Request.Query["number"];
            var ownFilter = Request.Query["own"];

            var retList = await client.FindAsync(titleFilter, artistFilter, yearFilter, numberFilter, ownFilter);
            if (retList != null)
            {
                return Ok(retList);
            }
            return NotFound();
        }
    }
}
