using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OnlineGym.Application.Interfaces.Repositories;
using OnlineGym.Application.Interfaces.Services;

namespace OnlineGym.Uix.ViewModels;

public partial class RateWorkoutItemsViewModel : ViewModelBase
{
    private readonly IWorkoutItemService _workoutItemService;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly long _clientId;

    [ObservableProperty]
    public partial decimal WorkoutId { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = "";

    public ObservableCollection<WorkoutItemViewModel> Items { get; } = new();

    public RateWorkoutItemsViewModel(
        IWorkoutItemService workoutItemService,
        IExerciseRepository exerciseRepository,
        long clientId)
    {
        _workoutItemService = workoutItemService;
        _exerciseRepository = exerciseRepository;
        _clientId = clientId;
    }

    [RelayCommand]
    private void LoadItems()
    {
        StatusMessage = "";
        Items.Clear();

        try
        {
            var items = _workoutItemService.GetWorkoutItems((long)WorkoutId, _clientId);

            foreach (var item in items)
            {
                var exercise = _exerciseRepository.GetById(item.ExerciseId);

                Items.Add(new WorkoutItemViewModel(
                    _workoutItemService,
                    _clientId,
                    item.Id,
                    exercise?.Name ?? $"Vezba #{item.ExerciseId}",
                    item.IsCompleted,
                    item.ItemRating,
                    item.Comment));
            }

            if (Items.Count == 0)
                StatusMessage = "Nema stavki za ovaj trening.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }
}