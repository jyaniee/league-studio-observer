
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeagueStudio.Observer.Views.Shared;

public partial class HeaderView : UserControl
{
    public HeaderView()
    {
        InitializeComponent();
    }

    private void Header_MouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            Window.GetWindow(this)?.DragMove();
        }
    }

    private void CloseButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        Window.GetWindow(this)?.Close();
    }
}
