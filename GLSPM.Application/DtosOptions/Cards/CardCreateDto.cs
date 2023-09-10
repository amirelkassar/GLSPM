using AutoMapper;
using CubesFramework.Security;
using FluentValidation;
using GLSPM.Domain;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GLSPM.Application.Dtos.Cards
{
    public class CardCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }
        public IFormFile? Logo { get; set; }
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
        [Required]
        public string UserID { get; set; }
    }

    public class CardCreateDtoValidator : AbstractValidator<CardCreateDto>
    {
        public CardCreateDtoValidator()
        {
            RuleFor(c => c.Title)
              .NotEmpty()
              .WithMessage("The Card title is required")
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


            RuleFor(c => c.UserID)
                .NotEmpty();

            When(c => c.Logo != null, () =>
            {
                RuleFor(c => c.Logo.Length)
                     .ExclusiveBetween(1000, 5000000)
                     .WithMessage("The Logo image size should be 1kb to 5mb");
            });
        }
    }

    public class CardCreateDtoMappingAction : IMappingAction<CardCreateDto, Card>
    {
        private readonly Crypto _crypto;
        private readonly IConfiguration _configuration;
        private readonly FilesPathes _filesPathes;
        private readonly string _encryptionCode;
        public CardCreateDtoMappingAction(Crypto crypto,
            IConfiguration configuration,
            IOptions<FilesPathes> filesPathes)
        {
            _crypto = crypto;
            _configuration = configuration;
            _filesPathes = filesPathes.Value;
            _encryptionCode = configuration.GetSection("EncryptionCode").Value;
        }

        public void Process(CardCreateDto source, Card destination, ResolutionContext context)
        {
            destination.EncriptedCardNumber = _crypto.EncryptAes(source.CardNumber, _encryptionCode).Result;
            destination.EncriptedCVV = _crypto.EncryptAes(source.CVV, _encryptionCode).Result;
            if (source.Logo != null)
            {
                var logoPath = Path.Combine(Path.GetFullPath(_filesPathes.LogosPath), $"{DateTime.Now.ToFileTime()}{Path.GetExtension(source.Logo.FileName)}");
                using (FileStream logoStream = new FileStream(logoPath, FileMode.Create))
                {
                    source.Logo.CopyTo(logoStream);
                    logoStream.Flush();
                    destination.LogoPath = logoPath;
                }
            }
        }
    }
}
