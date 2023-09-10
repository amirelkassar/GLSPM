using AutoMapper;
using GLSPM.Application.Dtos.Cards;
using GLSPM.Application.Dtos.Passwords;
using GLSPM.Domain.Dtos.Passwords;
using GLSPM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Dtos
{
    public class GLSPMMappingProfile : Profile
    {
        public GLSPMMappingProfile()
        {
            RegisterCards();
            RegisterPasswords();
        }

        private void RegisterCards()
        {
            CreateMap<CardCreateDto, Card>()
                .BeforeMap<CardCreateDtoMappingAction>();
            CreateMap<Card, CardReadDto>()
                .BeforeMap<CardToCardReadDtoMappingAction>()
                .ReverseMap()
                .BeforeMap<CardReadDtoToCardMappingAction>();
        }

        private void RegisterPasswords()
        {
            CreateMap<Password, PasswordReadDto>()
                .AfterMap<PasswordToPasswordReadDtoMappingAction>()
                .ReverseMap()
                .BeforeMap<PasswordReadDtoToPasswordMappingAction>();
            CreateMap<PasswordCreateDto, Password>()
                .BeforeMap<PasswordCreateDtoToPasswordMappingAction>();
        }
    }
}
