using System;
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
        var loginWindow = new LoginWindow();
        loginWindow.Show();
    }
}