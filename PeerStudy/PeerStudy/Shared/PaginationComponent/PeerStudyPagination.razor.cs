using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace PeerStudy.Shared.PaginationComponent
{
    public partial class PeerStudyPagination
    {
        [Parameter]
        public int NoTotalPages { get; set; }

        [Parameter]
        public int NoPreviousNextPagesDisplayed { get; set; } = 2;

        [Parameter]
        public EventCallback<int> OnNavigateToPreviousPage { get; set; }

        [Parameter]
        public EventCallback<int> OnNavigateToNextPage { get; set; }

        [Parameter]
        public EventCallback<int> OnSetActivePage { get; set; }


        private const string previous = "previous";
        private const string next = "next";
        private string currentPage = "1";
        private int startPage;
        private int endPage;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            startPage = 1;
            endPage = NoTotalPages > NoPreviousNextPagesDisplayed + 1 ? NoPreviousNextPagesDisplayed + 1 : NoTotalPages;
        }

        private bool IsActive(string page) => currentPage == page;

        private bool IsPageNavigationDisabled(string navigation)
        {
            if (navigation.Equals(previous))
            {
                return currentPage.Equals("1");
            }
            else if (navigation.Equals(next))
            {
                return currentPage.Equals(NoTotalPages.ToString());
            }
            return false;
        }

        private async Task GoToPreviousPage()
        {
            var currentPageAsInt = int.Parse(currentPage);
            if (currentPageAsInt > 1)
            {
                currentPage = (currentPageAsInt - 1).ToString();
                UpdatePageIntervals(int.Parse(currentPage));
                await OnNavigateToPreviousPage.InvokeAsync(int.Parse(currentPage));
            }
        }

        private async Task GoToNextPage()
        {
            var currentPageAsInt = int.Parse(currentPage);
            if (currentPageAsInt < NoTotalPages)
            {
                currentPage = (currentPageAsInt + 1).ToString();
                UpdatePageIntervals(int.Parse(currentPage));
                await OnNavigateToNextPage.InvokeAsync(int.Parse(currentPage));
            }
        }

        private async Task SetActive(string page)
        {
            currentPage = page;
            var currentPageAsInt = int.Parse(currentPage);
            UpdatePageIntervals(currentPageAsInt);
            await OnSetActivePage.InvokeAsync(int.Parse(currentPage));
        }

        private void UpdatePageIntervals(int currentPage)
        {
            startPage = currentPage - NoPreviousNextPagesDisplayed >= 1 ? currentPage - NoPreviousNextPagesDisplayed : currentPage;
            endPage = currentPage + NoPreviousNextPagesDisplayed <= NoTotalPages ? currentPage + NoPreviousNextPagesDisplayed : currentPage;
        }
    }
}
