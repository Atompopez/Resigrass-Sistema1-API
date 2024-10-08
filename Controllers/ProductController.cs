using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResiGrass_API.Logic;

namespace ResiGrass_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly DbQuery _dbQuery;

        public ProductController(DbQuery dbQuery)
        {
            _dbQuery = dbQuery;
        }


        #region GetProducts
        [HttpGet("Products")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetProducts()
        {
            var products = _dbQuery.GetProducts();
            if (products.Count != 0)
            {
                return Ok(products);
            }
            else
            {
                return BadRequest("Error al consultar el producto");
            }
        }
        #endregion
    }
}
