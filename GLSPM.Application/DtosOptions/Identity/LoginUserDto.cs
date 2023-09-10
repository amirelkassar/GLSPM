using FluentValidation;
using GLSPM.Domain.Dtos.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos.Identity
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
    }
}
