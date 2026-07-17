using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Services;

public interface IWorkoutItemService
{
    List<WorkoutItem> GetWorkoutItems(long workoutId, long clientId);

    WorkoutItem RateWorkoutItem(
        long workoutItemId,
        long clientId,
        int rating,
        string? comment);
}