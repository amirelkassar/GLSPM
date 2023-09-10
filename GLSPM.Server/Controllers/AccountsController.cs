using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.Dtos;
using GLSPM.Application.Dtos.Identity;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HeyRed.Mime;
using GLSPM.Domain.Dtos.Identity;
using GLSPM.Domain.Dtos;
using FileClass = System.IO.File;
namespace GLSPM.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticationAppService _authenticationAppService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountsController(ILogger<AccountsController> logger,
            UserManager<ApplicationUser> userManager,
            IAuthenticationAppService authenticationAppService,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _authenticationAppService = authenticationAppService;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromForm] RegisterUserDto input)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("invalid model passed.");
                var response = new SingleObjectResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "invalid model passed.",
                    Error = ModelState
                };
                return BadRequest(response);
            }
            var results = await _authenticationAppService.CreateNewUser(input);
            switch (results.StatusCode)
            {
                case 400:
                case 406:
                    return BadRequest(results);
                default:
                    return Created("", results);

            }
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto input)
        {
            _logger.LogInformation("Attimpting to login a user...");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("invalid model passed.");
                var response = new SingleObjectResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "invalid model passed.",
                    Error = ModelState
                };
                return BadRequest(response);
            }
            if (!await _authenticationAppService.ValidateUser(input))
            {
                _logger.LogWarning("User not found");
                var response = new SingleObjectResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Error = "incorrect username or password"
                };
                return NotFound(response);
            }
            else
            {
                await _signInManager.SignInAsync(_authenticationAppService.User,false);
                _logger.LogInformation("User logged");
                _logger.LogInformation("Attempting to preparing the response...");
                //preparing the user data
                var loginresponse = await _authenticationAppService.CreateLoginResponse(this);
                _logger.LogInformation("Token is ready");
                var response = new SingleObjectResponse<LoginResponseDto>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Login Success",
                    Data = loginresponse
                };
                return Ok(response);
            }
        }

        [HttpGet("UserAvatar/{userID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UserAvatar(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            if (user != null)
            {
                user.ImagePath ??= Path.GetFullPath("./Files/Imgs/userimg.png");
                if (FileClass.Exists(user.ImagePath))
                {
                    return PhysicalFile(user.ImagePath, MimeTypesMap.GetMimeType(user.ImagePath), $"avatar{Path.GetExtension(user.ImagePath)}");
                }
            }
            return NotFound(new SingleObjectResponse<object>
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "User not found",
                Error = "Couldn't find a user related to the passed id"
            });
        }
    }
}
