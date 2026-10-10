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
}