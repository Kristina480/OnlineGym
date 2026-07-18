using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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

    private void OnEquipmentClick(
        object? sender,
        RoutedEventArgs e)
    {
        EquipmentsWindow equipmentsWindow = new();
        equipmentsWindow.ShowDialog(this);
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
    

    private void OnExercisesClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (Trainer is null)
            return;

        ExerciseManagementWindow window =
            new ExerciseManagementWindow(Trainer.TrainerId);

        Hide();

        window.Closed += (_, _) =>
        {
            Show();
            Activate();
        };

        window.Show();
    }

    private void OnCreateWorkoutClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (Trainer is null)
            return;

        CreateWorkoutWindow window =
            new CreateWorkoutWindow(Trainer.TrainerId);

        Hide();

        window.Closed += (_, _) =>
        {
            Show();
            Activate();
        };

        window.Show();
    }
    private void OnRequestsClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (Trainer is null)
        {
            return;
        }

        CollaborationRequestsWindow requestsWindow =
            new(Trainer.TrainerId);

        requestsWindow.ShowDialog(this);
    }

}