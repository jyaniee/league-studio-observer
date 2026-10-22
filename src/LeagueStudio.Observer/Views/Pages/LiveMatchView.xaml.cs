using System.Windows;
using System.Windows.Controls;
using LeagueStudio.Observer.ViewModels;

namespace LeagueStudio.Observer.Views.Pages;

public partial class LiveMatchView : UserControl
{
    public LiveMatchView()
    {
        InitializeComponent();
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