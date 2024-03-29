﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GLSPM.Domain.Dtos
{
    public class MultiObjectsResponse<TData> : SingleObjectResponse<TData>
    {
        public MultiObjectsResponse(TData data, int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
        }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Uri FirstPage { get; set; }
        public Uri LastPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }

    }
}
