namespace PeerStudy.Core.Models.Pagination
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }


        public PaginationFilter()
        {
            PageNumber = Constants.DefaultPageNumber;
            PageSize = Constants.DefaultPageSize;
        }

        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < Constants.DefaultPageNumber ? Constants.DefaultPageNumber : pageNumber;
            PageSize = pageSize > Constants.DefaultPageSize ? Constants.DefaultPageSize : pageSize;
        }
    }
}
