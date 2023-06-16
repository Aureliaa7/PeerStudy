using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PeerStudy.Core.Enums;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Models.Pagination;
using PeerStudy.Core.Models.QAndA.Questions;
using PeerStudy.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.QAndA.Components.MainPageComponent
{
    public partial class QAndAMainPage : PeerStudyComponentBase, IDisposable
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IQuestionService QuestionService { get; set; }


        private const int pageSize = 20;
        private const int firstPage = 1;
        private const int noPreviousNextPagesDisplayed = 2;
        private static List<FlatQuestionModel> emptyDataSource = new List<FlatQuestionModel>();

        private readonly Dictionary<int, List<FlatQuestionModel>> savedQuestions = new Dictionary<int, List<FlatQuestionModel>>();
        private List<FlatQuestionModel> currentDataSource = emptyDataSource;
        private int currentNoPages;
        private int noPagesForRecentQuestions;
        private int currentPage;

        private bool isSearchModeEnabled;
        private bool showSearchQuestionsInstructions;
        private string searchQuery = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await SetCurrentUserDataAsync();

            base.OnInitialized();

            if (currentUserRole == Role.Student)
            {
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
        }

        protected override async Task InitializeAsync()
        {
            var response = await QuestionService.GetAllAsync(currentUserId, new PaginationFilter(1, pageSize));
            SetCurrentData(response.Data, response.TotalPages);
            noPagesForRecentQuestions = response.TotalPages;
            currentPage = firstPage;
            savedQuestions.Add(firstPage, response.Data);
        }

        private void HandleAddQuestion()
        {
            NavigationManager.NavigateTo("/post-question");
        }

        private async Task SetDataForPageNumber(int page)
        {
            await SetQuestionsToBeDisplayed(page);
        }

        private async Task SetQuestionsToBeDisplayed(int pageNumber)
        {
            currentPage = pageNumber;
            isLoading = true;
            SetCurrentData(currentDataSource, 0);

            if (isSearchModeEnabled)
            {
                await SetSearchResultsAsync(pageNumber);
            }
            else
            {
                await SetQuestionsByCreationDateAsync(pageNumber);
            }

            isLoading = false;
        }

        private async Task SetSearchResultsAsync(int pageNumber)
        {
            try
            {
                var questionsPagedModel = await QuestionService.SearchAsync(searchQuery, new PaginationFilter(pageNumber, pageSize));
                SetCurrentData(questionsPagedModel.Data, questionsPagedModel.TotalPages);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastService.ShowToast(ToastLevel.Error, UIMessages.GetQuestionsErrorMessage);
            }
        }

        private async Task SetQuestionsByCreationDateAsync(int pageNumber)
        {
            if (savedQuestions.TryGetValue(pageNumber, out var foundQuestions))
            {
                currentDataSource = foundQuestions;
                currentNoPages = noPagesForRecentQuestions;
            }
            else
            {
                try
                {
                    PagedResponseModel<FlatQuestionModel>? response = await QuestionService.GetAllAsync(currentUserId, new PaginationFilter(pageNumber, pageSize));
                    SetCurrentData(response.Data, response.TotalPages);
                    noPagesForRecentQuestions = response.TotalPages;
                    savedQuestions.Add(pageNumber, currentDataSource);
                }
                catch (Exception ex)
                {
                    ToastService.ShowToast(ToastLevel.Error, ex.Message);
                }
            }
        }

        private async Task HandleKeyDownEvent(KeyboardEventArgs args)
        {
            isSearchModeEnabled = true;

            if (args.Code == "Enter")
            {
                showSearchQuestionsInstructions = false;
                StateHasChanged();

                try
                {
                    await SetDataForPageNumber(firstPage);
                }
                catch (Exception ex)
                {
                    ToastService.ShowToast(ToastLevel.Error, UIMessages.GenericErrorMessage);
                }
            }
        }

        private void SetSearchQuery(ChangeEventArgs args)
        {
            searchQuery = args.Value.ToString();
        }

        private async Task DisplayRecentQuestions()
        {
            isSearchModeEnabled = false;
            searchQuery = string.Empty;

            await SetQuestionsToBeDisplayed(firstPage);
        }

        private void SetCurrentData(List<FlatQuestionModel> questions, int totalPages)
        {
            currentDataSource = questions;
            currentNoPages = totalPages;
        }

        public void Dispose()
        {
            NavigationMenuService.Reset();
        }
    }
}
