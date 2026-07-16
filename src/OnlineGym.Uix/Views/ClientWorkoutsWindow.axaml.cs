using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Domain.Enums;

namespace OnlineGym.Uix.Views;

public partial class ClientWorkoutsWindow : Window
{
    private readonly Client client;
    private readonly WorkoutRepository workoutRepository = new WorkoutRepository();
    private readonly WorkoutItemRepository workoutItemRepository = new WorkoutItemRepository();
    private readonly ExerciseRepository exerciseRepository = new ExerciseRepository();
    private readonly CollaborationRepository collaborationRepository = new CollaborationRepository();
    private readonly RatingRepository ratingRepository = new RatingRepository();

    private Workout? selectedWorkout;

    public ClientWorkoutsWindow(Client client)
    {
        InitializeComponent();
        this.client = client;
        LoadWorkouts();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadWorkouts()
    {
        var workoutsListBox = this.FindControl<ListBox>("WorkoutsListBox");
        workoutsListBox.ItemsSource = workoutRepository.GetByClientId(client.ClientId);
    }

    private void OnWorkoutSelected(object? sender, SelectionChangedEventArgs e)
    {
        var workoutsListBox = this.FindControl<ListBox>("WorkoutsListBox");
        selectedWorkout = workoutsListBox.SelectedItem as Workout;

        if (selectedWorkout == null)
            return;

        var selectedWorkoutText = this.FindControl<TextBlock>("SelectedWorkoutText");
        selectedWorkoutText.Text = $"Trening: {selectedWorkout.ScheduledAt}";

        var workoutRatingBox = this.FindControl<TextBox>("WorkoutRatingBox");
        var workoutCommentBox = this.FindControl<TextBox>("WorkoutCommentBox");

        workoutRatingBox.Text = selectedWorkout.WorkoutRating?.ToString() ?? "";
        workoutCommentBox.Text = selectedWorkout.Comment ?? "";

        LoadWorkoutItems(selectedWorkout.Id);
    }

    private void LoadWorkoutItems(long workoutId)
    {
        var workoutItemsListBox = this.FindControl<ListBox>("WorkoutItemsListBox");

        List<WorkoutItemDisplay> displayItems = new();
        List<WorkoutItem> workoutItems = workoutItemRepository.GetByWorkoutId(workoutId);

        foreach (WorkoutItem item in workoutItems)
        {
            Exercise? exercise = exerciseRepository.GetById(item.ExerciseId);

            displayItems.Add(new WorkoutItemDisplay(
                exercise?.Name ?? "Nepoznata vezba",
                item.RepetitionCount,
                exercise?.VideoUrl
            ));
        }

        workoutItemsListBox.ItemsSource = displayItems;
    }

    private void OnSaveWorkoutRatingClick(object? sender, RoutedEventArgs e)
    {
        var messageText = this.FindControl<TextBlock>("MessageText");

        if (selectedWorkout == null)
        {
            messageText.Text = "Izaberite trening.";
            return;
        }

        var workoutRatingBox = this.FindControl<TextBox>("WorkoutRatingBox");
        var workoutCommentBox = this.FindControl<TextBox>("WorkoutCommentBox");

        if (!int.TryParse(workoutRatingBox.Text, out int rating) || rating < 1 || rating > 5)
        {
            messageText.Text = "Ocena treninga mora biti broj od 1 do 5.";
            return;
        }

        selectedWorkout.WorkoutRating = rating;
        selectedWorkout.Comment = workoutCommentBox.Text;
        selectedWorkout.IsCompleted = true;
        selectedWorkout.Status = WorkoutStatus.Completed;

        workoutRepository.Update(selectedWorkout);

        messageText.Text = "Ocena treninga je sacuvana.";
        LoadWorkouts();
    }

    private void OnSaveTrainerRatingClick(object? sender, RoutedEventArgs e)
    {
        var messageText = this.FindControl<TextBlock>("MessageText");

        if (selectedWorkout == null)
        {
            messageText.Text = "Izaberite trening.";
            return;
        }

        var trainerRatingBox = this.FindControl<TextBox>("TrainerRatingBox");
        var trainerCommentBox = this.FindControl<TextBox>("TrainerCommentBox");

        if (!int.TryParse(trainerRatingBox.Text, out int ratingValue) || ratingValue < 1 || ratingValue > 5)
        {
            messageText.Text = "Ocena trenera mora biti broj od 1 do 5.";
            return;
        }

        Collaboration? collaboration = collaborationRepository.GetById(selectedWorkout.CollaborationId);

        if (collaboration == null)
        {
            messageText.Text = "Saradnja za izabrani trening nije pronadjena.";
            return;
        }

        Rating rating = new Rating(
            0,
            client.ClientId,
            collaboration.TrainerId,
            ratingValue,
            trainerCommentBox.Text,
            DateTime.Today
        );

        ratingRepository.Insert(rating);

        messageText.Text = "Ocena trenera je sacuvana.";
    }
}

public class WorkoutItemDisplay
{
    public string ExerciseName { get; set; }
    public string RepetitionText { get; set; }
    public string? VideoUrl { get; set; }

    public WorkoutItemDisplay(string exerciseName, int? repetitionCount, string? videoUrl)
    {
        ExerciseName = exerciseName;
        RepetitionText = repetitionCount.HasValue
            ? $"Broj ponavljanja: {repetitionCount.Value}"
            : "Broj ponavljanja nije definisan";
        VideoUrl = string.IsNullOrWhiteSpace(videoUrl)
            ? "Video nije dodat"
            : videoUrl;
    }
}