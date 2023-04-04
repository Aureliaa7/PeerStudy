using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PeerStudy.Services.Interfaces;
using System;

namespace PeerStudy.Shared.MainLayoutComponent
{
    public partial class MainLayout
    {
        [Inject]
        INavigationMenuService NavigationMenuService { get; set; }

        private ErrorBoundary errorBoundary;
        private bool showErrorAlert = true;

        MatTheme matBlazorTheme = new MatTheme()
        {
            Primary = (new MatThemeColorShadow("blue_primary", "blue1", "#3B7197")).Value,
            Secondary = (new MatThemeColorShadow("blue_secondary", "blue2", "#74BDE0")).Value
        };

        // Note: We need to call the 'Recover' method so that the error message gets vanished when navigating to other pages
        protected override void OnParametersSet()
        {
            errorBoundary?.Recover();
        }

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
