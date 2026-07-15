using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnlineGym.Application.Database.Repositories;

namespace OnlineGym.Uix.Views;

public partial class RegisterTrainerWindow : Window
{
    public RegisterTrainerWindow()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnRegisterClick(object? sender, RoutedEventArgs e)
    {
        var firstnameBox = this.FindControl<TextBox>("FirstnameBox");
        var lastnameBox = this.FindControl<TextBox>("LastnameBox");
        var usernameBox = this.FindControl<TextBox>("UsernameBox");
        var passwordBox = this.FindControl<TextBox>("PasswordBox");
        var specializationBox = this.FindControl<TextBox>("SpecializationBox");
        var educationBox = this.FindControl<TextBox>("EducationBox");
        var recommendationsBox = this.FindControl<TextBox>("RecommendationsBox");
        var errorText = this.FindControl<TextBlock>("ErrorText");
        
        string? first = firstnameBox?.Text;
        string? last = lastnameBox?.Text;
        string? username = usernameBox?.Text;
        string? password = passwordBox?.Text;
        string? specialization = specializationBox?.Text;
        string? education = educationBox?.Text;
        string? recommendations = recommendationsBox?.Text;
        
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last))
        {
            errorText.Text = "Popunite sva obavezna polja.";
            return;
        }
        
        TrainerRepository repository = new TrainerRepository();
        if (repository.UsernameExists(username))
        {
            errorText.Text = "Korisnicko ime vec postoji.";
            return;
        }
        repository.RegisterTrainer(username, password, first, last, specialization, education, recommendations);
        Console.WriteLine("Uspesna registracija!");
        this.Close();
    }
}