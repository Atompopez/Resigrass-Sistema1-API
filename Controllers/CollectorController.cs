using Microsoft.AspNetCore.Mvc;
using ResiGrass_API.Logic;
using ResiGrass_API.Models;

namespace ResiGrass_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CollectorController : ControllerBase
    {
        private readonly DbQuery _dbQuery;

        public CollectorController(DbQuery dbQuery)
        {
            _dbQuery = dbQuery;
        }

        #region GetTypeCollector
        [HttpGet("TypeCollector")]
        public IActionResult GetTypeCollectors()
        {
            var TypeCollector = _dbQuery.GetTypeCollectors();
            if (TypeCollector.Count != 0)
            {
                return Ok(TypeCollector);
            }
            else
            {
                return BadRequest("Error al consultar el tipo de recolector");
            }
        }
        #endregion

        #region ControllerCreation
        [HttpPost("CollectorCreation")]
        public IActionResult CollectorCreation([FromBody] CollectorRequestModel requestModel)
        {
            var collectorModel = requestModel.CollectorModel;
            var loginCollectorModel = requestModel.LoginCollectorModel;

            var client = _dbQuery.InsertCollector(collectorModel, loginCollectorModel);
            return Ok(client);
        }

        #endregion

        #region ControllerLoginGet
        [HttpPost("ControllerLoginGet")]
        public IActionResult CollectControllerLoginGetorCreation([FromBody] loginCreationCollectorModelValidate LoginCollector)
        {
            var response = _dbQuery.CollectorLoginGet(LoginCollector);

            if (!response.Success)
            {
                if (response.Message.Contains("no existe"))
                {
                    return NotFound(new { message = response.Message }); 
                }
                else if (response.Message.Contains("incorrecta"))
                {
                    return Unauthorized(new { message = response.Message }); 
                }
                else
                {
                    return BadRequest(new { message = response.Message }); 
                }
            }

            AuthController Auth = new AuthController();
            UserCredentials crede = new UserCredentials();
            crede.Username = "resigrass";
            string token = Auth.GenerateJwtToken(crede.Username);
            return Ok(new { message = response.Message, data = response.Collectors, Token = token });
        }


        #endregion

        #region CollectorColection
        [HttpPost("Colection")]
        public IActionResult CollectorColection([FromBody] RecolectionModelInsert CollectionInsertModel)
        {

            var client = _dbQuery.InsertCollection(CollectionInsertModel);

            return Ok(client);
        }

        #endregion
    }
}
