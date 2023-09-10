using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Domain.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.AppServices
{
    public class UriAppService : IUriAppService
    {
        private readonly string _baseUri;
        public UriAppService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetFileLink(string filepath) => new Uri(string.Concat(_baseUri, "/", filepath));

        public Uri GetPageUri(PaginationParametersBase filter, string route)
        {
            var _enpointUri = new Uri(string.Concat(_baseUri, route));
            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "pageNumber", filter.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());
            return new Uri(modifiedUri);
        }
    }
}
