using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Pagination;
using PeerStudy.Core.Models.QAndA.Questions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class QuestionPaginationService : PaginationServiceBase<FlatQuestionModel, Question>, IQuestionPaginationService
    {
        private readonly IUnitOfWork unitOfWork;

        public QuestionPaginationService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public override async Task<PagedResponseModel<FlatQuestionModel>> GetPagedResponseAsync(PaginationFilter paginationFilter, Expression<Func<Question, bool>> filter = null)
        {
            int totalRecords = await unitOfWork.QuestionsRepository.GetTotalRecordsAsync(filter);

            var questions = (await unitOfWork.QuestionsRepository.GetAllAsync(filter))
                .OrderByDescending(x => x.CreatedAt)
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize)
                .Select(x => new FlatQuestionModel
                {
                    AuthorId = x.AuthorId,
                    AuthorName = $"{x.Author.FirstName}{x.Author.LastName}",
                    CreatedAt = x.CreatedAt,
                    Id = x.Id,
                    Title = x.Title,
                    Tags = x.QuestionTags.Select(x => x.Tag.Content).ToList(),
                    NoAnswers = x.Answers.Count()
                })
                .ToList();

            return GetPagedResponseModel(questions, totalRecords, paginationFilter);
        }
    }
}
