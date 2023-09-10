using FluentValidation;
using GLSPM.Domain.Dtos.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos.Identity
{
    public class RegisterUserDtoValidator:AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(u=>u.Username)
                .NotEmpty()
                .MinimumLength(4);

            RuleFor(u => u.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress();

            When(u => u.Avatar != null, () =>
            {
                RuleFor(u => u.Avatar.Length)
                .ExclusiveBetween(1000, 5000000);
            });
        }
    }
}
