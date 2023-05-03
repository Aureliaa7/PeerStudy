using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.QAndA.Questions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.QAndA.Components.QuestionsComponent
{
    public partial class Questions : PeerStudyComponentBase
    {
        [Inject]
        private IQuestionService QuestionService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }


        private List<FlatQuestionModel> questionsModels = new List<FlatQuestionModel>();
        private const string noQuestionsMessage = "There are no questions yet...";

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            questionsModels = await QuestionService.GetFlatQuestionsAsync(currentUserId);
        }

        private void AddQuestion()
        {
            NavigationManager.NavigateTo("/post-question");
        }
    }
}
