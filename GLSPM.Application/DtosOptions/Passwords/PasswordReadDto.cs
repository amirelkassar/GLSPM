using AutoMapper;
using CubesFramework.Security;
using FluentValidation;
using GLSPM.Domain;
using GLSPM.Domain.Dtos.Passwords;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
    public class PasswordToPasswordReadDtoMappingAction : IMappingAction<Password, PasswordReadDto>
    {
        private readonly Crypto _crypto;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly FilesPathes _filesPathes;
        private readonly string _encryptionCode;

        public PasswordToPasswordReadDtoMappingAction(Crypto crypto,
            IConfiguration configuration,
            IOptions<FilesPathes> filesPathes,
            IHttpContextAccessor httpContextAccessor,
            LinkGenerator linkGenerator)
        {
            _crypto = crypto;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
            _filesPathes = filesPathes.Value;
            _encryptionCode = configuration.GetSection("EncryptionCode").Value;
        }
        public void Process(Password source, PasswordReadDto destination, ResolutionContext context)
        {
            destination.Password = _crypto.DecryptAes(source.EncriptedPassword, _encryptionCode).Result;
            destination.LogoPath = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext,"Logo", "Passwords", new {id=source.ID});
        }
    }

    public class PasswordReadDtoToPasswordMappingAction : IMappingAction<PasswordReadDto, Password>
    {
        private readonly Crypto _crypto;
        private readonly IConfiguration _configuration;
        private readonly FilesPathes _filesPathes;
        private readonly string _encryptionCode;

        public PasswordReadDtoToPasswordMappingAction(Crypto crypto,
            IConfiguration configuration,
            IOptions<FilesPathes> filesPathes)
        {
            _crypto = crypto;
            _configuration = configuration;
            _filesPathes = filesPathes.Value;
            _encryptionCode = configuration.GetSection("EncryptionCode").Value;

        }

        public void Process(PasswordReadDto source, Password destination, ResolutionContext context)
        {
           destination.EncriptedPassword=_crypto.EncryptAes(source.Password, _encryptionCode).Result;
        }
    }
}
