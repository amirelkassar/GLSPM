using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.Dtos.Identity;
using GLSPM.Domain.Dtos;
using GLSPM.Domain.Dtos.Identity;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices
{
    public class AuthenticationAppService : IAuthenticationAppService
    {
        private readonly ILogger<AuthenticationAppService> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticationAppService(
            ILogger<AuthenticationAppService> logger,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
        }

        private ApplicationUser appuser;
        public ApplicationUser User { get => appuser; private set => appuser = value; }

        public async Task<TokenModel> CreateUserToken(string oldtoken = null)
        {
            var model = new TokenModel();
            //preparing the singing credentials
            var signingcredentials = GetSigningCredentials();
            //preparing the overload
            var claims = await GetClaims();
            //preparing the token
            DateTime lifetime;
            var token = GetSecurityToken(signingcredentials, claims, out lifetime);
            //preparing the model
            model.Token = new JwtSecurityTokenHandler().WriteToken(token);
            model.Expiration = lifetime;
            return model;
        }

        public string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        public async Task<bool> ValidateUser(LoginUserDto input)
        {
            appuser = await _userManager.FindByNameAsync(input.Username) ?? await _userManager.FindByEmailAsync(input.Username);
            return (appuser != null && await _userManager.CheckPasswordAsync(appuser, input.Password));
        }

        public async Task<bool> ValidateUser(string useremail)
        {
            appuser = await _userManager.FindByEmailAsync(useremail);
            return appuser != null;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = _configuration.GetSection("Jwt").GetSection("Key").Value;
            var keybytes = Encoding.UTF8.GetBytes(key);

            return new SigningCredentials(key: new SymmetricSecurityKey(keybytes),
            algorithm: SecurityAlgorithms.HmacSha256);
        }
        private async Task<ICollection<Claim>> GetClaims()
        {
            //user data
            var userclaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,appuser.UserName),
                new Claim(ClaimTypes.Email,appuser.Email),
                new Claim(JwtRegisteredClaimNames.Sub,appuser.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Sid,appuser.Id),
                new Claim(ClaimTypes.NameIdentifier,appuser.Id)
            };

            if (!string.IsNullOrEmpty(appuser.ImagePath))
                userclaims.Add(new Claim("UserImage", Path.GetFileName(appuser.ImagePath)));
            //user roles
            var roles = await _userManager.GetRolesAsync(appuser);
            foreach (var role in roles)
            {
                userclaims.Add(new Claim(ClaimTypes.Role, role));
            }
            return userclaims;
        }
        private JwtSecurityToken GetSecurityToken(SigningCredentials credentials, ICollection<Claim> claims, out DateTime lifetime)
        {
            //preparing the defualt data
            var jwtsettings = _configuration.GetSection("Jwt");
            lifetime = DateTime.Now.AddDays(1);
            //preparing the token
            var token = new JwtSecurityToken(
                issuer: jwtsettings.GetSection("Issuer").Value,
                audience: jwtsettings.GetSection("Audience").Value,
                claims: claims,
                signingCredentials: credentials,
                expires: lifetime);
            return token;
        }

        public async Task<LoginResponseDto> CreateLoginResponse(ControllerBase controller)
        {
            var loginresponse = new LoginResponseDto()
            {
                UserID = User.Id,
                Email = User.Email,
                Username = User.UserName,
                Avatar = controller.Url.Action("UserAvatar", "Accounts", new { userid = User.Id }, controller.Request.Scheme)
            };
            loginresponse.Roles = await _userManager.GetRolesAsync(User);
            loginresponse.IsAppAdmin = loginresponse.Roles.Contains("Admin");
            _logger.LogInformation("User data is ready");
            //preparing and setting the token model
            var tokenmodel = await CreateUserToken();
            loginresponse.Token = tokenmodel.Token;
            loginresponse.TokenExpirationDate = tokenmodel.Expiration;
            return loginresponse;
        }

        public async Task<SingleObjectResponse<object>> CreateNewUser(RegisterUserDto input)
        {
            var alreadyExists = await _userManager.FindByEmailAsync(input.Email) ?? await _userManager.FindByNameAsync(input.Username);
            if (alreadyExists != null)
            {
                return new SingleObjectResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status406NotAcceptable,
                    Message = "User already exists",
                    Error= "User already exists"

                };
            }
            var user = new ApplicationUser
            {
                UserName = input.Username,
                Email = input.Email,
            };
            if (input.Avatar != null)
            {
                var filepath = Path.GetFullPath($"./Files/Imgs/{DateTime.Now.ToFileTime()}{Path.GetExtension(input.Avatar.FileName)}");
                using (FileStream avatartstream = new FileStream(filepath, FileMode.Create))
                {
                    await input.Avatar.CopyToAsync(avatartstream);
                    await avatartstream.FlushAsync();
                    user.ImagePath = filepath;
                }
            }
            else
                user.ImagePath = Path.GetFullPath($"./Files/Imgs/userimg.png");

            var userCreation = await _userManager.CreateAsync(user, input.Password);
            if (userCreation.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return new SingleObjectResponse<object>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status201Created,
                    Message = "User Created"
                };
            }
            else
                return new SingleObjectResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Couldn't create the user",
                    Error = userCreation.Errors
                };
        }
    }
}
