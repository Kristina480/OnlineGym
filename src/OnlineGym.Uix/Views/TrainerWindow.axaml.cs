using Avalonia.Controls;
using Avalonia.Interactivity;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;

namespace OnlineGym.Uix.Views;

public partial class TrainerWindow : Window
{
    private readonly TrainerRepository _trainerRepository = new();

    public Trainer? Trainer { get; private set; }

    public TrainerWindow(long accountId)
    {
        InitializeComponent();

        Trainer = _trainerRepository.GetTrainerByAccountId(accountId);
    }

    private void OnMachinesClick(
        object? sender,
        RoutedEventArgs e)
    {
        MachinesWindow machinesWindow = new();
        machinesWindow.ShowDialog(this);
    }

    private void OnLogoutClick(
        object? sender,
        RoutedEventArgs e)
    {
        Trainer = null;

        Close();

        LoginWindow loginWindow = new();
        loginWindow.Show();
    }
}