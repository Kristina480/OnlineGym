using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface IWorkoutItemRepository
{
    long Insert(WorkoutItem item);

    void CreateMany(List<WorkoutItem> items);

    WorkoutItem? GetById(long id);

    List<WorkoutItem> GetByWorkoutId(long workoutId);

    void Update(WorkoutItem item);
}