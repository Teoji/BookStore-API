using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;



namespace BookStore_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsersController : BookStoreBaseController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public UsersController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILoggerService logger,
            IConfiguration config
            ) : base()
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// User register endpoint
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            var location = GetControllerActionName();
            try
            {
                var username = userDTO.EMailAddress;
                var password = userDTO.Password;
                _logger.LogInfo($"{location} : {username}/{password} Attemped Register");
                var user = new IdentityUser
                {
                    Email = username,
                    UserName = username
                }; 
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                { 
                  
                    foreach(var error in result.Errors)
                    {
                        _logger.LogInfo($"{location} : {error.Code}/{error.Description}");
                    }
                    return InternalError($"{location} : {username} - {password}");
                }
                _logger.LogInfo($"{location} : {username}/{password} successfully Registered");
                return Ok(new { result.Succeeded });
            }
            catch (Exception e)
            {
                return InternalError($"{location} : {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// User login endpoint
        /// 
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var username = userDTO.EMailAddress;
            var password = userDTO.Password;
            var location = GetControllerActionName();
            _logger.LogInfo($"{location} : {username}({password} Attemped login");
            try
            {

                var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
                if (result.Succeeded)
                {
                    var  benutzer = await _userManager.FindByNameAsync(username);
                    _logger.LogInfo($"{location} : {username}/{password} successfull login");
                    var tokenstring = await GenerateJWT(benutzer);
                    return Ok(new { token =tokenstring });
                }
                _logger.LogInfo($"{location} : {username}/{password} unknown user");
                return Unauthorized(userDTO);
            }
            catch (Exception e)
            {
                return InternalError($"{location} : {e.Message} - {e.InnerException}");
            }
        }

     

        private async Task<string> GenerateJWT(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                null,
                expires:DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
}
}