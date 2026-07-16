using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;
using OnlineGym.Application.Interfaces.Services;

namespace OnlineGym.Application.Services;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ITrainerRepository _trainerRepository;

    public ExerciseService(
        IExerciseRepository exerciseRepository,
        ITrainerRepository trainerRepository)
    {
        _exerciseRepository = exerciseRepository;
        _trainerRepository = trainerRepository;
    }

    public long CreateExercise(Exercise exercise, long trainerId)
    {
        EnsureTrainerExists(trainerId);

        if (exercise.TrainerId != trainerId)
            throw new InvalidOperationException(
                "The exercise must belong to the current trainer.");

        ValidateExercise(exercise);
        return _exerciseRepository.Insert(exercise);
    }

    public Exercise? GetById(long exerciseId)
    {
        return _exerciseRepository.GetById(exerciseId);
    }

    public List<Exercise> GetTrainerExercises(long trainerId)
    {
        EnsureTrainerExists(trainerId);
        return _exerciseRepository.GetByTrainerId(trainerId);
    }

    public void UpdateExercise(Exercise exercise, long trainerId)
    {
        EnsureTrainerExists(trainerId);

        Exercise? existingExercise =
            _exerciseRepository.GetById(exercise.Id);

        if (existingExercise is null)
            throw new InvalidOperationException(
                $"Exercise with ID {exercise.Id} does not exist.");

        if (existingExercise.TrainerId != trainerId ||
            exercise.TrainerId != trainerId)
        {
            throw new InvalidOperationException(
                "The trainer can update only their own exercises.");
        }

        ValidateExercise(exercise);
        _exerciseRepository.Update(exercise);
    }

    public void DeleteExercise(long exerciseId, long trainerId)
    {
        EnsureTrainerExists(trainerId);

        Exercise? exercise = _exerciseRepository.GetById(exerciseId);

        if (exercise is null)
            throw new InvalidOperationException(
                $"Exercise with ID {exerciseId} does not exist.");

        if (exercise.TrainerId != trainerId)
            throw new InvalidOperationException(
                "The trainer can delete only their own exercises.");

        _exerciseRepository.Delete(exerciseId);
    }

    private void EnsureTrainerExists(long trainerId)
    {
        if (_trainerRepository.GetById(trainerId) is null)
            throw new InvalidOperationException(
                $"Trainer with ID {trainerId} does not exist.");
    }

    private static void ValidateExercise(Exercise exercise)
    {
        if (string.IsNullOrWhiteSpace(exercise.Name))
            throw new ArgumentException("Exercise name is required.");
    }
}