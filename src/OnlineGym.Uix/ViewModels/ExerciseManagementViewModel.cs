using System.Collections.Generic;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Services;
using OnlineGym.Application.Services;

namespace OnlineGym.Uix.ViewModels;

public class ExerciseManagementViewModel
{
    private readonly long _trainerId;
    private readonly IExerciseService _service;

    public ExerciseManagementViewModel(long trainerId)
    {
        _trainerId = trainerId;

        _service = new ExerciseService(
            new ExerciseRepository(),
            new TrainerRepository());
    }

    public List<Exercise> GetExercises()
    {
        return _service.GetTrainerExercises(_trainerId);
    }

    public long CreateExercise(
        string name,
        string? videoUrl,
        long? equipmentId,
        long? machineId)
    {
        Exercise exercise = new Exercise(
            0,
            _trainerId,
            equipmentId,
            machineId,
            name,
            videoUrl);

        return _service.CreateExercise(exercise, _trainerId);
    }

    public void DeleteExercise(long exerciseId)
    {
        _service.DeleteExercise(exerciseId, _trainerId);
    }
}
