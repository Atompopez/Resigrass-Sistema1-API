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
    }
}
