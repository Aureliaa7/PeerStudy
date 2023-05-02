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
        public int CurrentPage { get; set; } = 1;

        [Parameter]
        public EventCallback<int> OnSetActivePage { get; set; }


        private const string previous = "previous";
        private const string next = "next";
        private int startPage;
        private int endPage;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            startPage = 1;
            endPage = NoTotalPages > NoPreviousNextPagesDisplayed + 1 ? NoPreviousNextPagesDisplayed + 1 : NoTotalPages;
        }

        private bool IsActive(int page) => CurrentPage == page;

        private bool IsPageNavigationDisabled(string navigation)
        {
            if (navigation.Equals(previous))
            {
                return CurrentPage.Equals("1");
            }
            else if (navigation.Equals(next))
            {
                return CurrentPage.Equals(NoTotalPages.ToString());
            }
            return false;
        }

        private async Task GoToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                UpdatePageIntervals(CurrentPage - 1);
                await OnSetActivePage.InvokeAsync(CurrentPage - 1);
            }
        }

        private async Task GoToNextPage()
        {
            if (CurrentPage < NoTotalPages)
            {
                UpdatePageIntervals(CurrentPage + 1);
                await OnSetActivePage.InvokeAsync(CurrentPage + 1);
            }
        }

        private async Task SetActive(string page)
        {
            CurrentPage = int.Parse(page);
            UpdatePageIntervals(CurrentPage);
            await OnSetActivePage.InvokeAsync(CurrentPage);
        }

        private void UpdatePageIntervals(int currentPage)
        {
            startPage = currentPage - NoPreviousNextPagesDisplayed >= 1 ? currentPage - NoPreviousNextPagesDisplayed : currentPage;
            endPage = currentPage + NoPreviousNextPagesDisplayed <= NoTotalPages ? currentPage + NoPreviousNextPagesDisplayed : currentPage;
        }
    }
}
