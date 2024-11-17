using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResiGrass_API.Logic;
using ResiGrass_API.Models;

namespace ResiGrass_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserAdminController : ControllerBase
    {
        private readonly DbQuery _dbQuery;

        public UserAdminController(DbQuery dbQuery)
        {
            _dbQuery = dbQuery;
        }

        #region adminUserCreation
        [HttpPost("AdminUserCreation")]
        //[Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult AdminUsersCreate([FromBody] userAdminModel requestModel)
        {
           var client = _dbQuery.UserAdminCreation(requestModel);
            return Ok(client);
        }

        #endregion

        [HttpPost("ControllerLoginUserAdmin")]
        public IActionResult userAdminVerification([FromBody] userAdminLoginModel requestModel)
        {
            var response = _dbQuery.UserAdminLogin(requestModel);

            if (!response.Success)
            {
                if (response.Message.Contains("no existe"))
                {
                    return NotFound(new { message = response.Message });
                }
                else if (response.Message.Contains("incorrecta"))
                {
                    return BadRequest(new { message = response.Message });
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

    }
}
