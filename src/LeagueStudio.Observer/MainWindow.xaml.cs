namespace LeagueStudio.Observer;

using System.Windows;
using LeagueStudio.Observer.Services.Server;
using LeagueStudio.Observer.ViewModels;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var apiClient = new ObserverApiClient();

        DataContext = new MainViewModel(apiClient);
    }
}