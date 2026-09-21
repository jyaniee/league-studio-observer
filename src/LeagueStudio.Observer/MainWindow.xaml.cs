using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LeagueStudio.Observer.Services.Server;
using LeagueStudio.Observer.ViewModels;

namespace LeagueStudio.Observer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var apiClient = new ObserverApiClient();

        DataContext = new MainViewModel(apiClient);
    }

    private void Header_MouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void CloseButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        Close();
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