using OnlineGym.Application.Domain.Enums;

namespace OnlineGym.Application.Domain;

public class Workout
{
    public long Id { get; set; }

    public long CollaborationId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public WorkoutStatus Status { get; set; }

    public bool IsCompleted { get; set; }

    public string? Comment { get; set; }

    public int? WorkoutRating { get; set; }

    public Workout(
        long id,
        long collaborationId,
        DateTime scheduledAt,
        WorkoutStatus status,
        bool isCompleted,
        string? comment,
        int? workoutRating)
    {
        Id = id;
        CollaborationId = collaborationId;
        ScheduledAt = scheduledAt;
        Status = status;
        IsCompleted = isCompleted;
        Comment = comment;
        WorkoutRating = workoutRating;
    }
}