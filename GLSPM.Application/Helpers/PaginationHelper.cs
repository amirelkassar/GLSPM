using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.Helpers
{
    public class PaginationHelper
    {
        public static MultiObjectsResponse<T> CreatePagedReponse<T>(T pagedData, PaginationParametersBase validFilter, int totalRecords, IUriAppService uriService, string route)
        {
            //preparing the page data
            var respose = new MultiObjectsResponse<T>(pagedData, validFilter.PageNumber, validFilter.PageSize);
            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            //preparing the pagination navigation
            //next page
            respose.NextPage =
                validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                ? uriService.GetPageUri(new PaginationParametersBase(validFilter.PageNumber + 1, validFilter.PageSize), route)
                : null;
            //previous page
            respose.PreviousPage =
                validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                ? uriService.GetPageUri(new PaginationParametersBase(validFilter.PageNumber - 1, validFilter.PageSize), route)
                : null;
            //first page
            respose.FirstPage = uriService.GetPageUri(new PaginationParametersBase(1, validFilter.PageSize), route);
            //last page
            respose.LastPage = uriService.GetPageUri(new PaginationParametersBase(roundedTotalPages, validFilter.PageSize), route);
            respose.TotalPages = roundedTotalPages;
            respose.TotalRecords = totalRecords;
            return respose;
        }
    }
}
