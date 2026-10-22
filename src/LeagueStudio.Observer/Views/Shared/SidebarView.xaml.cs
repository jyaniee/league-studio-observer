
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LeagueStudio.Observer.Views.Shared;

public partial class SidebarView : UserControl
{
    public SidebarView()
    {
        InitializeComponent();
    }

    private void NavigationButton_PreviewMouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        foreach (var child in NavigationMenu.Children)
        {
            if (child is not Button button)
            {
                continue;
            }

            button.Foreground =
                (Brush)FindResource("TextSecondaryBrush");

            button.Background = Brushes.Transparent;
        }

        var selectedButton = (Button)sender;

        selectedButton.Foreground =
            (Brush)FindResource("PrimaryBrush");

        selectedButton.Background =
            (Brush)FindResource("PrimaryMutedBrush");
    }
}
