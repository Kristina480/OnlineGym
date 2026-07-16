namespace OnlineGym.Application.Domain;

public class WorkoutItem
{
    public long Id { get; set; }

    public long WorkoutId { get; set; }

    public long ExerciseId { get; set; }

    public int? RepetitionCount { get; set; }

    public bool IsCompleted { get; set; }

    public int? ItemRating { get; set; }

    public string? Comment { get; set; }

    public WorkoutItem(
        long id,
        long workoutId,
        long exerciseId,
        int? repetitionCount,
        bool isCompleted,
        int? itemRating,
        string? comment)
    {
        Id = id;
        WorkoutId = workoutId;
        ExerciseId = exerciseId;
        RepetitionCount = repetitionCount;
        IsCompleted = isCompleted;
        ItemRating = itemRating;
        Comment = comment;
    }
}