using Blazored.Toast.Services;
using Blazorise;
using Blazorise.RichTextEdit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.QAndA.Questions;
using PeerStudy.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PeerStudy.Features.Q_A.Components.CreateQuestionComponent
{
    public partial class CreateQuestion
    {
        [Inject]
        private IQuestionService QuestionService { get; set; }

        [Inject]
        private IAuthService AuthService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IPeerStudyToastService ToastService { get; set; }


        [Parameter]
        public CreateQuestionModel CreateQuestionModel { get; set; } = new CreateQuestionModel();

        private Steps stepsRef;
        private string selectedStep = "1";
        private string newTag = string.Empty;

        // text editor
        protected RichTextEdit richTextEditRef;
        private bool showDescriptionInfoMessage = true;
        private const string descriptionInfoMessage = "Make sure you save the description before going to the next step...";

        private Guid currentUserId;

        protected override async Task OnInitializedAsync()
        {
            currentUserId = new Guid(await AuthService.GetCurrentUserIdAsync());
        }

        private bool NavigationAllowed(StepNavigationContext context)
        {
            if (context.CurrentStepIndex == 1 && context.NextStepIndex > 1 && ValidationRule.IsEmpty(CreateQuestionModel.Title))
            {
                return false;
            }

            else if (context.CurrentStepIndex == 2 && context.NextStepIndex > 2 && ValidationRule.IsEmpty(CreateQuestionModel.Description))
            {
                return false;
            }
            else if (context.CurrentStepIndex == 3 && context.NextStepIndex > 3 && !CreateQuestionModel.Tags.Any())
            {
                return false;
            }

            return true;
        }

        private void AddNewTag(EditContext editContext)
        {
            CreateQuestionModel.Tags.Add(newTag);
            newTag = string.Empty;
        }

        public async Task OnSave()
        {
            CreateQuestionModel.Description = await richTextEditRef.GetHtmlAsync();
        }

        private async void SaveQuestion()
        {
            try
            {
                CreateQuestionModel.AuthorId = currentUserId;
                var savedQuestion = await QuestionService.CreateAsync(CreateQuestionModel);
                NavigationManager.NavigateTo($"questions/{savedQuestion.Id}");
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, "An error occurred while saving the question...");
            }
        }

        private void DeleteTag(string tag)
        {
            CreateQuestionModel.Tags.Remove(tag);
        }
    }
}
