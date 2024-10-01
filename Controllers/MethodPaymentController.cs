using Microsoft.AspNetCore.Mvc;
using ResiGrass_API.Logic;

namespace ResiGrass_API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class MethodPaymentController : ControllerBase
    {
        private readonly DbQuery _dbQuery;

        public MethodPaymentController(DbQuery dbQuery)
        {
            _dbQuery = dbQuery;
        }


        #region GetMethodPayment
        [HttpGet("MethodPayment")]
        public IActionResult GetMethodPayment()
        {
            var methodpayment = _dbQuery.GetMethodPayment();
            if (methodpayment.Count != 0)
            {
                return Ok(methodpayment);
            }
            else
            {
                return BadRequest("Error al consultar los métodos de pago");
            }
        }
        #endregion
    }
}
