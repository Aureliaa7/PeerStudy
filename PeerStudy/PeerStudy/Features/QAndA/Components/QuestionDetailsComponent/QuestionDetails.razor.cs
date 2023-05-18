using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.QAndA.Answers;
using PeerStudy.Core.Models.QAndA.Questions;
using PeerStudy.Core.Models.QAndA.Votes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.QAndA.Components.QuestionDetailsComponent
{
    public partial class QuestionDetails : PeerStudyComponentBase
    {
        [Inject]
        private IQuestionService QuestionService { get; set; }

        [Inject]
        private IAnswerService AnswerService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }


        [Parameter]
        public Guid QuestionId { get; set; }


        private QuestionDetailsModel questionDetails;
        private bool showDeleteQuestionConfirmationPopup;
        private bool showDeleteAnswerConfirmationPopup;

        private bool showAddAnswerEditor;
        private bool isEditQuestionDisabled = true;

        private Guid? answerId;

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            questionDetails = await QuestionService.GetAsync(QuestionId);
        }

        private void EnableEditQuestion()
        {
            isEditQuestionDisabled = false;
        }

        private async Task UpdateQuestion(string htmlContent)
        {
            isEditQuestionDisabled = true;

            try
            {
                await QuestionService.UpdateAsync(new UpdateQuestionModel
                {
                    Id = questionDetails.Id,
                    CurrentUserId = currentUserId,
                    Description = htmlContent
                });
                questionDetails.HtmlDescription = htmlContent;
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.UpdateQuestionErrorMessage);
            }
        }

        private void CancelUpdateQuestion()
        {
            isEditQuestionDisabled = true;
        }

        private void ShowDeleteQuestionPopup()
        {
            showDeleteQuestionConfirmationPopup = true;
        }

        private async Task DeleteQuestion()
        {
            showDeleteQuestionConfirmationPopup = false;

            try
            {
                await QuestionService.DeleteAsync(QuestionId, currentUserId);
                NavigationManager.NavigateTo("/my-questions");
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.DeleteQuestionErrorMessage);
            }
        }

        private async Task SaveAnswer(string htmlContent)
        {
            showAddAnswerEditor = false;
            try
            {
                var savedAnswer = await AnswerService.AddAsync(new AddAnswerModel
                {
                    AuthorId = currentUserId,
                    Content = htmlContent,
                    QuestionId = QuestionId
                });

                savedAnswer.AuthorName = userName;
                questionDetails.Answers.Add(savedAnswer);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.AddAnswerErrorMessage);
            }
        }

        private void CancelAddAnswer()
        {
            showAddAnswerEditor = false;
        }

        private void ShowDeleteAnswerPopup(Guid answerId)
        {
            this.answerId = answerId;
            showDeleteAnswerConfirmationPopup = true;
        }

        private async Task DeleteAnswer()
        {
            showDeleteAnswerConfirmationPopup = false;
            try
            {
                await AnswerService.DeleteAsync(answerId.Value, currentUserId);
                var deletedAnswer = questionDetails.Answers.First(x => x.Id == answerId);
                questionDetails.Answers.Remove(deletedAnswer);
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.DeleteAnswerErrorMessage);
            }

            answerId = null;
        }

        private void EnableEditAnswer(Guid answerId)
        {
            var answerToBeUpdated = questionDetails.Answers.First(x => x.Id == answerId);
            answerToBeUpdated.IsReadOnly = false;
        }

        private async Task SaveUpdatedAnswer((string htmlContent, Guid answerId) data)
        {
            try
            {
                await AnswerService.UpdateAsync(new UpdateAnswerModel
                {
                    Id = data.answerId,
                    AuthorId = currentUserId,
                    Content = data.htmlContent
                });
                var answerToBeUpdated = questionDetails.Answers.First(x => x.Id == data.answerId);
                answerToBeUpdated.HtmlContent = data.htmlContent;
                answerToBeUpdated.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.UpdateAnswerErrorMessage);
            }
        }

        private void DisableAnswerEditMode(Guid answerId)
        {
            var answerToBeUpdated = questionDetails.Answers.First(x => x.Id == answerId);
            answerToBeUpdated.IsReadOnly = true;
        }


        #region Upvote/downvote answer
        private async Task UpvoteAnswer(Guid answerId)
        {
            await VoteAnswerAsync(answerId, VoteType.Upvote);
        }

        private async Task DownvoteAnswer(Guid answerId)
        {
            await VoteAnswerAsync(answerId, VoteType.Downvote);
        }

        private async Task VoteAnswerAsync(Guid answerId,  VoteType voteType)
        {
            try
            {
                await AnswerService.VoteAsync(new VoteModel
                {
                    EntityId = answerId,
                    UserId = currentUserId,
                    VoteType = voteType
                });

                UpdateAnswerVotesInUI(answerId, voteType);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.UpdateAnswerErrorMessage);
            }
        }

        private void UpdateAnswerVotesInUI(Guid answerId, VoteType voteType)
        {
            var answer = questionDetails.Answers.First(x => x.Id == answerId);
            if (!RemoveAnswerVoteIfExists(answer, voteType))
            {
                if (voteType == VoteType.Upvote)
                {
                    RemoveAnswerVoteIfExists(answer, VoteType.Downvote);
                    answer.NoUpvotes += 1;
                }
                else
                {
                    RemoveAnswerVoteIfExists(answer, VoteType.Upvote);
                    answer.NoDownvotes += 1;
                }

                answer.Votes.Add(new VoteDetailsModel
                {
                    EntityId = answerId,
                    UserId = currentUserId,
                    VoteType = voteType
                });
            }
        }

        private bool RemoveAnswerVoteIfExists(AnswerDetailsModel answer, VoteType voteType)
        {
            var vote = answer.Votes.FirstOrDefault(x => x.EntityId == answer.Id &&
                x.UserId == currentUserId &&
                x.VoteType == voteType);
            if (vote != null)
            {
                answer.Votes.Remove(vote);
                if(voteType == VoteType.Downvote)
                {
                    answer.NoDownvotes -= 1;
                }
                else
                {
                    answer.NoUpvotes -= 1;
                }
                return true;
            }

            return false;
        }
        #endregion


        #region Upvote/Downvote question
        private async Task UpvoteQuestion()
        {
            await VoteQuestionAsync(VoteType.Upvote);
        }

        private async Task DownvoteQuestion()
        {
            await VoteQuestionAsync(VoteType.Downvote);
        }

        private async Task VoteQuestionAsync(VoteType voteType)
        {
            try
            {
                await QuestionService.VoteAsync(new VoteModel
                {
                    EntityId = QuestionId,
                    UserId = currentUserId,
                    VoteType = voteType
                });

                UpdateQuestionVotesInUI(voteType);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.UpdateQuestionErrorMessage);
            }
        }

        private void UpdateQuestionVotesInUI(VoteType voteType)
        {
            if (!RemoveQuestionVoteIfExists(voteType))
            {
                if (voteType == VoteType.Upvote)
                {
                    RemoveQuestionVoteIfExists(VoteType.Downvote);
                    questionDetails.NoUpvotes += 1;
                }
                else
                {
                    RemoveQuestionVoteIfExists(VoteType.Upvote);
                    questionDetails.NoDownvotes += 1;
                }

                questionDetails.Votes.Add(new VoteDetailsModel
                {
                    EntityId = QuestionId,
                    UserId = currentUserId,
                    VoteType = voteType
                });
            }
        }

        private bool RemoveQuestionVoteIfExists(VoteType voteType)
        {
            var vote = questionDetails.Votes.FirstOrDefault(x =>
                x.UserId == currentUserId &&
                x.VoteType == voteType);
            if (vote != null)
            {
                questionDetails.Votes.Remove(vote);
                if (voteType == VoteType.Downvote)
                {
                    questionDetails.NoDownvotes -= 1;
                }
                else
                {
                    questionDetails.NoUpvotes -= 1;
                }
                return true;
            }

            return false;
        }

        #endregion
    }
}
