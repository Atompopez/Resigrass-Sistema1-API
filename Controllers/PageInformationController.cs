using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResiGrass_API.Logic;

namespace ResiGrass_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PageInformationController : Controller
    {
        private readonly DbQuery _dbQuery;

        public PageInformationController(DbQuery dbQuery)
        {
            _dbQuery = dbQuery;
        }

        #region GetWhatsappNumber
        [HttpGet("WhatsappNumber")]
        public IActionResult GetWhatsappNumber()
        {
            var whatsappNumber = _dbQuery.GetWhatsappNumber();
            return Ok(whatsappNumber);
        }
        #endregion

        #region GetIsValidCollector
        [HttpGet("GetIsValidCollector/{idCollector}/{pin}")]
        public IActionResult GetIsValidCollector(int idCollector, int pin)
        {
            var pin_db = _dbQuery.GetPinCollector(idCollector);
            if (pin_db == 0)
                return BadRequest("Error en la consulta");
            return Ok(pin_db == pin);
        }
        #endregion

        #region GetDataCollector
        [HttpGet("GetDataCollector/{id}")]
        public IActionResult GetDataCollector(int id)
        {
            var data = _dbQuery.GetDataCollector(id);

            if (data is not null)
                return Ok(data); 

            return BadRequest("Error en la consulta");
        }
        #endregion
    }
}
