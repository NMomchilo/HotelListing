using AutoMapper;
using HotelListing.Data;
using HotelListing.IReposiroty;
using HotelListing.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelListing.Controllers
{
    [ApiVersion("2.0", Deprecated = true)]
    [Route("api/country")]//"api/{v:apiversion}/country"  api/2.0/country
    [ApiController]
    public class CountryV2Controller : ControllerBase
    {
        private readonly DatabaseContext context;

        public CountryV2Controller(DatabaseContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries()
        {
            return Ok(this.context.Countries);
        }
    }
}
