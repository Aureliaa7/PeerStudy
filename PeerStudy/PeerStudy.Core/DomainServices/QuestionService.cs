using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Pagination;
using PeerStudy.Core.Models.QAndA.Answers;
using PeerStudy.Core.Models.QAndA.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IQuestionPaginationService questionPaginationService;

        public QuestionService(IUnitOfWork unitOfWork, IQuestionPaginationService questionPaginationService)
        {
            this.unitOfWork = unitOfWork;
            this.questionPaginationService = questionPaginationService;
        }

        public async Task<QuestionDetailsModel> CreateAsync(CreateQuestionModel createQuestionModel)
        {
            var tagsIds = await GetTagsIdsAsync(createQuestionModel.Tags);

            var savedQuestion = await unitOfWork.QuestionsRepository.AddAsync(new Question
            {
                AuthorId = createQuestionModel.AuthorId,
                CreatedAt = DateTime.UtcNow,
                Description = createQuestionModel.Description,
                Title = createQuestionModel.Title
            });

            var questionTags = new List<QuestionTag>();
            foreach (var id in tagsIds)
            {
                questionTags.Add(new QuestionTag
                {
                    QuestionId = savedQuestion.Id,
                    TagId = id
                });
            }
            await unitOfWork.QuestionTagsRepository.AddRangeAsync(questionTags);
            await unitOfWork.SaveChangesAsync();

            return new QuestionDetailsModel
            {
                Id = savedQuestion.Id,
                AuthorId = createQuestionModel.AuthorId,
                CreatedAt = savedQuestion.CreatedAt,
                HtmlDescription = createQuestionModel.Description,
                Title = createQuestionModel.Title,
                Tags = createQuestionModel.Tags
            };
        }

        public async Task DeleteAsync(Guid id, Guid authorId)
        {
            bool questionExists = await unitOfWork.QuestionsRepository.ExistsAsync(x => x.Id == id && x.AuthorId == authorId);

            if (!questionExists)
            {
                throw new EntityNotFoundException($"Question with id {id} and author id {authorId} was not found!");
            }

            await unitOfWork.QuestionsRepository.RemoveAsync(id);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<QuestionDetailsModel> GetAsync(Guid id)
        {
            var questionDetailsModel = (await unitOfWork.QuestionsRepository.GetAllAsync(x => x.Id == id))
                .Select(x => new QuestionDetailsModel
                {
                    Id = x.Id,
                    AuthorId = x.AuthorId,
                    CreatedAt = x.CreatedAt,
                    HtmlDescription = x.Description,
                    Title = x.Title,
                    Tags = x.QuestionTags.Select(x => x.Tag.Content).ToList(),
                    Answers = x.Answers.Select(y => new AnswerDetailsModel
                    {
                        Id = y.Id,
                        AuthorId = y.AuthorId,
                        AuthorName = $"{y.Author.FirstName}{y.Author.LastName}",
                        CreatedAt= y.CreatedAt,
                        HtmlContent = y.Content,
                        NoDownvotes = y.NoDownvotes,
                        NoUpvotes = y.NoUpvotes
                    })
                    .ToList()
                })
                .FirstOrDefault();

            return questionDetailsModel ?? throw new EntityNotFoundException($"The question with id {id} was not found!");
        }

        public async Task<List<FlatQuestionModel>> GetFlatQuestionsAsync(Guid authorId)
        {
            var questionModels = (await unitOfWork.QuestionsRepository.GetAllAsync(x => x.AuthorId == authorId))
                .Select(x => new FlatQuestionModel
                {
                    Id = x.Id,
                    AuthorId = authorId,
                    CreatedAt= x.CreatedAt,
                    Tags = x.QuestionTags
                        .Select(x => x.Tag.Content)
                        .ToList(),
                    Title = x.Title,
                    NoAnswers = x.Answers.Count()
                })
                .ToList();

            return questionModels;
        }

        public async Task<PagedResponseModel<FlatQuestionModel>> GetAllAsync(Guid currentUserId, PaginationFilter paginationFilter)
        {
            Expression<Func<Question, bool>> filter = x => x.AuthorId != currentUserId;

            var response = await questionPaginationService.GetPagedResponseAsync(paginationFilter, filter);
            
            return response;
        }

        private async Task<List<Guid>> GetTagsIdsAsync(List<string> tags)
        {
            var foundTags = (await unitOfWork.TagsRepository.GetAllAsync(x => tags.Contains(x.Content)))
               .Select(x => new
               {
                   x.Id,
                   x.Content
               })
               .ToList();

            var tagsToBeCreated = tags
                .Except(foundTags
                    .Select(x => x.Content)
                    .ToList())
                .ToList();

            var newTags = new List<Tag>();
            foreach (var tag in tagsToBeCreated)
            {
                newTags.Add(new Tag { Content = tag });
            }

            var createdTags = await unitOfWork.TagsRepository.AddRangeAsync(newTags);
            await unitOfWork.SaveChangesAsync();

            return foundTags
                .Select(x => x.Id)
                .Union(createdTags
                    .Select(x => x.Id))
                .ToList();
        }

        public async Task UpdateAsync(UpdateQuestionModel updateQuestionModel)
        {
            var question = await unitOfWork.QuestionsRepository.GetFirstOrDefaultAsync(x => x.Id == updateQuestionModel.Id
             && x.AuthorId == updateQuestionModel.CurrentUserId) ?? 
             throw new EntityNotFoundException($"The question with id {updateQuestionModel.Id} and author id {updateQuestionModel.CurrentUserId} was not found!");
           
            question.Description = updateQuestionModel.Description;
            await unitOfWork.QuestionsRepository.UpdateAsync(question);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
