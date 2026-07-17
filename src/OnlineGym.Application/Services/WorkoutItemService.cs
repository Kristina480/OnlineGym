using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;
using OnlineGym.Application.Interfaces.Services;

namespace OnlineGym.Application.Services;

public class WorkoutItemService : IWorkoutItemService
{
    private const int MinRating = 1;
    private const int MaxRating = 5;

    private readonly IWorkoutItemRepository _workoutItemRepository;
    private readonly IWorkoutRepository _workoutRepository;
    private readonly ICollaborationRepository _collaborationRepository;

    public WorkoutItemService(
        IWorkoutItemRepository workoutItemRepository,
        IWorkoutRepository workoutRepository,
        ICollaborationRepository collaborationRepository)
    {
        _workoutItemRepository = workoutItemRepository;
        _workoutRepository = workoutRepository;
        _collaborationRepository = collaborationRepository;
    }

    public List<WorkoutItem> GetWorkoutItems(long workoutId, long clientId)
    {
        Workout workout = GetOwnedWorkout(workoutId, clientId);

        return _workoutItemRepository.GetByWorkoutId(workout.Id);
    }

    public WorkoutItem RateWorkoutItem(
        long workoutItemId,
        long clientId,
        int rating,
        string? comment)
    {
        if (rating < MinRating || rating > MaxRating)
            throw new InvalidOperationException(
                $"Rating must be between {MinRating} and {MaxRating}.");

        WorkoutItem item = _workoutItemRepository.GetById(workoutItemId)
            ?? throw new InvalidOperationException(
                $"Workout item with ID {workoutItemId} does not exist.");

        GetOwnedWorkout(item.WorkoutId, clientId);

        if (!item.IsCompleted)
            throw new InvalidOperationException(
                "Only completed exercises can be rated.");

        item.ItemRating = rating;
        item.Comment = comment;

        _workoutItemRepository.Update(item);

        return item;
    }

    private Workout GetOwnedWorkout(long workoutId, long clientId)
    {
        Workout workout = _workoutRepository.GetById(workoutId)
            ?? throw new InvalidOperationException(
                $"Workout with ID {workoutId} does not exist.");

        Collaboration collaboration = _collaborationRepository.GetById(workout.CollaborationId)
            ?? throw new InvalidOperationException(
                $"Collaboration with ID {workout.CollaborationId} does not exist.");

        if (collaboration.ClientId != clientId)
            throw new InvalidOperationException(
                "Client does not have permission to access this workout.");

        return workout;
    }
}