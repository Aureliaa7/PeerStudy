using Blazorise.Charts;
using PeerStudy.Core.Enums;
using System.Collections.Generic;

namespace PeerStudy.Services.Interfaces
{
    public interface IChartDataService
    {
        ChartData<int> GetChartData(Dictionary<WorkItemStatus, int> workItemsStatusNoWorkItems);
    }
}
