using System;
using System.Collections.Generic;
using System.Text;

namespace GLSPM.Domain.Dtos
{
    public class PaginationParametersBase
    {
        public PaginationParametersBase()
        {

        }
        public PaginationParametersBase(int pagenumber, int pagesize)
        {
            PageNumber = pagenumber;
            PageSize = pagesize;
        }
        private const int MaxPageSize = 100;
        private const int MiniPageNumber = 1;
        private int pagesize = MaxPageSize;

        protected int TotalRecords { get; set; }
        /// <summary>
        /// Returned page size . Max size(100)
        /// </summary>
        public int PageSize
        {
            get { return pagesize; }
            set { pagesize = value <= MaxPageSize ? value : MaxPageSize; }
        }

        private int pagenumber = MiniPageNumber;
        /// <summary>
        /// Needed page number . Min page(1)
        /// </summary>
        public int PageNumber
        {
            get { return pagenumber; }
            set { pagenumber = value >= MiniPageNumber ? value : MiniPageNumber; }
        }
        private int _skippeddata;
        public int SkippedData
        {
            get => _skippeddata >= 1 ? _skippeddata : (PageNumber - 1) * PageSize;
            set => _skippeddata = value;
        }
    }
}
