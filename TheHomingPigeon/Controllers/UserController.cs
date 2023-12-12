using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TheHomingPigeon.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using TheHomingPigeon.Middlewares;
using TheHomingPigeon.Data.Interface;


namespace TheHomingPigeon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtValidateController _jwtValidateController;

        public UserController(IUserRepository userRepository, JwtValidateController jwtController)
        {
            _userRepository = userRepository;
            _jwtValidateController = jwtController;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {

            var token = HttpContext.Request.Headers["Token"].FirstOrDefault()?.Split(" ").Last();

            var result = await _jwtValidateController.HandleTokenValidation(token);

            if (result != null)
            {
                return result;
            }

            var users = await _userRepository.GetActiveUsers();

            return Ok(users);
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var validateExistUser = await _userRepository.ValidateExistUser(user.username, user.email);

            if (validateExistUser)
            {
                var responseExistUser = new
                {
                    success = false,
                    status = 400,
                    message = "El nombre de usuarion o el correo ya estan en uso",
                    result = false
                };

                return Conflict(responseExistUser);
            }

            await _userRepository.InsertUser(user);

            var response = new
            {
                success = true,
                status = 200,
                message = "Usuario registrado exitosamente",
                result = true
            };

            return Created("Created", response);
        }
    }
}
