using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Services;

public interface IExerciseService
{
    long CreateExercise(Exercise exercise, long trainerId);
    Exercise? GetById(long exerciseId);
    List<Exercise> GetTrainerExercises(long trainerId);
    void UpdateExercise(Exercise exercise, long trainerId);
    void DeleteExercise(long exerciseId, long trainerId);
}