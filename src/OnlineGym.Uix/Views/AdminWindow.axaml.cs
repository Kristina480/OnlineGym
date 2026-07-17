using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;

namespace OnlineGym.Uix.Views;

public partial class AdminWindow : Window
{
    public RegistrationRequestRepository requestRepo = new RegistrationRequestRepository();
    public TrainerRepository trainerRepo = new TrainerRepository();
    
    public AdminWindow()
    {
        InitializeComponent();
        LoadAllData();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadAllData()
    {
        LoadApprovedTrainers();
        LoadPendingTrainers();
    }
    private void LoadApprovedTrainers()
    {
        try
        {
            var trainers = requestRepo.GetApprovedTrainers();
            var listBox = this.FindControl<ListBox>("ApprovedTrainersListBox");
            if (listBox != null)
            {
                listBox.ItemsSource = trainers;
            }
        }
        catch (Exception ex)
        {
            ShowStatus($"Greska: {ex.Message}");
        }
    }
    private void LoadPendingTrainers()
    {
        try
        {
            var trainers = requestRepo.GetNotRegisteredTrainers();
            var listBox = this.FindControl<ListBox>("PendingTrainersListBox");
            if (listBox != null)
            {
                listBox.ItemsSource = trainers;
            }
        }
        catch (Exception ex)
        {
            ShowStatus($"Greska: {ex.Message}");
        }
    }
    private void OnApproveClick(object? sender, RoutedEventArgs e)
    {
        var listBox = this.FindControl<ListBox>("PendingTrainersListBox");
        if (listBox?.SelectedItem is Trainer selectedTrainer)
        {
            if (requestRepo.HasLicense(selectedTrainer.TrainerId))
            {
                bool success = requestRepo.UpdateStatus(selectedTrainer.TrainerId, "APPROVED");
                if (success)
                {
                    ShowStatus($"Trener {selectedTrainer.FirstName} {selectedTrainer.LastName} je ODOBREN!");
                    LoadAllData();
                }
                else
                {
                    ShowStatus("Greska pri odobravanju.");
                }
            }
            else
            {
                ShowStatus("Ovaj trener nema licencu, ne mozete ga odobriti.");
            }
        }
        else
        {
            ShowStatus("Izaberite trenera iz liste neregistrovanih.");
        }
    }
    private void OnRejectClick(object? sender, RoutedEventArgs e)
    {
        var listBox = this.FindControl<ListBox>("PendingTrainersListBox");
        if (listBox?.SelectedItem is Trainer selectedTrainer)
        {
            bool success = requestRepo.UpdateStatus(selectedTrainer.TrainerId, "REJECTED");
            if (success)
            {
                ShowStatus($"Trener {selectedTrainer.FirstName} {selectedTrainer.LastName} je ODBIJEN!");
                LoadAllData();
            }
            else
            {
                ShowStatus("Greska pri odbijanju.");
            }
        }
        else
        {
            ShowStatus("Izaberite trenera iz liste neregistrovanih.");
        }
    }
    private void OnDeleteTrainerClick(object? sender, RoutedEventArgs e)
    {
        var listBox = this.FindControl<ListBox>("ApprovedTrainersListBox");
        if (listBox?.SelectedItem is Trainer selectedTrainer)
        {
            bool success = trainerRepo.DeleteTrainer(selectedTrainer.TrainerId);
            bool succes2 = trainerRepo.DeleteTrainerAccount(selectedTrainer.AccountId);
            if (success && succes2)
            {
                ShowStatus($"Trener {selectedTrainer.FirstName} {selectedTrainer.LastName} je izbacen!");
                LoadAllData();
            }
            else
            {
                ShowStatus("Greska pri brisanju.");
            }
        }
        else
        {
            ShowStatus("Izaberite trenera iz liste aktivnih.");
        }
    }

    private void OnRefreshClick(object? sender, RoutedEventArgs e)
    {
        LoadAllData();
    }

    private void ShowStatus(string message)
    {
        var statusText = this.FindControl<TextBlock>("StatusText");
        if (statusText != null)
        {
            statusText.Text = message;
        }
    }

    private void OnLogoutClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
        var loginWindow = new LoginWindow();
        loginWindow.Show();
    }
    
}