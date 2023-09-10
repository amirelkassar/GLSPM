using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos.Cards
{
    public class CardUpdateDto
    {
        [Required]
        public int ID { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }
        public string? AdditionalInfo { get; set; }
        [StringLength(250, MinimumLength = 3)]
        [Required]
        public string HolderName { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public int ExpiryMonth { get; set; }
        [Required]
        public int ExpiryYear { get; set; }
        [Required]
        public string CVV { get; set; }
    }

    public class CardUpdateDtoValidator : AbstractValidator<CardUpdateDto>
    {
        public CardUpdateDtoValidator()
        {
            RuleFor(c => c.ID)
                .NotEmpty()
                .WithMessage("The card ID is required");

            RuleFor(c => c.Title)
                .NotEmpty()
                .WithMessage("The card title is required")
                .Length(2, 50)
                .WithMessage("The card title should be 2 to 50 charachters");

            #region Card Props
            RuleFor(c => c.HolderName)
                   .NotEmpty()
                   .WithMessage("The HolderName is required")
                   .Length(3, 250)
                   .WithMessage("The HolderName should be 3 to 250 charachters");

            RuleFor(c => c.CardNumber)
                    .NotEmpty()
                    .WithMessage("The CardNumber is required")
                    .Length(14);

            RuleFor(c => c.ExpiryMonth)
                  .NotEmpty()
                  .WithMessage("The ExpiryMonth is required")
                  .ExclusiveBetween(01, 12);

            RuleFor(c => c.ExpiryYear)
                  .NotEmpty()
                  .WithMessage("The ExpiryYear is required")
                  .ExclusiveBetween(DateTime.Now.Year, 9999);

            RuleFor(c => c.CVV)
                     .NotEmpty()
                     .WithMessage("The CVV is required")
                     .Length(3);
            #endregion
        }
    }
}
