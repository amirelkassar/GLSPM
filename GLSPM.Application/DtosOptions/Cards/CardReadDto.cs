using AutoMapper;
using CubesFramework.Security;
using GLSPM.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GLSPM.Application.Dtos.Cards
{
    public class CardReadDto : CriticalEntityBase<int, string>
    {
        [Required]
        [StringLength(250, MinimumLength = 3)]
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

    public class CardToCardReadDtoMappingAction : IMappingAction<Card, CardReadDto>
    {
        private readonly Crypto _crypto;
        private readonly IConfiguration _configuration;
        private readonly string _encryptionCode;


        public CardToCardReadDtoMappingAction(Crypto crypto, IConfiguration configuration)
        {
            _crypto = crypto;
            _configuration = configuration;
            _encryptionCode = configuration.GetSection("EncryptionCode").Value;

        }
        public void Process(Card source, CardReadDto destination, ResolutionContext context)
        {
            destination.CardNumber =  _crypto.DecryptAes(source.EncriptedCardNumber, _encryptionCode).Result;
            destination.CVV = _crypto.DecryptAes(source.EncriptedCVV, _encryptionCode).Result;
        }
    }

    public class CardReadDtoToCardMappingAction : IMappingAction<CardReadDto,Card>
    {
        private readonly Crypto _crypto;
        private readonly IConfiguration _configuration;
        private readonly string _encryptionCode;


        public CardReadDtoToCardMappingAction(Crypto crypto, IConfiguration configuration)
        {
            _crypto = crypto;
            _configuration = configuration;
            _encryptionCode = configuration.GetSection("EncryptionCode").Value;

        }

        public void Process(CardReadDto source, Card destination, ResolutionContext context)
        {
            destination.EncriptedCardNumber = _crypto.DecryptAes(source.CardNumber, _encryptionCode).Result;
            destination.EncriptedCVV = _crypto.DecryptAes(source.CVV, _encryptionCode).Result;
        }
    }
}
