using Account.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Account.Api.Controllers
{
    /// <summary>
    /// Account controller
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region Private fields
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        #endregion

        #region Public constructor
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Public methods
        [HttpPost(nameof(RegisterUser))]
        public async Task<IActionResult> RegisterUser(AccountRequest accountRequest)
        {
            var response = await Register(accountRequest, AccountType.User);
            return Ok(response);
        }

        [HttpPost(nameof(RegisterGameMaster))]
        public async Task<IActionResult> RegisterGameMaster(AccountRequest accountRequest)
        {
            var response = await Register(accountRequest, AccountType.GameMaster);
            return Ok(response);
        }

        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(AccountRequest accountRequest)
        {
            var loginResponse = new CommandResponse<string>();
            try
            {
                var response = await _signInManager.PasswordSignInAsync(accountRequest.Username, accountRequest.Password, false, false);

                if (response.Succeeded)
                {
                    loginResponse.Data = await CreateToken(accountRequest);

                    if (loginResponse.Data == null)
                    {
                        loginResponse.Message = new CommandMessage() { Text = "Failed to generate token", Type = MessageType.Error.ToString() };
                    }
                }
            }
            catch (Exception e)
            {
                loginResponse.Success = false;
                loginResponse.Message = new CommandMessage() { Text = "Login failed", Type = MessageType.Error.ToString() };
            }

            return Ok(loginResponse);
        }
        #endregion

        #region Private methods
        private async Task<CommandResponse> Register(AccountRequest accountRequest, AccountType accountType)
        {
            var registerResponse = new CommandResponse();
            try
            {
                var user = new IdentityUser(accountRequest.Username);
                var createUser = await _userManager.CreateAsync(user, accountRequest.Password);

                if (createUser.Succeeded)
                {
                    var role = accountType.ToString();
                    var roleResponse = await _userManager.AddToRoleAsync(user, role);
                    if (roleResponse.Succeeded)
                    {
                        registerResponse.Message = new CommandMessage()
                        {
                            Text = accountType.ToString() + " has been successfuly registered",
                            Type = MessageType.Information.ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                registerResponse.Success = false;
                registerResponse.Message = new CommandMessage() { Text = accountType.ToString() + " registration failed", Type = MessageType.Error.ToString() };
            }

            return registerResponse;
        }
        private async Task<string?> CreateToken(AccountRequest accountRequest)
        {
            var user = await _userManager.FindByNameAsync(accountRequest.Username);
            if (user == null)
            {
                return null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, user.Id) };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("afsdkjasjflxswafsdklk434orqiwup3457u-34oewir4irroqwiffv48mfs"));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);
            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: credentials
            );

            var tokenResponse = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenResponse;
        }
        #endregion
    }
}
