using FluentValidation;
using GLSPM.Domain;
using GLSPM.Domain.Dtos.Passwords;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Client.Dtos.Passwords
{
    public class PasswordCreateDtoValidator : AbstractValidator<PasswordCreateDto>
    {
        public PasswordCreateDtoValidator()
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

            When(c => c.Logo != null, () =>
            {
                RuleFor(c => c.Logo.Length)
                     .ExclusiveBetween(1000, 5000000)
                     .WithMessage("The Logo image size should be 1kb to 5mb");
            });

            RuleFor(c => c.UserID)
               .NotEmpty();
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<PasswordCreateDto>.CreateWithOptions((PasswordCreateDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
