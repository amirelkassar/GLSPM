using AutoMapper;
using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.Dtos;
using GLSPM.Application.Helpers;
using GLSPM.Domain;
using GLSPM.Domain.Dtos;
using GLSPM.Domain.Entities;
using GLSPM.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices
{
    public class AppServiceBase<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto> : IAppService<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto>
    {
        public AppServiceBase(IUnitOfWork unitOfWork,
            ILogger<AppServiceBase<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto>> logger,
            IRepository<TEntity, TKey> repository,
            IMapper mapper,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IOptions<FilesPathes> filesPathes,
            IUriAppService uriAppService,
            IHttpContextAccessor httpContextAccessor)
        {
            UnitOfWork = unitOfWork;
            Logger = logger;
            Repository = repository;
            Mapper = mapper;
            Configuration = configuration;
            Environment = environment;
            UriAppService = uriAppService;
            HttpContextAccessor = httpContextAccessor;
            FilesPathes = filesPathes.Value;
        }
        public IUnitOfWork UnitOfWork { get; }

        public IRepository<TEntity, TKey> Repository { get; }
        public IMapper Mapper { get; }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        public IUriAppService UriAppService { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public FilesPathes FilesPathes { get; }
        public ILogger Logger { get; }

        public virtual async Task<SingleObjectResponse<TReadDto>> CreateAsync(TCreateDto input)
        {
            var data = Mapper.Map<TEntity>(input);
            data = await Repository.InsertAsync(data);
            await UnitOfWork.CommitAsync();
            var results = Mapper.Map<TReadDto>(data);
            return new SingleObjectResponse<TReadDto>
            {
                Success = true,
                Message = "Item Created",
                Data = results,
                StatusCode = StatusCodes.Status201Created
            };
        }

        public virtual async Task DeleteAsync(TKey key)
        {
            await Repository.DeleteAsync(key);
            await UnitOfWork.CommitAsync();
        }

        public virtual async Task<SingleObjectResponse<TReadDto>> GetAsync(TKey key)
        {
            var data = await Repository.GetAsync(key);
            if (data != null)
            {
                var results = Mapper.Map<TReadDto>(data);
                return new SingleObjectResponse<TReadDto>
                {
                    Success = true,
                    Message = "Item Found",
                    Data = results,
                    StatusCode = StatusCodes.Status200OK
                };
            }
            else
            {
                return new SingleObjectResponse<TReadDto>
                {
                    Success = false,
                    Message = "Item Not Found",
                    StatusCode = StatusCodes.Status404NotFound,
                    Error = "Couldn't find an entity realted to the passed id"
                }; ;
            }

        }

        public virtual async Task<MultiObjectsResponse<IEnumerable<TReadDto>>> GetListAsync(GetListDto input)
        {
            IEnumerable<TEntity> data;
            int totalCount = 0;
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                data = await Repository.GetAllAsync(filter: input.Filter, input.Sorting, skipCound: input.SkippedData, input.PageSize);
                totalCount = await Repository.GetCountAsync(filter: input.Filter);
            }
            else
            {
                data = await Repository.GetAllAsync(sorting: input.Sorting, skipCound: input.SkippedData, input.PageSize);
                totalCount = await Repository.GetCountAsync(filter: null);
            }
            var results = Mapper.Map<IEnumerable<TReadDto>>(data);
            var response = PaginationHelper.CreatePagedReponse(results, input, totalCount, UriAppService, HttpContextAccessor.HttpContext.Request.Path.Value);
            response.Success = true;
            response.Message = "Items Found";
            response.StatusCode = StatusCodes.Status200OK;
            return response;
        }

        public virtual async Task<SingleObjectResponse<TReadDto>> UpdateAsync(TKey key, TUpdateDto input)
        {
            if (await Repository.GetAsync(key) != null)
            {
                var data = Mapper.Map<TEntity>(input);
                await Repository.UpdateAsync(data);
                await UnitOfWork.CommitAsync();
                var results = Mapper.Map<TReadDto>(data);
                return new SingleObjectResponse<TReadDto>
                {
                    Success = true,
                    Message = "Item Updated",
                    Data = results,
                    StatusCode = StatusCodes.Status202Accepted
                };
            }
            return new SingleObjectResponse<TReadDto>
            {
                Success = false,
                Message = "Item Not Found",
                StatusCode = StatusCodes.Status404NotFound,
                Error = "Couldn't find an entity realted to the passed id"
            }; ;
        }
    }
}
