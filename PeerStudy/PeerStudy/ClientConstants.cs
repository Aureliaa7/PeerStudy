using Blazorise.Charts;
using System.Collections.Generic;

namespace PeerStudy
{
    public static class ClientConstants
    {
        public const string Token = "token";

        public const int ImageWidth = 300;
        public const int ImageHeight = 500;

        public const string AppName = "Peer Study";
        public const string Motto = "Learning together, growing together";


        public static class ChartColors
        {
            public static List<string> BackgroundColors = new List<string> 
            { 
                ChartColor.FromRgba(255, 99, 132, 0.2f), 
                ChartColor.FromRgba(54, 162, 235, 0.2f), 
                ChartColor.FromRgba(255, 206, 86, 0.2f), 
                ChartColor.FromRgba(75, 192, 192, 0.2f), 
                ChartColor.FromRgba(153, 102, 255, 0.2f),
                ChartColor.FromRgba(255, 159, 64, 0.2f) 
            };

            public static List<string> BorderColors = new List<string> 
            { 
                ChartColor.FromRgba(255, 99, 132, 1f), 
                ChartColor.FromRgba(54, 162, 235, 1f), 
                ChartColor.FromRgba(255, 206, 86, 1f),
                ChartColor.FromRgba(75, 192, 192, 1f), 
                ChartColor.FromRgba(153, 102, 255, 1f),
                ChartColor.FromRgba(255, 159, 64, 1f) 
            };
        }
    }
}
