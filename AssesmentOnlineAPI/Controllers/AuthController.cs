using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AssesmentOnlineAPI.DTO;
using AssesmentOnlineAPI.Models;
using AssesmentOnlineAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AssesmentOnlineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _services;
        private readonly IConfiguration _config;
        public AuthController(IAuthServices services,IConfiguration config)
        {
            _services = services;
            _config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(userForRegisterDTO registerUser)
        {
            if (registerUser.InitialBalance == null)
                registerUser.InitialBalance = 0;

            registerUser.Username = registerUser.Username.ToLower();
            //if (await _services.isExistingUser(registerUser.Username))
            //    return BadRequest("Username '" + registerUser.Username + "' already exists");

            var newUser = new User();
            newUser = registerUser;

            var createdNewUser = await _services.Register(newUser, registerUser.Password);

            return Ok(createdNewUser.Id);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(userForLoginDTO userLogin)
        {
            var activeUser = await _services.Login(userLogin.Username, userLogin.Password);
            if (activeUser == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, activeUser.Id.ToString()),
                new Claim(ClaimTypes.Name,activeUser.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(14),
                SigningCredentials = credential
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });

        }
    }
}