using MatBlazor;

namespace PeerStudy.Shared
{
    public partial class MainLayout
    {
        MatTheme matBlazorTheme = new MatTheme()
        {
            Primary = (new MatThemeColorShadow("blue_primary", "blue1", "#3B7197")).Value,
            Secondary = (new MatThemeColorShadow("blue_secondary", "blue2", "#74BDE0")).Value
        };
    }
}
