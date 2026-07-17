using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;
namespace OnlineGym.Uix.Views;

public partial class TrainerWindow : Window
{
    TrainerRepository trainerRepo=new TrainerRepository();
    public Trainer trainer;
    public TrainerWindow(long accountId)
    {
        InitializeComponent();
        trainer=trainerRepo.GetTrainerByAccountId(accountId);
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

   

    private void OnLogoutClick(object? sender, RoutedEventArgs e)
    {
        trainer = null;
        this.Close();
        // var loginWindow = new LoginWindow();
        // loginWindow.Show();
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
    
    private void OnRequestsClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (trainer is null)
            return;

        CollaborationRequestsWindow window =
            new CollaborationRequestsWindow(trainer.TrainerId);

        Hide();

        window.Closed += (_, _) =>
        {
            Show();
            Activate();
        };

        window.Show();
    }

    private void OnExercisesClick(
        object? sender,
        RoutedEventArgs e)
    {
        if (trainer is null)
            return;

        ExerciseManagementWindow window =
            new ExerciseManagementWindow(trainer.TrainerId);

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
        if (trainer is null)
            return;

        CreateWorkoutWindow window =
            new CreateWorkoutWindow(trainer.TrainerId);

        Hide();

        window.Closed += (_, _) =>
        {
            Show();
            Activate();
        };

        window.Show();
    }

}