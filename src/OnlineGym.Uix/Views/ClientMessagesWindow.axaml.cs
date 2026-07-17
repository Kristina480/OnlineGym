using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OnlineGym.Application.Domain;
using OnlineGym.Uix.ViewModels;

namespace OnlineGym.Uix.Views;

public partial class ClientMessagesWindow : Window
{
    private ClientMessagesViewModel? _viewModel;

    public ClientMessagesWindow()
    {
        InitializeComponent();
    }

    public ClientMessagesWindow(long accountId) : this()
    {
        _viewModel =
            new ClientMessagesViewModel(accountId);

        LoadMessages();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadMessages()
    {
        DataGrid grid =
            GetControl<DataGrid>("MessagesDataGrid");

        try
        {
            if (_viewModel is null)
            {
                throw new InvalidOperationException(
                    "Prozor nije pravilno inicijalizovan.");
            }

            List<Message> messages =
                _viewModel.GetMessages();

            _viewModel.MarkAllAsRead();

            foreach (Message message in messages)
            {
                message.IsRead = true;
            }

            grid.ItemsSource = messages;
            grid.SelectedItem = null;

            if (messages.Count == 0)
            {
                ShowInfo("Nemate primljenih poruka.");
            }
            else
            {
                ClearMessage();
            }
        }
        catch (Exception exception)
        {
            grid.ItemsSource = null;
            ShowError(exception);
        }
    }

    private void OnRefreshClick(
        object? sender,
        RoutedEventArgs e)
    {
        LoadMessages();
    }

    private void OnCloseClick(
        object? sender,
        RoutedEventArgs e)
    {
        Close();
    }

    private T GetControl<T>(string name)
        where T : Control
    {
        return this.FindControl<T>(name)
            ?? throw new InvalidOperationException(
                $"Kontrola '{name}' nije pronađena u XAML-u.");
    }

    private void ShowInfo(string text)
    {
        TextBlock message =
            GetControl<TextBlock>("MessageTextBlock");

        message.Text = text;
        message.Foreground = Brushes.DimGray;
    }

    private void ShowError(Exception exception)
    {
        TextBlock message =
            GetControl<TextBlock>("MessageTextBlock");

        if (exception is InvalidOperationException
            or ArgumentException)
        {
            message.Text = exception.Message;
        }
        else
        {
            Console.Error.WriteLine(exception);
            message.Text =
                "Došlo je do greške pri učitavanju poruka.";
        }

        message.Foreground = Brushes.DarkRed;
    }

    private void ClearMessage()
    {
        GetControl<TextBlock>(
            "MessageTextBlock").Text = string.Empty;
    }
}
