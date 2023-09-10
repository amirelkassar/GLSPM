using FluentValidation;
using GLSPM.Domain.Dtos.Passwords;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos.Passwords
{
    public class PasswordUpdateDtoValidator:AbstractValidator<PasswordUpdateDto>
    {
        public PasswordUpdateDtoValidator()
        {
            RuleFor(c => c.Title)
            .NotEmpty()
            .WithMessage("The title is required")
            .Length(2, 50)
            .WithMessage("The title should be 2 to 50 charachters");

            RuleFor(p => p.Password)
                .NotEmpty()
                .WithMessage("The password is required");

            RuleFor(p => p.Username)
               .NotEmpty()
               .WithMessage("The username is required");
        }
    }
}
