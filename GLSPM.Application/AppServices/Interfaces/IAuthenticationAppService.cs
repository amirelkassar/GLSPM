using GLSPM.Application.Dtos.Identity;
using GLSPM.Domain.Dtos;
using GLSPM.Domain.Dtos.Identity;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices.Interfaces
{
    public interface IAuthenticationAppService
    {
        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        string GenerateRandomPassword(PasswordOptions opts = null);
        Task<bool> ValidateUser(LoginUserDto user);
        Task<bool> ValidateUser(string useremail);
        Task<TokenModel> CreateUserToken(string oldtoken = null);
        Task<LoginResponseDto> CreateLoginResponse(ControllerBase controller);
        ApplicationUser User { get; }
        Task<SingleObjectResponse<object>> CreateNewUser(RegisterUserDto input);
    }
}
