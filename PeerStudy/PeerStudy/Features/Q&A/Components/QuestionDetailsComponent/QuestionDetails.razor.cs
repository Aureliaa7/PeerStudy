using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.QAndA.Answers;
using PeerStudy.Core.Models.QAndA.Questions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Q_A.Components.QuestionDetailsComponent
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
        private const string deleteQuestionConfirmationMessage = "Are you sure you want to delete this question?";
        private const string deleteAnswerConfirmationMessage = "Are you sure you want to delete this answer?";

        private bool showAddCommentEditor;
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
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while updating the question...");
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
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while deleting the question...");
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
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while posting your answer...");
            }
        }

        private void SaveComment(string htmlContent)
        {
            showAddCommentEditor = false;
            //TODO: save the comment
        }

        private void CancelAddComment()
        {
            showAddCommentEditor = false;
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
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while deleting the answer...");
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
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while updating the answer...");
            }
        }

        private void DisableAnswerEditMode(Guid answerId)
        {
            var answerToBeUpdated = questionDetails.Answers.First(x => x.Id == answerId);
            answerToBeUpdated.IsReadOnly = true;
        }

        private async Task UpvoteAnswer(Guid answerId)
        {
            await VoteAsync(answerId, VoteType.Upvote);
        }

        private async Task DownvoteAnswer(Guid answerId)
        {
            await VoteAsync(answerId, VoteType.Downvote);
        }

        private async Task VoteAsync(Guid answerId,  VoteType voteType)
        {
            try
            {
                await AnswerService.VoteAsync(new VoteAnswerModel
                {
                    AnswerId = answerId,
                    UserId = currentUserId,
                    VoteType = voteType
                });

                UpdateVotesInUI(answerId, voteType);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while updating the answer...");
            }
        }

        private void UpdateVotesInUI(Guid answerId, VoteType voteType)
        {
            var answer = questionDetails.Answers.First(x => x.Id == answerId);
            if (!RemoveVoteIfExists(answer, voteType))
            {
                if (voteType == VoteType.Upvote)
                {
                    RemoveVoteIfExists(answer, VoteType.Downvote);
                    answer.NoUpvotes += 1;
                }
                else
                {
                    RemoveVoteIfExists(answer, VoteType.Upvote);
                    answer.NoDownvotes += 1;
                }

                answer.Votes.Add(new VoteAnswerDetailsModel
                {
                    AnswerId = answerId,
                    UserId = currentUserId,
                    VoteType = voteType
                });
            }
        }

        private bool RemoveVoteIfExists(AnswerDetailsModel answer, VoteType voteType)
        {
            var vote = answer.Votes.FirstOrDefault(x => x.AnswerId == answer.Id &&
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
    }
}
