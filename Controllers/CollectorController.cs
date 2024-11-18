using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly Logic.EmailNotificationService  bk;
        public CollectorController(DbQuery dbQuery, EmailNotificationService emailNotificationService)
        {
            _dbQuery = dbQuery;
            bk = emailNotificationService;
        }


        #region GetTypeCollector
        [HttpGet("TypeCollector")]
        [Authorize(AuthenticationSchemes = "Bearer")]
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
        [Authorize(AuthenticationSchemes = "Bearer")]
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
            return Ok(new { message = response.Message, data = response.Data, Token = token });
        }


        #endregion

        #region CollectorColection
        [HttpPost("Colection")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult CollectorColection([FromBody] RecolectionModelInsert CollectionInsertModel)
        {

            var client = _dbQuery.InsertCollection(CollectionInsertModel);

            return Ok(client);
        }

        #endregion

        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            try
            {
                var testClients = new List<RecolectionModel>
        {
            new RecolectionModel {  id = 2, bowlEmpty = 1,  headquarterId = 1, netWeight = 10, priceUnit = 10,endDate = DateTime.Now , fullPayment = 10,
            observations = "dsfdf", receivedDate = DateTime.Now , receivedFull = 10 }
        };

                await bk.SendNotificationsAsync();

                return Ok("Correo de prueba enviado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al enviar el correo de prueba: {ex.Message}");
            }
        } // PRUEBA PARA ENVIO DE EMAIL

    }
}
