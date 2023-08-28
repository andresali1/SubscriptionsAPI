using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAuthors.DTOs;
using WebApiAuthors.Services;

namespace WebApiAuthors.Controllers.V1
{
    [ApiController]
    [Route("api/v1/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly KeyService _keyService;

        public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration,
                                 SignInManager<IdentityUser> signInManager, KeyService keyService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _keyService = keyService;
        }

        /// <summary>
        /// Method to Register a New User
        /// </summary>
        /// <param name="userCredentials">UserCredentials object with data</param>
        /// <returns></returns>
        [HttpPost("register", Name = "registerUser")] // api/account/register
        public async Task<ActionResult<AuthenticationResponse>> Register(UserCredentials userCredentials)
        {
            var user = new IdentityUser { UserName = userCredentials.Email, Email = userCredentials.Email };
            var result = await _userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                await _keyService.CreateKey(user.Id, KeyType.Free);

                return await BuildToken(userCredentials, user.Id);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        /// <summary>
        /// Method to login an existent User
        /// </summary>
        /// <param name="userCredentials">UserCredentials object with data</param>
        /// <returns></returns>
        [HttpPost("login", Name = "loginUser")]
        public async Task<ActionResult<AuthenticationResponse>> Login(UserCredentials userCredentials)
        {
            var result = await _signInManager.PasswordSignInAsync(userCredentials.Email, userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(userCredentials.Email);

                return await BuildToken(userCredentials, user.Id);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        /// <summary>
        /// Method used to renew atoken if expires
        /// </summary>
        /// <returns></returns>
        [HttpGet("RenewToken", Name = "renewToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<AuthenticationResponse>> Renew()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var idClaim = HttpContext.User.Claims.Where(claim => claim.Type == "id").FirstOrDefault();
            var userId = idClaim.Value;

            var userCredentials = new UserCredentials()
            {
                Email = email,
            };

            return await BuildToken(userCredentials, userId);
        }

        /// <summary>
        /// Method to create a new token
        /// </summary>
        /// <param name="userCredentials">UserCredentials object with data</param>
        /// <returns></returns>
        private async Task<AuthenticationResponse> BuildToken(UserCredentials userCredentials, string userId)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email),
                new Claim("id", userId)
            };

            var user = await _userManager.FindByEmailAsync(userCredentials.Email);
            var claimsDb = await _userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDb);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTkey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims, expires: expiration, signingCredentials: creds);

            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiration = expiration
            };
        }

        /// <summary>
        /// Method to give admin privileges to an User
        /// </summary>
        /// <param name="adminEditDTO">AdminEditDTO with data</param>
        /// <returns></returns>
        [HttpPost("BecomeAdmin", Name = "becomeAdmin")]
        public async Task<ActionResult> BecomeAdmin(AdminEditDTO adminEditDTO)
        {
            var user = await _userManager.FindByEmailAsync(adminEditDTO.Email);
            await _userManager.AddClaimAsync(user, new Claim("isAdmin", "1"));
            return NoContent();
        }

        /// <summary>
        /// Method to remove admin privileges to an User
        /// </summary>
        /// <param name="adminEditDTO">AdminEditDTO with data</param>
        /// <returns></returns>
        [HttpPost("RemoveAdmin", Name = "removeAdmin")]
        public async Task<ActionResult> RemoveAdmin(AdminEditDTO adminEditDTO)
        {
            var user = await _userManager.FindByEmailAsync(adminEditDTO.Email);
            await _userManager.RemoveClaimAsync(user, new Claim("isAdmin", "1"));
            return NoContent();
        }
    }
}
