using System.Windows;
using System.Windows.Controls;

namespace LeagueStudio.Observer.Views.Pages;

public partial class MatchSetupView : UserControl
{
    public MatchSetupView()
    {
        InitializeComponent();
    }

    private void MainScrollViewer_ScrollChanged(
        object sender,
        ScrollChangedEventArgs e)
    {
        if (ScrollHint is null)
        {
            return;
        }

        bool canScroll =
            MainScrollViewer.ScrollableHeight > 0;

        bool isAtBottom =
            MainScrollViewer.VerticalOffset
            >= MainScrollViewer.ScrollableHeight - 1;

        ScrollHint.Visibility =
            canScroll && !isAtBottom
                ? Visibility.Visible
                : Visibility.Collapsed;
    }
}