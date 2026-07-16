using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace OnlineGym.Uix.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void OnLoginClick(object? sender, RoutedEventArgs e)
    {
        LoginWindow loginWindow = new LoginWindow();
        loginWindow.Show();
        this.Hide();
    }

    private void OnRegisterClientClick(object? sender, RoutedEventArgs e)
    {
        RegisterClientWindow registerWindow = new RegisterClientWindow();
        registerWindow.Show();
        this.Hide();
    }
    private void OnRegisterTrainerClick(object? sender, RoutedEventArgs e)
    {
        RegisterTrainerWindow registerWindow = new RegisterTrainerWindow();
        registerWindow.Show();
        this.Hide();
    }
}