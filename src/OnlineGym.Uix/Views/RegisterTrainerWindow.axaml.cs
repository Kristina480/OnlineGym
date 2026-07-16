using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnlineGym.Application.Database.Repositories;


namespace OnlineGym.Uix.Views;

public partial class RegisterTrainerWindow : Window
{
    public long? newTrainer;
    public bool registered = false;
    TrainerRepository repository = new TrainerRepository();
    
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
        var firstNameBox = this.FindControl<TextBox>("FirstNameBox");
        var lastNameBox = this.FindControl<TextBox>("LastNameBox");
        var usernameBox = this.FindControl<TextBox>("UsernameBox");
        var passwordBox = this.FindControl<TextBox>("PasswordBox");
        var specializationBox = this.FindControl<TextBox>("SpecializationBox");
        var educationBox = this.FindControl<TextBox>("EducationBox");
        var recommendationsBox = this.FindControl<TextBox>("RecommendationsBox");
        var errorText = this.FindControl<TextBlock>("ErrorText");
        
        string? first = firstNameBox?.Text;
        string? last = lastNameBox?.Text;
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
        
        if (repository.UsernameExists(username))
        {
            errorText.Text = "Korisnicko ime vec postoji.";
            return;
        }
        long accountId = repository.SaveUserAccount(username, password);
        long trainerId = repository.SaveTrainer(accountId, first, last, specialization, education, recommendations);
        repository.CreateRegistrationRequest(trainerId);
        
        newTrainer = trainerId;
        registered = true;
        errorText.Text ="Uspesna registracija! Ceka se odobrenje administratora.";
        //this.Close();
    }

    private void OnSaveLicenseClick(object? sender, RoutedEventArgs e)
    {
        var licenseNameBox = this.FindControl<TextBox>("LicenseNameBox");
        var documentTypeBox = this.FindControl<TextBox>("DocumentTypeBox");
        var issueDatePicker = this.FindControl<DatePicker>("IssueDatePicker");
        var errorText = this.FindControl<TextBlock>("ErrorText");
        
        string? licenseName = licenseNameBox?.Text;
        string? documentType = documentTypeBox?.Text;
        DateTimeOffset? issueDate = issueDatePicker?.SelectedDate;

        if (string.IsNullOrWhiteSpace(licenseName) || string.IsNullOrWhiteSpace(documentType) || issueDate == null)
        {
            errorText.Text = "Popunite sva polja.";
            return;
        }
        if (!registered || !newTrainer.HasValue)
        {
            errorText.Text = "Morate se prvo registrovati.";
            return;
        }
        repository.SaveLicense(newTrainer.Value, licenseName, documentType, issueDate.Value.DateTime);
        errorText.Text = "Licenca je dodata.";
        licenseNameBox.Text = string.Empty;
        documentTypeBox.Text = string.Empty;
        issueDatePicker.SelectedDate = null;
    }
    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        LoginWindow loginWindow = new LoginWindow();
        loginWindow.Show();
        this.Hide();
    }
}