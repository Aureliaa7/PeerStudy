using MatBlazor;
using Microsoft.AspNetCore.Components;
using PeerStudy.Services.Interfaces;
using System;

namespace PeerStudy.Shared
{
    public partial class MainLayout
    {
        [Inject]
        INavigationMenuService NavigationMenuService { get; set; }


        MatTheme matBlazorTheme = new MatTheme()
        {
            Primary = (new MatThemeColorShadow("blue_primary", "blue1", "#3B7197")).Value,
            Secondary = (new MatThemeColorShadow("blue_secondary", "blue2", "#74BDE0")).Value
        };

        protected override void OnInitialized()
        {
            NavigationMenuService.OnChanged += MenuChanged;
        }

        void Dispose()
        {
            NavigationMenuService.OnChanged -= MenuChanged;
        }

        private void MenuChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
    }
}
