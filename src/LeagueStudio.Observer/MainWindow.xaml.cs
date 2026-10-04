using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LeagueStudio.Observer.Services.Server;
using LeagueStudio.Observer.ViewModels;
using System.Windows.Media;

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
    private async void ResetMatchButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "현재 경기 정보와 Observer Override를 모두 초기화합니다.\n" +
            "이 작업은 되돌릴 수 없습니다.\n\n" +
            "계속하시겠습니까?",
            "Reset Match",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        if (sender is Button button &&
            button.DataContext is LiveMatchViewModel viewModel &&
            viewModel.ResetMatchCommand.CanExecute(null))
        {
            await viewModel.ResetMatchCommand.ExecuteAsync(null);
        }
    }
}