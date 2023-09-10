using FluentValidation;
using GLSPM.Domain.Dtos.Passwords;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Client.Dtos.Passwords
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

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<PasswordUpdateDto>.CreateWithOptions((PasswordUpdateDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
