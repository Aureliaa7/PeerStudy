﻿using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.QAndA.Questions;
using System;
using System.Collections.Generic;

namespace PeerStudy.Features.Q_A.Components.QuestionsListComponent
{
    public partial class QuestionsList
    {
        [Inject]
        private NavigationManager NavigationManager {  get; set; }


        [Parameter]
        public List<FlatQuestionModel> Questions { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public string NoQuestionsMessage { get; set; } = "There are no questions yet...";

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private void HandleClickedQuestion(Guid questionId)
        {
            NavigationManager.NavigateTo($"/questions/{questionId}");
        }
    }
}
