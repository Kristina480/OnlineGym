using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Services;

public interface IWorkoutService
{
    long CreateWorkout(
        long trainerId,
        long clientId,
        DateTime scheduledAt,
        string? comment,
        List<WorkoutItem> items);

    Workout? GetById(long workoutId);
    List<Workout> GetByClientId(long clientId);
    List<WorkoutItem> GetWorkoutItems(long workoutId);
    List<Client> GetActiveClients(long trainerId);
}