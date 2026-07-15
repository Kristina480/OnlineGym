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

    private void OnRegisterClick(object? sender, RoutedEventArgs e)
    {
        //RegisterWindow registerForm = new RegisterWindow();
        //registerForm.Show();
        this.Hide();
    }
}