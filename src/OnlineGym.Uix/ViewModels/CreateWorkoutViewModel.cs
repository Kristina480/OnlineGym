using System;
using System.Collections.Generic;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Services;
using OnlineGym.Application.Services;

namespace OnlineGym.Uix.ViewModels;

public class CreateWorkoutViewModel
{
    private readonly long _trainerId;
    private readonly IWorkoutService _workoutService;
    private readonly IExerciseService _exerciseService;

    public CreateWorkoutViewModel(long trainerId)
    {
        _trainerId = trainerId;

        _workoutService = new WorkoutService(
            new WorkoutRepository(),
            new WorkoutItemRepository(),
            new CollaborationRepository(),
            new ExerciseRepository(),
            new ClientRepository(),
            new TrainerRepository());

        _exerciseService = new ExerciseService(
            new ExerciseRepository(),
            new TrainerRepository());
    }

    public List<Client> GetActiveClients()
    {
        return _workoutService.GetActiveClients(_trainerId);
    }

    public List<Exercise> GetExercises()
    {
        return _exerciseService.GetTrainerExercises(_trainerId);
    }

    public long CreateWorkout(
        long clientId,
        DateTime scheduledAt,
        string? comment,
        List<WorkoutItem> items)
    {
        return _workoutService.CreateWorkout(
            _trainerId,
            clientId,
            scheduledAt,
            comment,
            items);
    }
}
