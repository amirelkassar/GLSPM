using AutoMapper;
using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.Dtos;
using GLSPM.Application.Dtos.Cards;
using GLSPM.Domain.Entities;
using GLSPM.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using CubesFramework.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using GLSPM.Domain;
using GLSPM.Domain.Dtos;
using GLSPM.Application.Helpers;

namespace GLSPM.Application.AppServices
{
    public class CardAppSerivce : AppServiceBase<Card, int, CardReadDto, CardCreateDto, CardUpdateDto>, ICardsAppService
    {
        private readonly Crypto _crypto;
        private readonly string _encryptionCode;
        public CardAppSerivce(IUnitOfWork unitOfWork,
            ILogger<AppServiceBase<Card, int, CardReadDto, CardCreateDto, CardUpdateDto>> logger,
            IRepository<Card, int> repository,
            IMapper mapper,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IOptions<FilesPathes> filesPathes,
            IUriAppService uriAppService,
            IHttpContextAccessor httpContextAccessor,
            Crypto crypto) : base(unitOfWork, logger, repository, mapper,configuration, environment, filesPathes, uriAppService, httpContextAccessor)
        {
            _crypto = crypto;
            _encryptionCode = configuration.GetSection("EncryptionCode").Value;
        }

        public async Task ChangeLogo(ChangeLogoDto<int> input)
        {
            var card = await Repository.GetAsync(input.Key);
            if (card != null)
            {
                if (!string.IsNullOrEmpty(card.LogoPath) && File.Exists(card.LogoPath))
                {
                    File.Delete(card.LogoPath);
                }
                var logoPath = Path.Combine(Path.GetFullPath(FilesPathes.LogosPath), $"{DateTime.Now.ToFileTime()}{Path.GetExtension(input.Logo.FileName)}");
                using (FileStream logoStream = new FileStream(logoPath, FileMode.Create))
                {
                    await input.Logo.CopyToAsync(logoStream);
                    await logoStream.FlushAsync();
                    card.LogoPath = logoPath;
                    await Repository.UpdateAsync(card);
                    await UnitOfWork.CommitAsync();
                }
            }
        }

        public async Task<MultiObjectsResponse<IEnumerable<CardReadDto>>> GetDeletedAsync(PaginationParametersBase pagination)
        {
            var dbset = await Repository.GetAsQueryableAsync();

            var data = await dbset.IgnoreQueryFilters()
                                  .Where(c => c.IsSoftDeleted)
                                  .ToArrayAsync();
            var results = Mapper.Map<IEnumerable<CardReadDto>>(data);
            var response = PaginationHelper.CreatePagedReponse(results, pagination, data.Count(), UriAppService, HttpContextAccessor.HttpContext.Request.Path.Value);
            response.Success = true;
            response.Message = "Items Found";
            response.StatusCode = StatusCodes.Status200OK;
            return response;
        }

        public async override Task<MultiObjectsResponse<IEnumerable<CardReadDto>>> GetListAsync(GetListDto input)
        {
            IEnumerable<Card> cards;
            int totalCount=0;
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                input.Filter = input.Filter.ToLower();
                var dbset = await Repository.GetAsQueryableAsync();
                cards = dbset.Where(c => c.Title.ToLower().Contains(input.Filter))
                             .OrderBy(input.Sorting)
                             .Skip(input.SkippedData)
                             .Take(input.PageSize);
                totalCount = await Repository.GetCountAsync(c => c.Title.ToLower().Contains(input.Filter));
            }
            else
            {
                cards =await Repository.GetAllAsync(input.Sorting, input.SkippedData, input.PageSize);
                totalCount = await Repository.GetCountAsync(filter: null);


            }
            var results = Mapper.Map<IEnumerable<CardReadDto>>(cards);
            var response = PaginationHelper.CreatePagedReponse(results, input, totalCount, UriAppService, HttpContextAccessor.HttpContext.Request.Path.Value);
            response.Success = true;
            response.Message = "Items Found";
            response.StatusCode = StatusCodes.Status200OK;
            return response;
        }

        public Task<string> GetLogoPathAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsDeleted(int key)
        {
            var dbset = await Repository.GetAsQueryableAsync();

            var card = await dbset.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.ID == key);

            return card != null ? card.IsSoftDeleted : true;
        }

        public async Task MarkAsDeletedAsync(int key)
        {
            if (!await IsDeleted(key))
            {
                var card = await Repository.GetAsync(key);
                card.DeleteDate = DateTime.Now;
                card.IsSoftDeleted = true;
                await UnitOfWork.CommitAsync();
            }
        }

        public async Task<SingleObjectResponse<CardReadDto>> UnMarkAsDeletedAsync(int key)
        {
            if (await IsDeleted(key))
            {
                var dbset = await Repository.GetAsQueryableAsync();
                var card = await dbset.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(c => c.ID == key);
                card.DeleteDate = DateTime.Now;
                card.IsSoftDeleted = false;
                await UnitOfWork.CommitAsync();
                var results= Mapper.Map<CardReadDto>(card);
                return new SingleObjectResponse<CardReadDto>
                {
                    Success=true,
                    StatusCode=StatusCodes.Status202Accepted,
                    Data=results,
                    Message="Item Was Restored"
                };
            }
            return new SingleObjectResponse<CardReadDto>
            {
                Success=false,
                StatusCode=StatusCodes.Status404NotFound,
                Message="Item Not Found",
                Error="Couldn't find an entity related to the passed id"
            };
        }

        public async override Task<SingleObjectResponse<CardReadDto>> UpdateAsync(int key, CardUpdateDto input)
        {
            var card = await Repository.GetAsync(key);
            if (card != null)
            {
                card.Title = input.Title;
                card.AdditionalInfo = input.AdditionalInfo;
                card.LastUpdateDate = DateTime.Now;
                //card props
                card.HolderName=input.HolderName;
                card.EncriptedCardNumber=await _crypto.EncryptAes(input.CardNumber, _encryptionCode);
                card.EncriptedCVV = await _crypto.EncryptAes(input.CVV, _encryptionCode);
                card.ExpiryMonth=input.ExpiryMonth;
                card.ExpiryYear=input.ExpiryYear;

                await Repository.UpdateAsync(card);
                await UnitOfWork.CommitAsync();
                var results= Mapper.Map<CardReadDto>(card);
                return new SingleObjectResponse<CardReadDto>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status202Accepted,
                    Data = results,
                    Message = "Item Updated"
                };
            }
            return new SingleObjectResponse<CardReadDto>
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Item Not Found",
                Error = "Couldn't find an entity related to the passed id"
            };
        }
    }
}
