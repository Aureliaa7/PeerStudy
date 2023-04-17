using System.Collections.Generic;

namespace PeerStudy.Core.Models.Pagination
{
    public class PagedResponseModel<T> where T : class, new()
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public int TotalRecords { get; set; }

        public List<T> Data { get; set; }
    }
}
