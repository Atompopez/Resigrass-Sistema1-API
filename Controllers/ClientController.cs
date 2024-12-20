using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResiGrass_API.Logic;
using ResiGrass_API.Models;
using System.Collections.Generic;

namespace ResiGrass_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly DbQuery _dbQuery;

        public ClientController(DbQuery dbQuery)
        {
            _dbQuery = dbQuery;
        }

        #region GetTypeBusiness
        [HttpGet("TypeBusiness")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetTypeBusiness()
        {
            var typebusiness = _dbQuery.GetTypeBusiness();
            return Ok(typebusiness);
        }
        #endregion

        #region TypeBusinessCreation
        [HttpPost("TypeBusinessCreation")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult TypeBusinessCreation(TypeBusinessModel TypeBusiness)
        {
            var client = _dbQuery.InsertTypeBusiness(TypeBusiness);
            return Ok(client);
        }
        #endregion

        #region GetClients
        [HttpGet("{idTypeBusiness}")]
        //[Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetClients(int idTypeBusiness)
        {
            var client = _dbQuery.GetClients(idTypeBusiness);
            return Ok(client);
        } 
        #endregion

        #region ClientCreation
        [HttpPost("ClientCreation")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult ClientCreation(ClientModelInsert ClientModel)
        {
            var client = _dbQuery.InsertClient(ClientModel);
            return Ok(client);
        } 
        #endregion

        #region ClientUpdate
        [HttpPut("ClientUpdate/{IdClient}")]
     //   [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult ClientUpdate(int IdClient, [FromBody] ClientModelInsert clientModel)
        {
            var existingClient = _dbQuery.GetClient(IdClient);

            if (existingClient.Count == 0)
            {

                return NotFound($"Cliente con ID {IdClient} no encontrado.");
            }

            try
            {

                clientModel.nitCc = existingClient.FirstOrDefault()?.nitCc;

                var updatedClient = _dbQuery.ClientUpdate(clientModel, IdClient);
                if (updatedClient.Count == 0)
                {
                    return BadRequest("Error al actualizar el cliente.");
                }
                return Ok(updatedClient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el cliente: {ex.Message}");
            }
        }
        #endregion

        #region GetHeadquarters
        [HttpGet("{clientId}/{idLocality}/GetHeadquarters")]
   //     [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetHeadquarters(int clientId, int idLocality)
        {
            var client = _dbQuery.GetHeadquarters(clientId, idLocality);
            return Ok(client);
        } 
        #endregion

        #region HeadquartersCreation
        [HttpPost("HeadquartersCreation")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult HeadquartersCreation(HeadQuartersModelCreation HeadQuarterModel)
        {
            var client = _dbQuery.HeadquartersCreation(HeadQuarterModel);
            if (client.Count == 0)
            {
                return BadRequest("Error al insertar la sede. No se pudo completar la operación.");
            }
            else
            {
                return Ok(client);
            }
        }
        #endregion

        #region HeadquartersUpdate
        [HttpPut("HeadquartersUpdate/{IdHeadquarter}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult HeadquartersUpdate(int IdHeadquarter, [FromBody] HeadQuartersModelCreation HeadQuarter)
        {
            var existingHeadquarter = _dbQuery.HeadquarterGet(IdHeadquarter);

            if (existingHeadquarter.Count == 0)
            {

                return NotFound($"Cliente con ID {IdHeadquarter} no encontrado.");
            }

            try
            {                

                var updatedheadquarter = _dbQuery.HeadQuarterUpdate(HeadQuarter, IdHeadquarter);
                if (updatedheadquarter.Count == 0)
                {
                    return BadRequest("Error al actualizar el cliente.");
                }
                return Ok(updatedheadquarter);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el cliente: {ex.Message}");
            }
        }
        #endregion
    }
}
