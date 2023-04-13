using Microsoft.AspNetCore.Components;
using PeerStudy.Models;
using PeerStudy.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace PeerStudy.Features.Q_A.Components.MainPageComponent
{
    public partial class QAndAMainPage : IDisposable
    {
        [Inject]
        private INavigationMenuService NavigationMenuService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }
  

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

        private void HandleAddQuestion()
        {
            NavigationManager.NavigateTo("/post-question");
        }

        public void Dispose() => NavigationMenuService.Reset();
    }
}
