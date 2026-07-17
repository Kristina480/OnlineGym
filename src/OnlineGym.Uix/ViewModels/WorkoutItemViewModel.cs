using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineGym.Application.Interfaces.Services;

namespace OnlineGym.Uix.ViewModels;

public partial class WorkoutItemViewModel : ViewModelBase
{
    private readonly IWorkoutItemService _workoutItemService;
    private readonly long _clientId;

    public long WorkoutItemId { get; }
    public string ExerciseName { get; }
    public bool IsCompleted { get; }
    public string CompletedStatusText => IsCompleted ? "Odradjeno: da" : "Odradjeno: ne";

    [ObservableProperty]
    public partial decimal Rating { get; set; }

    [ObservableProperty]
    public partial string? Comment { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = "";

    public WorkoutItemViewModel(
        IWorkoutItemService workoutItemService,
        long clientId,
        long workoutItemId,
        string exerciseName,
        bool isCompleted,
        int? rating,
        string? comment)
    {
        _workoutItemService = workoutItemService;
        _clientId = clientId;

        WorkoutItemId = workoutItemId;
        ExerciseName = exerciseName;
        IsCompleted = isCompleted;
        Rating = rating ?? 0;
        Comment = comment;
    }

    [RelayCommand]
    private void SaveRating()
    {
        try
        {
            _workoutItemService.RateWorkoutItem(WorkoutItemId, _clientId, (int)Rating, Comment);
            StatusMessage = "Sacuvano.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }
}