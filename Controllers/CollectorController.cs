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

        #region GetNextNumber
        [HttpGet("GetNextNumber/{idClient}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public string GetNextNumber()
        {
            var number = _dbQuery.GetNextNumber();
            return number;
        }
        #endregion

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

        #region ControllerUpdate
        [HttpPut("UpdateCollector/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult UpdateCollector(int id, [FromBody] CollectorModelUpdate requestModel)
        {
            try
            {
                // Validar que el modelo de actualización no sea nulo
                if (requestModel == null)
                    return BadRequest("El modelo de actualización no puede ser nulo.");

                // Convertir la imagen Base64 a bytes (si existe)
                byte[] profileImageBytes = null;
                if (!string.IsNullOrWhiteSpace(requestModel.ProfileImageBase64))
                {
                    try
                    {
                        profileImageBytes = Convert.FromBase64String(requestModel.ProfileImageBase64);
                    }
                    catch (FormatException)
                    {
                        return BadRequest("Formato inválido en la imagen Base64.");
                    }
                }

                // Llamar al método de actualización
                bool isUpdated = _dbQuery.UpdateCollector(id, requestModel, profileImageBytes);

                // Verificar si se actualizó correctamente
                if (isUpdated)
                    return Ok($"Recolector con ID {id} actualizado correctamente.");
                else
                    return NotFound($"No se encontró el recolector con ID {id} o no se pudo actualizar.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al actualizar el recolector: {ex.Message}");
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
            var number = _dbQuery.GetNextNumber();
            var newNumber = $"RS-01-{number}";

            if (number != "0")
            {
                var client = _dbQuery.InsertCollection(CollectionInsertModel, newNumber);
                return Ok(client);
            }
            return BadRequest("Error al consultar el número de recolección");
        }

        #endregion

        #region UpdateCollection
        [HttpPut("UpdateCollection")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult UpdateCollection([FromBody] UpdateCollectionModel updateModel)
        {
            try
            {                
                if (updateModel == null || updateModel.CollectionId <= 0)
                {
                    return BadRequest("Datos inválidos para actualizar la recolección.");
                }

                var isUpdated = _dbQuery.UpdateCollection(updateModel.CollectionId, updateModel.FullPayment, updateModel.NetWeight);

                if (isUpdated)
                {
                    return Ok(new { message = "Recolección actualizada correctamente." });
                }
                else
                {
                    return NotFound(new { message = "No se encontró la recolección a actualizar." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error al actualizar la recolección: {ex.Message}" });
            }
        }
        #endregion

        #region ColectionGet
        [HttpPost("ColectionGet")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult ColectionGet()
        {

            var client = _dbQuery.GetAllCollections();

            return Ok(client);
        }

        #endregion

        #region GetTotalOilByDate
        [HttpPost("GetTotalOilByDate")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetTotalOilByDate([FromBody] DateRangeRequest dateRange)
        {
            if (dateRange == null || dateRange.StartDate == default || dateRange.EndDate == default)
            {
                return BadRequest("El rango de fechas es inválido.");
            }

            if (dateRange.StartDate > dateRange.EndDate)
            {
                return BadRequest("La fecha de inicio no puede ser mayor que la fecha de fin.");
            }

            try
            {
                var totalOil = _dbQuery.GetWeeklyOilByDateRange(dateRange.StartDate, dateRange.EndDate);

                return Ok(totalOil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar la solicitud: {ex.Message}");
            }
        }
        #endregion

        #region AllCollectorsGet
        [HttpPost("AllCollectorsGet")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult AllCollectorsGet()
        {

            var client = _dbQuery.AllCollectorsGet();

            return Ok(client);
        }

        #endregion

        #region Mail
        [HttpPost("receive-collection")]
        public async Task<IActionResult> ReceiveCollection([FromBody] List<int> recolectionIds)
        {
            try
            {
                if (recolectionIds == null || !recolectionIds.Any())
                {
                    return BadRequest("La lista de IDs de recolecciones está vacía o es inválida.");
                }

                var records = new List<RecolectionModel>();

                foreach (var id in recolectionIds)
                {
                    var record = _dbQuery.GetRecolectionById(id);
                    if (record != null)
                    {
                        records.Add(record);
                    }
                }

                if (!records.Any())
                {
                    return NotFound("No se encontraron recolecciones para los IDs proporcionados.");
                }

            
                await bk.SendEmailAsync(records);

               
                foreach (var record in records)
                {
                    _dbQuery.MarkAsSent(record.id);
                }

                return Ok("Recolección recibida y notificaciones enviadas.");
            }                                           
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al recibir la recolección: {ex.Message}");
            }
        }
        #endregion

        #region TokenLanding
        [HttpGet("GetCollectorLandig/{id}")]
     //   [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult CollectorToken(int id)
        {
            var collector = _dbQuery.GetToken(id);
            return Ok(collector);
        }

        #endregion


    }
}
