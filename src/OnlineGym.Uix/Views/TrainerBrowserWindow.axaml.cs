using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OnlineGym.Application.Domain;
using OnlineGym.Uix.ViewModels;

namespace OnlineGym.Uix.Views;

public partial class TrainerBrowserWindow : Window
{
    private TrainerBrowserViewModel? _viewModel;

    public TrainerBrowserWindow()
    {
        InitializeComponent();
    }

    public TrainerBrowserWindow(long clientId) : this()
    {
        _viewModel = new TrainerBrowserViewModel(clientId);
        LoadTrainers();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadTrainers()
    {
        DataGrid grid = GetControl<DataGrid>("TrainersDataGrid");

        try
        {
            List<Trainer> trainers =
                _viewModel?.GetAvailableTrainers() ?? new List<Trainer>();

            grid.ItemsSource = trainers;
            grid.SelectedItem = null;

            GetControl<Button>("SendRequestButton").IsEnabled =
                trainers.Count > 0;

            if (trainers.Count == 0)
            {
                ShowInfo(
                    "Nema odobrenih trenera za prikaz.");
            }
            else
            {
                ClearMessage();
            }
        }
        catch (Exception exception)
        {
            grid.ItemsSource = null;
            GetControl<Button>("SendRequestButton").IsEnabled = false;
            ShowError(exception);
        }
    }

    private void OnSendClick(object? sender, RoutedEventArgs e)
    {
        DataGrid grid = GetControl<DataGrid>("TrainersDataGrid");

        if (_viewModel is null)
        {
            ShowError("Prozor nije pravilno inicijalizovan.");
            return;
        }

        if (grid.SelectedItem is not Trainer trainer)
        {
            ShowError("Izaberite trenera.");
            return;
        }

        Button? button = sender as Button;

        try
        {
            if (button is not null)
                button.IsEnabled = false;

            _viewModel.SendRequest(trainer.TrainerId);
            ShowSuccess("Zahtev je uspešno poslat.");
            grid.SelectedItem = null;
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
        finally
        {
            if (button is not null)
                button.IsEnabled = true;
        }
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private T GetControl<T>(string name) where T : Control
    {
        return this.FindControl<T>(name)
            ?? throw new InvalidOperationException(
                $"Kontrola '{name}' nije pronađena u XAML-u.");
    }

    private void ShowSuccess(string text)
    {
        TextBlock message = GetControl<TextBlock>("MessageTextBlock");
        message.Text = text;
        message.Foreground = Brushes.DarkGreen;
    }

    private void ShowInfo(string text)
    {
        TextBlock message = GetControl<TextBlock>("MessageTextBlock");
        message.Text = text;
        message.Foreground = Brushes.DimGray;
    }

    private void ShowError(string text)
    {
        TextBlock message = GetControl<TextBlock>("MessageTextBlock");
        message.Text = text;
        message.Foreground = Brushes.DarkRed;
    }

    private void ShowError(Exception exception)
    {
        if (exception is InvalidOperationException or ArgumentException)
        {
            ShowError(exception.Message);
            return;
        }

        Console.Error.WriteLine(exception);
        ShowError("Došlo je do greške pri radu sa trenerima.");
    }

    private void ClearMessage()
    {
        GetControl<TextBlock>("MessageTextBlock").Text = string.Empty;
    }
}
