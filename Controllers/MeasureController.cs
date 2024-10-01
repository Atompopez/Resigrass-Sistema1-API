using Microsoft.AspNetCore.Mvc;
using ResiGrass_API.Logic;

namespace ResiGrass_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeasureController : ControllerBase
    {
        private readonly DbQuery _dbQuery;

        public MeasureController(DbQuery dbQuery)
        {
            _dbQuery = dbQuery;
        }


        #region GetMeasures
        [HttpGet("Measures")]
        public IActionResult GetMeasure()
        {
            var measures = _dbQuery.GetMeasures();
            if(measures.Count != 0)
            {
                return Ok(measures);
            }
            else
            {
                return BadRequest("Error al consultar los datos");
            }
        }
        #endregion
    }
}
