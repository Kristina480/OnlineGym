using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;
namespace OnlineGym.Uix.Views;
public partial class LoginWindow : Window
{
    public long? accountId;
    public LoginWindow()
    {
        InitializeComponent();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnLoginClick(object? sender, RoutedEventArgs e)
    {

        var usernameBox = this.FindControl<TextBox>("UsernameBox");
        var passwordBox = this.FindControl<TextBox>("PasswordBox");
        var errorText = this.FindControl<TextBlock>("ErrorText");

        string? username = usernameBox?.Text;
        string? password = passwordBox?.Text;
        
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            errorText.Text = "Popunite sva polja.";
            return;
        }

        UserAccountRepository repo = new UserAccountRepository();
        accountId = repo.GetIdByCredentials(username, password);
        if(accountId == null)
        {
            errorText.Text = "Pogresan username ili lozinka.";
            return;
        }
        
        string? userType = repo.GetUserTypeById(accountId.Value);
        if(userType == "TRAINER")
        {
            TrainerRepository trainerRepo = new TrainerRepository();
            Trainer? trainer=trainerRepo.GetTrainerByAccountId(accountId.Value);
            string? status=trainerRepo.GetRegistrationStatus(trainer.TrainerId);
            if (status == "APPROVED")
            {
                TrainerWindow trainerWindow = new TrainerWindow(accountId.Value);
                trainerWindow.Show();
                this.Hide();
            }
            else
            {
                errorText.Text = "Nije vam odobren zahtev za registraciju.";
            }
        }
        else if(userType == "CLIENT")
        {
            ClientWindow clientWindow = new ClientWindow(accountId.Value);
            clientWindow.Show();
            this.Hide();
        }
        else if(userType == "ADMIN")
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();
            this.Hide();
        }
    }

}