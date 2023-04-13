using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.QAndA;
using System;
using System.Threading.Tasks;

namespace PeerStudy.Features.Q_A.Components.QuestionDetailsComponent
{
    public partial class QuestionDetails : PeerStudyComponentBase
    {
        [Inject]
        private IQuestionService QuestionService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }


        [Parameter]
        public Guid QuestionId { get; set; }


        private QuestionDetailsModel questionDetails;
        private bool showDeleteQuestionConfirmationPopup;
        private const string deleteQuestionConfirmationMessage = "Are you sure you want to delete this question?";

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            questionDetails = await QuestionService.GetAsync(QuestionId);
        }

        private void Edit()
        {

        }

        private void ShowDeletePopup()
        {
            showDeleteQuestionConfirmationPopup = true;
        }

        private async Task Delete()
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
    }
}
