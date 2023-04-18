using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Pagination;
using PeerStudy.Core.Models.QAndA.Questions;
using PeerStudy.Models;
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
        private const string noQuestionsMessage = "No questions were found...";
        private static List<FlatQuestionModel> emptyDataSource = new List<FlatQuestionModel>();

        private readonly Dictionary<int, List<FlatQuestionModel>> savedQuestions = new Dictionary<int, List<FlatQuestionModel>>();
        private List<FlatQuestionModel> currentDataSource = emptyDataSource;
        private int noPages;

        private bool isSearchModeEnabled;
        private bool isSearchFieldDisabled;
        private bool showSearchQuestionsInstructions;
        private string searchQuery = string.Empty;

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

        private async Task SetDataForPageNumber(int page)
        {
            await SetQuestionsAsync(page);
        }

        private async Task SetQuestionsAsync(int pageNumber)
        {
            if (isSearchModeEnabled)
            {
                isLoading = true;
                try
                {
                    var questionsPagedModel = await QuestionService.SearchAsync(searchQuery, new PaginationFilter(pageNumber, pageSize));

                    currentDataSource = questionsPagedModel.Data;
                }
                catch (Exception ex)
                {
                    ToastService.ShowToast(ToastLevel.Error, "An error occurred while fetching the questions...");
                }
                isLoading = false;
            }
            else
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
        }

        //search questions
        private async Task HandleKeyDownEvent(KeyboardEventArgs args)
        {
            isSearchModeEnabled = true;

            if (args.Code == "Enter")
            {
                isSearchFieldDisabled = true;
                showSearchQuestionsInstructions = false;
                isLoading = true;
                currentDataSource = emptyDataSource;
                StateHasChanged();

                try
                {
                    var pagedQuestionsModel = await QuestionService.SearchAsync(searchQuery, new PaginationFilter());
                    currentDataSource = pagedQuestionsModel.Data;

                }
                catch (Exception ex) 
                { 
                }
                isSearchFieldDisabled = false;
                isLoading = false;
            }
        }

        private void SetSearchQuery(ChangeEventArgs args)
        {
            searchQuery = args.Value.ToString();
        }

        private async void DisplayRecentQuestions()
        {
            isSearchModeEnabled = false;
            currentDataSource = emptyDataSource;
            searchQuery = string.Empty;

            await SetQuestionsAsync(1);
        }

        protected override void Dispose(bool disposed)
        {
            base.Dispose(disposed);
            NavigationMenuService.Reset();
        }
    }
}
