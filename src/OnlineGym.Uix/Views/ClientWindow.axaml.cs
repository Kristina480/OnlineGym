using System;
using System.Xml.Schema;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;
namespace OnlineGym.Uix.Views;

public partial class ClientWindow : Window
{
    ClientRepository clientRepo=new ClientRepository();
    public Client client;
    public ClientWindow(long accountId)
    {
        InitializeComponent();
        client=clientRepo.GetClientByAccountId(accountId);
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnUpdateProfileClick(object? sender, RoutedEventArgs e)
    {
        if (client != null)
        {
            var updateClientWindow = new UpdateClientWindow(client);
            updateClientWindow.Show();
        }
    }

    private void OnLogoutClick(object? sender, RoutedEventArgs e)
    {
        client = null;
        this.Close();
        var mainWindow = new MainWindow();
        mainWindow.Show();
        // var loginWindow = new LoginWindow();
        // loginWindow.Show();
    }
    
    private void OnBrowseTrainersClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (client is null)
            return;

        TrainerBrowserWindow window =
            new TrainerBrowserWindow(client.ClientId);

        Hide();

        window.Closed += (_, _) =>
        {
            Show();
            Activate();
        };

        window.Show();
    }
    private void OnMessagesClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (client is null)
            return;

        ClientMessagesWindow window =
            new ClientMessagesWindow(client.AccountId);

        Hide();

        window.Closed += (_, _) =>
        {
            Show();
            Activate();
        };

        window.Show();
    }
}