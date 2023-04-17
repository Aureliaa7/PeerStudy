using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public abstract class PaginationServiceBase<T, U> : IPaginationService<T, U> where T : class, new()
    {
        public abstract Task<PagedResponseModel<T>> GetPagedResponseAsync(
            PaginationFilter paginationFilter,
            Expression<Func<U, bool>> filter = null);

        public int GetRoundedTotalPages(int totalRecords, int pageSize)
        {
            var totalPages = ((double)totalRecords / pageSize);
            return Convert.ToInt32(Math.Ceiling(totalPages));
        }

        public PagedResponseModel<T> GetPagedResponseModel(List<T> data, int totalRecords, PaginationFilter paginationFilter)
        {
            int roundedTotalPages = GetRoundedTotalPages(totalRecords, paginationFilter.PageSize);
            return new PagedResponseModel<T>
            {
                Data = data,
                TotalPages = roundedTotalPages,
                PageNumber = paginationFilter.PageNumber,
                PageSize = paginationFilter.PageSize,
                TotalRecords = totalRecords
            };
        }
    }
}
