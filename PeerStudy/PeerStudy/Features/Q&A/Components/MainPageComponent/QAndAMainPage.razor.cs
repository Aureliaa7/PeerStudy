using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Pagination;
using PeerStudy.Core.Models.QAndA.Questions;
using PeerStudy.Models;
using PeerStudy.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.Q_A.Components.MainPageComponent
{
    public partial class QAndAMainPage : PeerStudyComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IQuestionService QuestionService { get; set; }


        private const int pageSize = 20;
        private const int noPreviousNextPagesDisplayed = 2;

        private readonly Dictionary<int, List<FlatQuestionModel>> savedQuestions = new Dictionary<int, List<FlatQuestionModel>>();
        private List<FlatQuestionModel> currentDataSource = new List<FlatQuestionModel>();
        private int noPages;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            NavigationMenuService.AddMenuItems(new List<MenuItem>
            {
                new MenuItem
                {
                    Href = "/my-questions",
                    Name = "My questions"
                }
            });
            NavigationMenuService.NotifyChanged();
        }

        protected override async Task InitializeAsync()
        {
            await SetCurrentUserDataAsync();
            var response = await QuestionService.GetAllAsync(currentUserId, new PaginationFilter(1, pageSize));
            
            currentDataSource = response.Data;
            noPages = response.TotalPages;
            savedQuestions.Add(1, response.Data);
        }

        private void HandleAddQuestion()
        {
            NavigationManager.NavigateTo("/post-question");
        }

        private async Task GoToPreviousPage(int page)
        {
            await SetQuestionsAsync(page);
        }

        private async Task GoToNextPage(int page)
        {
            await SetQuestionsAsync(page);
        }

        private async Task SetActivePage(int page)
        {
            await SetQuestionsAsync(page);
        }

        private async Task SetQuestionsAsync(int pageNumber)
        {
            if (savedQuestions.TryGetValue(pageNumber, out var foundQuestions))
            {
                currentDataSource = foundQuestions;
            }
            else
            {
                isLoading = true;
                try
                {
                    PagedResponseModel<FlatQuestionModel>? response = await QuestionService.GetAllAsync(currentUserId, new PaginationFilter(pageNumber, pageSize));
                    currentDataSource = response.Data;
                    savedQuestions.Add(pageNumber, currentDataSource);
                }
                catch (Exception ex)
                {
                    ToastService.ShowToast(ToastLevel.Error, ex.Message);
                }
                isLoading = false;
            }
        }

        protected override void Dispose(bool disposed)
        {
            base.Dispose(disposed);
            NavigationMenuService.Reset();
        }
    }
}
