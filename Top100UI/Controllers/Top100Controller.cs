//
// © Copyright 2017 Kevin Pearson
//

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Top100Common;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net;
using System.Threading;

namespace Top100UI.Controllers
{
    public class Top100Controller : Controller
    {
        private readonly IStore client;

        public Top100Controller(IStore client)
        {
            this.client = client;
        }

        public IActionResult Index() => View();

        //Create
        [Route("/API/v1/Top100/Songs/{year:int}/{number:int}")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(int year, int number, [FromBody]SongRequest songRequest, CancellationToken cancelToken)
        {
            var song = new Song
            {
                Title = songRequest.Title,
                Artist = songRequest.Artist,
                Year = year,
                Number = number,
                Own = songRequest.Own
            };
            try
            {
                var ret = await client.CreateAsync(song, cancelToken);

                if (ret != null)
                {
                    var resourceUri = new Uri(Request.GetDisplayUrl());
                    Console.WriteLine($"Uri={resourceUri}");
                    return Created(resourceUri, song);
                }
            }
            catch (Top100Exception e)
            {
                switch (e.Reason)
                {
                    case ReasonType.Conflict:
                        return StatusCode((int)HttpStatusCode.Conflict);
                    case ReasonType.Unknown:
                        return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
            return BadRequest();
        }

        //Get
        [Route("/API/v1/Top100/Songs/{year:int}/{number:int}")]
        [HttpGet]
        public async Task<IActionResult> GetAsync(int year, int number, CancellationToken cancelToken)
        {
            try
            {
                var ret = await client.ReadAsync(year, number, cancelToken);
                return Ok(ret);
            }
            catch (Top100Exception e)
            {
                switch (e.Reason)
                {
                    case ReasonType.NotFound:
                        return NotFound();
                }
            }
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [Route("/API/v1/Top100/Songs/{year:int}/{number:int}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int year, int number, CancellationToken cancelToken)
        {
            try
            {
                var ret = await client.DeleteAsync(year, number, cancelToken);
                if (ret != null)
                {
                    return Ok(ret);
                }
            }
            catch (Top100Exception e)
            {
                switch (e.Reason)
                {
                    case ReasonType.NotFound:
                        return NotFound();
                }
            }
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [Route("/API/v1/Top100/Songs/{year:int}/{number:int}")]
        [HttpPut]
        public async Task<IActionResult> PutAsync(int year, int number, [FromBody]SongRequest songRequest, CancellationToken cancelToken)
        {
            try
            {
                var song = new Song
                {
                    Title = songRequest.Title,
                    Artist = songRequest.Artist,
                    Year = year,
                    Number = number,
                    Own = songRequest.Own
                };

                var ret = await client.UpdateAsync(song, cancelToken);
                if (ret != null)
                {
                    return Ok(ret);
                }
            }
            catch (Top100Exception e)
            {
                switch (e.Reason)
                {
                    case ReasonType.NotFound:
                        return NotFound();
                }
            }
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [Route("/API/v1/Top100/Songs")]
        [HttpGet]
        public async Task<IActionResult> FindAsync(CancellationToken cancelToken)
        {
            var titleFilter = Request.Query["title"];
            var artistFilter = Request.Query["artist"];
            var yearFilter = Request.Query["year"];
            var numberFilter = Request.Query["number"];
            var ownFilter = Request.Query["own"];

            var retList = await client.SearchAsync(titleFilter, artistFilter, yearFilter, numberFilter, ownFilter, cancelToken);
            if (retList != null)
            {
                return Ok(retList);
            }
            return NotFound();
        }
    }
}
