using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Exceptions;
using PeerStudy.Core.Extensions;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Pagination;
using PeerStudy.Core.Models.QAndA.Answers;
using PeerStudy.Core.Models.QAndA.Questions;
using PeerStudy.Core.Models.QAndA.Votes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PeerStudy.Core.DomainServices
{
    public class QuestionService : VotingBaseService, IQuestionService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IQuestionPaginationService questionPaginationService;
        private readonly IRewardingService rewardingService;

        public QuestionService(IUnitOfWork unitOfWork,
            IQuestionPaginationService questionPaginationService,
            IRewardingService rewardingService)
        {
            this.unitOfWork = unitOfWork;
            this.questionPaginationService = questionPaginationService;
            this.rewardingService = rewardingService;
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

            if (await IsStudentAsync(createQuestionModel.AuthorId))
            {
                await rewardingService.UpdateBadgesForQuestionsAsync(createQuestionModel.AuthorId);
            }

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
                    AuthorName = $"{x.Author.FirstName} {x.Author.LastName}",
                    Answers = x.Answers.Select(y => new AnswerDetailsModel
                    {
                        Id = y.Id,
                        AuthorId = y.AuthorId,
                        AuthorName = $"{y.Author.FirstName} {y.Author.LastName}",
                        CreatedAt= y.CreatedAt,
                        HtmlContent = y.Content,
                        NoDownvotes = y.Votes.Where(x => x.VoteType == VoteType.Downvote).Count(),
                        NoUpvotes = y.Votes.Where(x => x.VoteType == VoteType.Upvote).Count(),
                        Votes = y.Votes
                                .Select(z => new VoteDetailsModel
                                {
                                    Id = z.Id,
                                    EntityId = z.AnswerId,
                                    UserId = z.AuthorId,
                                    VoteType = z.VoteType
                                })
                                .ToList()
                    })
                    .ToList(),
                    NoUpvotes = x.QuestionVotes.Count(x => x.VoteType == VoteType.Upvote),
                    NoDownvotes = x.QuestionVotes.Count(x => x.VoteType == VoteType.Downvote),
                    Votes = x.QuestionVotes.Select(x => new VoteDetailsModel
                    {
                        EntityId = x.QuestionId,
                        UserId = x.AuthorId,
                        VoteType = x.VoteType,
                        Id = x.Id
                    })
                    .ToList(),
                    AuthorProfileImageName = x.Author.ProfilePhotoName
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
                    NoAnswers = x.Answers.Count(),
                    AuthorName = $"{x.Author.FirstName} {x.Author.LastName}",
                    ProfilePhotoName = x.Author.ProfilePhotoName
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

        public async Task<PagedResponseModel<FlatQuestionModel>> SearchAsync(string searchQuery, PaginationFilter paginationFilter)
        {
            // [tag1] AND/OR [tag2] AND/OR "some text" 

            var filter = GetSearchFilter(searchQuery);

            var response = await questionPaginationService.GetPagedResponseAsync(paginationFilter, filter);
            return response;
        }

        public async Task VoteAsync(VoteModel voteModel)
        {
            var question = await unitOfWork.QuestionsRepository.GetByIdAsync(voteModel.EntityId)
                ?? throw new EntityNotFoundException($"Question with id {voteModel.EntityId} was not found!");

            await VoteEntityAsync(voteModel);

            if (await IsStudentAsync(question.AuthorId))
            {
                await rewardingService.UpdateBadgesForUpvotedQuestionAsync(voteModel.EntityId);
            }
        }

        private async Task<bool> IsStudentAsync(Guid userId)
        {
            var user = await unitOfWork.UsersRepository.GetByIdAsync(userId)
               ?? throw new EntityNotFoundException($"User with id {userId} was not found!");

            return user.Role == Role.Student;
        }

        protected override async Task<bool> DeleteVoteIfExistsAsync(Guid questionId, Guid authorId, VoteType voteType)
        {
            var vote = await GetVoteAsync(questionId, authorId, voteType);

            if (vote != null)
            {
                await unitOfWork.QuestionVotesRepository.RemoveAsync(vote);
                await unitOfWork.SaveChangesAsync();

                return true;
            }

            return false;
        }

        protected override async Task SaveVoteAsync(VoteModel voteModel)
        {
            await unitOfWork.QuestionVotesRepository.AddAsync(new QuestionVote
            {
                QuestionId = voteModel.EntityId,
                AuthorId = voteModel.UserId,
                VoteType = voteModel.VoteType
            });
            await unitOfWork.SaveChangesAsync();
        }

        private Task<QuestionVote> GetVoteAsync(Guid questionId, Guid authorId, VoteType voteType)
        {
            return unitOfWork.QuestionVotesRepository.GetFirstOrDefaultAsync(
                x => x.QuestionId == questionId &&
                x.AuthorId == authorId &&
                x.VoteType == voteType);
        }

        private static Expression<Func<Question, bool>> GetSearchFilter(string searchQuery)
        {
            Expression<Func<Question, bool>> filter = x => false;
            var wordsSplitByOr = searchQuery.Split("OR");

            var isSearchByTags = searchQuery.IndexOfAny(new char[] { '[', ']' }) != -1;
            var isSearchByTitleOrDescription = searchQuery.IndexOfAny(new char[] { '"' }) != -1;

            if (!isSearchByTags && !isSearchByTitleOrDescription)
            {
                var splitSearchQuery = searchQuery.Split(' ', StringSplitOptions.TrimEntries);

                return GetFilterForSimpleSearch(splitSearchQuery);
            }

            var operators = new List<string> { "AND", "OR" };
            bool isMixedSearch = isSearchByTags && isSearchByTitleOrDescription && operators.Any(x => searchQuery.Contains(x));

            if (isMixedSearch)
            {
                return GetFilterForMixedSearch(wordsSplitByOr);
            }

            if (isSearchByTags)
            {
                return GetTagsFilter(wordsSplitByOr);
            }

            if (isSearchByTitleOrDescription)
            {
                return GetDescriptionTitleFilter(wordsSplitByOr);
            }

            return filter;
        }

        private static Expression<Func<Question, bool>> GetFilterForSimpleSearch(string[] words)
        {
            Expression<Func<Question, bool>> filter = x => false;

            foreach (var word in words)
            {
                filter = filter.Or(x => x.Title.Contains(word) || x.Description.Contains(word));
            }

            return filter;
        }

        private static Expression<Func<Question, bool>> GetFilterForMixedSearch(string[] words)
        {
            Expression<Func<Question, bool>> filter = x => false;

            foreach (var word in words)
            {
                var wordsSplitByAnd = word.Trim().Split("AND", StringSplitOptions.TrimEntries);
                Expression<Func<Question, bool>> andFilter = x => true;

                andFilter = andFilter.And(GetTagsFilter(wordsSplitByAnd));
                andFilter = andFilter.And(GetDescriptionTitleFilter(wordsSplitByAnd));

                filter = filter.Or(andFilter);
            }

            return filter;
        }

        private static Expression<Func<Question, bool>> GetTagsFilter(string[] words)
        {
            Expression<Func<Question, bool>> filter = x => true;

            var tags = words
                .Where(x => x.StartsWith("[") && x.EndsWith("]"))
                .Select(x => x.Trim('[', ']').Trim())
                .ToList();

            if (!tags.Any())
            {
                return x => false;
            }

            foreach (var tag in tags)
            {
                filter = filter.And(x => x.QuestionTags.Any(t => t.Tag.Content == tag));
            }

            return filter;
        }

        private static Expression<Func<Question, bool>> GetDescriptionTitleFilter(string[] words)
        {
            Expression<Func<Question, bool>> filter = x => true;

            var wordsFromDescription = words
                    .Where(x => x.StartsWith("\"") && x.EndsWith("\""))
                    .Select(x => x.Trim('"').Trim())
                    .ToList();

            if (!wordsFromDescription.Any())
            {
                return x => false;
            }

            foreach (var descriptionWord in wordsFromDescription)
            {
                filter = filter.And(x => x.Title.Contains(descriptionWord) || x.Description.Contains(descriptionWord));
            }

            return filter;
        }
    }
}
