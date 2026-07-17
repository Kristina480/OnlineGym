using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnlineGym.Application.Database.Repositories;
namespace OnlineGym.Uix.Views;
public partial class RegisterClientWindow : Window
{
    public RegisterClientWindow()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnRegisterClick(object? sender, RoutedEventArgs e)
    {
        var firstNameBox = this.FindControl<TextBox>("FirstNameBox");
        var lastNameBox = this.FindControl<TextBox>("LastNameBox");
        var usernameBox = this.FindControl<TextBox>("UsernameBox");
        var passwordBox = this.FindControl<TextBox>("PasswordBox");
        var heightBox = this.FindControl<TextBox>("HeightBox");
        var weightBox = this.FindControl<TextBox>("WeightBox");
        var goalBox = this.FindControl<TextBox>("GoalBox");
        var healthIssuesBox = this.FindControl<TextBox>("HealthIssuesBox");
        var workoutsPerWeekBox = this.FindControl<TextBox>("WorkoutsPerWeekBox");
        var errorText = this.FindControl<TextBlock>("ErrorText");
        
        string? first = firstNameBox?.Text;
        string? last = lastNameBox?.Text;
        string? username = usernameBox?.Text;
        string? password = passwordBox?.Text;
        string? heightText = heightBox?.Text;
        string? weightText = weightBox?.Text;
        string? goal = goalBox?.Text;
        string? health = healthIssuesBox?.Text;
        string? workoutsText = workoutsPerWeekBox?.Text;
        
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last) ||
            string.IsNullOrWhiteSpace(heightText) || string.IsNullOrWhiteSpace(weightText) || string.IsNullOrWhiteSpace(workoutsText))
        {
            errorText.Text = "Popunite sva polja.";
            return;
        }
        
        if (!double.TryParse(heightText, out double height))
        {
            errorText.Text = "Visina mora biti broj.";
            return;
        }
        if (!double.TryParse(weightText, out double weight))
        {
            errorText.Text = "Tezina mora biti broj.";
            return;
        }
        if (!int.TryParse(workoutsText, out int workouts))
        {
            errorText.Text = "Broj treninga mora biti ceo broj.";
            return;
        }
        
        ClientRepository repository = new ClientRepository();
        if(repository.UsernameExists(username))
        {
            errorText.Text = "Korisnicko ime vec postoji.";
            return;
        }
        repository.RegisterClient(username, password, first, last, height, weight, goal, health, workouts);
        Console.WriteLine("Uspesna registracija!");
        LoginWindow loginWindow = new LoginWindow();
        loginWindow.Show();
        this.Hide();
    }

}