using FluentValidation;
using GLSPM.Domain.Dtos.Identity;

namespace GLSPM.Client.Dtos.Identity
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(u => u.Username)
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
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<RegisterUserDto>.CreateWithOptions((RegisterUserDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
