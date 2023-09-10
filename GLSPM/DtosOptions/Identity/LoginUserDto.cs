using FluentValidation;
using GLSPM.Domain.Dtos.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Client.Dtos.Identity
{
    public class LoginUserDtoValidtor : AbstractValidator<LoginUserDto>
    {
        public LoginUserDtoValidtor()
        {
            RuleFor(l => l.Username)
                .NotEmpty();

            RuleFor(l => l.Password)
                .NotEmpty()
                .MinimumLength(8);
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<LoginUserDto>.CreateWithOptions((LoginUserDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
