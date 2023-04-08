using Blazorise.Charts;
using PeerStudy.Core.Enums;
using PeerStudy.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace PeerStudy.Services
{
    public class ChartDataService : IChartDataService
    {
        public ChartData<int> GetChartData(Dictionary<WorkItemStatus, int> workItemsStatusNoWorkItems)
        {
            var chartData = new ChartData<int>
            {
                Datasets = new List<ChartDataset<int>>
                {
                    new ChartDataset<int>
                    {
                        Data = workItemsStatusNoWorkItems
                                .OrderBy(x => x.Key)
                                .Select(x => x.Value)
                                .ToList(),
                        BackgroundColor = new List<string> { "rgba(75, 192, 192, 0.6)" , "rgba(54, 162, 235, 0.6)" , "rgba(255, 99, 132, 0.6)" }
                    }
                },
                Labels = workItemsStatusNoWorkItems.OrderBy(x => x.Key).Select(x => (object)x.Key.ToString()).ToList()
            };

            return chartData;
        }
    }
}
