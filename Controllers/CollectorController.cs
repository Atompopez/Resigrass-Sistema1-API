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
            try
            {
                var collectorModel = requestModel.CollectorModel;
                var loginCollectorModel = requestModel.LoginCollectorModel;
                var profileImageBase64 = requestModel.ProfileImage;

                byte[] profileImageBytes = null;
                if (!string.IsNullOrWhiteSpace(profileImageBase64))
                {
                    profileImageBytes = Convert.FromBase64String(profileImageBase64);
                }
                var client = _dbQuery.InsertCollector(collectorModel, loginCollectorModel, profileImageBytes);
                return Ok(client);
            }
            catch (FormatException ex)
            {
                return BadRequest($"Formato inválido en la imagen Base64: {ex.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear el recolector: {ex.Message}");
            }
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
      //  [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult CollectorColection([FromBody] RecolectionModelInsert CollectionInsertModel)
        {

            var client = _dbQuery.InsertCollection(CollectionInsertModel);

            return Ok(client);
        }

        #endregion

        #region ColectionGet
        [HttpPost("ColectionGet")]
        //  [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult ColectionGet()
        {

            var client = _dbQuery.GetAllCollections();

            return Ok(client);
        }

        #endregion

        #region Email

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
        }
        #endregion

        //#region TestCollectorCreation
        //[HttpPost("TestCollectorCreation")]
        //public IActionResult TestCollectorCreation()
        //{
        //    try
        //    {
                
        //        string imagePath = "./images/ds.png";

                
        //        if (!System.IO.File.Exists(imagePath))
        //        {
        //            return BadRequest("La imagen especificada no existe en la ruta proporcionada.");
        //        }

                
        //        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
        //        string base64Image = Convert.ToBase64String(imageBytes);

                
        //        var requestModel = new CollectorRequestModel
        //        {
        //            CollectorModel = new CollectorModelInsert
        //            {
        //                nameCollector = "Prueba Local",
        //                numberPhoneCollector = "123456789",
        //                status = true,
        //                loginCollectorId = 0,
        //                typeCollectorId = 1,
        //                dateCreationCollector = DateTime.Now
        //            },
        //            LoginCollectorModel = new loginCreationCollectorModel
        //            {
        //                user = "prueba.local",
        //                password = "securePassword123",
        //                status = true
        //            },
        //            ProfileImage = base64Image
        //        };

                
        //        var response = CollectorCreation(requestModel);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error al probar la creación del recolector: {ex.Message}");
        //    }
        //}
        //#endregion




    }
}
