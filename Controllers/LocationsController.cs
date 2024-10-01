using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResiGrass_API.Logic;
using ResiGrass_API.Models;
using System.Collections.Generic;

namespace ResiGrass_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    // [Authorize] 
    public class LocationsController : ControllerBase
    {
        private readonly DbQuery _dbQuery;

        public LocationsController(DbQuery dbQuery)
        {
            _dbQuery = dbQuery;
        }

        [HttpGet("Municipalities")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetMunicipalities()
        {
            var municipalities = _dbQuery.GetMunicipalities();
            return Ok(municipalities);
        }

        [HttpPut("{idMunicipality}")]
        public IActionResult Localities(int idMunicipality)
        {
            var localities = _dbQuery.GetLocalities(idMunicipality);
            return Ok(localities);
        }



    }
}
