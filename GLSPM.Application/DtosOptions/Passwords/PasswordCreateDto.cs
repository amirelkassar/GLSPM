using AutoMapper;
using CubesFramework.Security;
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

namespace GLSPM.Application.Dtos.Passwords
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
    }

    public class PasswordCreateDtoToPasswordMappingAction : IMappingAction<PasswordCreateDto, Password>
    {
        private readonly Crypto _crypto;
        private readonly IConfiguration _configuration;
        private readonly FilesPathes _filesPathes;
        private readonly string _encryptionCode;


        public PasswordCreateDtoToPasswordMappingAction(Crypto crypto,
            IConfiguration configuration,
            IOptions<FilesPathes> filesPathes)
        {
            _crypto = crypto;
            _configuration = configuration;
            _filesPathes = filesPathes.Value;
            _encryptionCode = configuration.GetSection("EncryptionCode").Value;
        }
        public void Process(PasswordCreateDto source, Password destination, ResolutionContext context)
        {
            destination.EncriptedPassword = _crypto.EncryptAes(source.Password, _encryptionCode).Result;
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
