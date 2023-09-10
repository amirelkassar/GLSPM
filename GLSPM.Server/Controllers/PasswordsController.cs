using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.Dtos;
using GLSPM.Application.Dtos.Passwords;
using GLSPM.Domain;
using GLSPM.Domain.Dtos;
using GLSPM.Domain.Dtos.Passwords;
using HeyRed.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using FileClass = System.IO.File;
namespace GLSPM.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class PasswordsController : ControllerBase
    {
        private readonly ILogger<PasswordsController> _logger;
        private readonly IPasswordsAppService _passwordsAppService;
        private readonly FilesPathes _filesPathes;

        public PasswordsController(ILogger<PasswordsController> logger,
            IPasswordsAppService passwordsAppService,
            IOptions<FilesPathes> filesPathes)
        {
            _logger = logger;
            _passwordsAppService = passwordsAppService;
            _filesPathes = filesPathes.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetListDto input)
        {
            input.Sorting ??= nameof(PasswordReadDto.Title);
            var results= await _passwordsAppService.GetListAsync(input);
            return Ok(results);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var results = await _passwordsAppService.GetAsync(id);
            return results.Success? Ok(results) : BadRequest(results);
        }
        [HttpGet("Trashed")]
        public async Task<IActionResult> GetTrashed([FromQuery]PaginationParametersBase pagination)
        {
            var results = await _passwordsAppService.GetDeletedAsync(pagination);
            return Ok(results);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]PasswordCreateDto input)
        {
            var results=await _passwordsAppService.CreateAsync(input);
            return results.Success ? Created("",results) : BadRequest(results);
        }
        [HttpPost("Generate")]
        public async Task<IActionResult> GeneratePassword([FromBody] int length)
        {
            var results = await _passwordsAppService.GeneratePassword(length);
            return results.Success ? Created("", results) : BadRequest(results);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody]PasswordUpdateDto input)
        {
            var results = await _passwordsAppService.UpdateAsync(id,input);
            return results.Success ? Accepted(results) : BadRequest(results);
        }
        [HttpPut("Logo")]
        public async Task<IActionResult> ChnageLogo([FromForm] ChangeLogoDto<int> input)
        {
            await _passwordsAppService.ChangeLogo(input);
            return Accepted(new SingleObjectResponse<object>
            {
                Success=true,
                StatusCode=StatusCodes.Status202Accepted,
                Message="Logo Updated",
            });
        }
        [HttpPost("Trash/{id}")]
        public async Task<IActionResult> MoveToTrash(int id)
        {
            await _passwordsAppService.MarkAsDeletedAsync(id);
            return Accepted(new SingleObjectResponse<object>
            {
                Success = true,
                StatusCode=StatusCodes.Status202Accepted,
                Message="Item Moved to Trash"
            });
        }
        [HttpPut("Restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var results=await _passwordsAppService.UnMarkAsDeletedAsync(id);
            return results.Success ? Accepted(results) : BadRequest(results);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _passwordsAppService.DeleteAsync(id);
            return NoContent();
        }
        [HttpGet("Logo/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Logo(int id)
        {
            var logoPath = await _passwordsAppService.GetLogoPathAsync(id);
            if (logoPath != null)
            {
                return PhysicalFile(logoPath, MimeTypesMap.GetMimeType(logoPath), $"logo{Path.GetExtension(logoPath)}");
            }
            return NotFound(new SingleObjectResponse<object>
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "User Or Image not found",
                Error = "Couldn't find a user or and Image related to the passed id"
            });
        }
    }
}
