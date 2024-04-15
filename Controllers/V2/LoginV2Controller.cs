using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace TuitionApi.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/Login")]
    [ControllerName("Login Version 2.0")]
    [ApiController]
    public class LoginV2Controller : ControllerBase
    {
        private IConfiguration _config;
        private DummyDataContext _dummyDataContext;

        public LoginV2Controller(DummyDataContext dummyDataContext, IConfiguration config)
        {
            this._dummyDataContext = dummyDataContext;
            this._config = config;
        }

        /// <summary>
        /// Provide your username and password in response you will get a json string that contains your JWT Token, and you user information.
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserLoginDtoRequest userLogin)
        {
            var user = Authenticate(userLogin);

            if (user != null)
            {
                var token = Generate(user);
                return Ok(new UserLoginDtoResponse { Token = token, User = new Models.Dto.UserDto { Name = user.Name, Surname = user.Surname } });
            }

            return NotFound();
        }

        private string Generate(UserModel user)
        {
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "_?"));
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey,
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Username)
                };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel Authenticate(UserLoginDtoRequest userLogin)
        {
            var currentUser = _dummyDataContext.Users.FirstOrDefault(o => o.Username.ToLower() == userLogin.Username.ToLower() && o.Password == userLogin.Password);

            if (currentUser != null)
            {
                return currentUser;
            }

            return null;
        }
    }
}