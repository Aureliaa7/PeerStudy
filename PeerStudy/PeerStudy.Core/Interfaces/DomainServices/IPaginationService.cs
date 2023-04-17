using PeerStudy.Core.Models.Pagination;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface IPaginationService<T, U> where T : class, new()
    {
        Task<PagedResponseModel<T>> GetPagedResponseAsync(
            PaginationFilter paginationFilter,
            Expression<Func<U, bool>> filter = null);

        int GetRoundedTotalPages(int totalRecords, int pageSize);

        PagedResponseModel<T> GetPagedResponseModel(List<T> data, int totalRecords, PaginationFilter paginationFilter);
    }
}
