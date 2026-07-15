using System.Data;
using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Database.Repositories;

public class WorkoutItemRepository
{
    public long Insert(WorkoutItem item)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO workout_items
        (workout_id, exercise_id, repetition_count, is_completed, item_rating, comment)
        VALUES
        (@workout_id, @exercise_id, @repetition_count,
         @is_completed, @item_rating, @comment)
        RETURNING workout_item_id;";

        DataBaseHelper.AddParameter(command, "@workout_id", item.WorkoutId);
        DataBaseHelper.AddParameter(command, "@exercise_id", item.ExerciseId);
        DataBaseHelper.AddParameter(command, "@repetition_count", item.RepetitionCount ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@is_completed", item.IsCompleted);
        DataBaseHelper.AddParameter(command, "@item_rating", item.ItemRating ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@comment", item.Comment ?? (object)DBNull.Value);

        object? result = command.ExecuteScalar();

        if (result is null || result == DBNull.Value)
            throw new InvalidOperationException("Workout item was not created.");

        return Convert.ToInt64(result);
    }
    public void CreateMany(List<WorkoutItem> items)
    {
        foreach (WorkoutItem item in items)
        {
            Insert(item);
        }
    }
    
    public List<WorkoutItem> GetByWorkoutId(long workoutId)
    {
        List<WorkoutItem> items = new();

        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT workout_item_id,
               workout_id,
               exercise_id,
               repetition_count,
               is_completed,
               item_rating,
               comment
        FROM workout_items
        WHERE workout_id = @workout_id;";

        DataBaseHelper.AddParameter(command, "@workout_id", workoutId);

        using IDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            items.Add(MapFromReader(reader));
        }

        return items;
    }
    public WorkoutItem? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT workout_item_id,
               workout_id,
               exercise_id,
               repetition_count,
               is_completed,
               item_rating,
               comment
        FROM workout_items
        WHERE workout_item_id = @workout_item_id;";

        DataBaseHelper.AddParameter(command, "@workout_item_id", id);

        using IDataReader reader = command.ExecuteReader();

        return reader.Read() ? MapFromReader(reader) : null;
    }
    public void Update(WorkoutItem item)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        UPDATE workout_items
        SET
            workout_id = @workout_id,
            exercise_id = @exercise_id,
            repetition_count = @repetition_count,
            is_completed = @is_completed,
            item_rating = @item_rating,
            comment = @comment
        WHERE workout_item_id = @workout_item_id;";

        DataBaseHelper.AddParameter(command, "@workout_item_id", item.Id);
        DataBaseHelper.AddParameter(command, "@workout_id", item.WorkoutId);
        DataBaseHelper.AddParameter(command, "@exercise_id", item.ExerciseId);
        DataBaseHelper.AddParameter(command, "@repetition_count", item.RepetitionCount ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@is_completed", item.IsCompleted);
        DataBaseHelper.AddParameter(command, "@item_rating", item.ItemRating ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@comment", item.Comment ?? (object)DBNull.Value);

        command.ExecuteNonQuery();
    }
    private WorkoutItem MapFromReader(IDataReader reader)
    {
        int repetitionIndex = reader.GetOrdinal("repetition_count");
        int ratingIndex = reader.GetOrdinal("item_rating");
        int commentIndex = reader.GetOrdinal("comment");

        return new WorkoutItem(
            reader.GetInt64(reader.GetOrdinal("workout_item_id")),
            reader.GetInt64(reader.GetOrdinal("workout_id")),
            reader.GetInt64(reader.GetOrdinal("exercise_id")),
            reader.IsDBNull(repetitionIndex)
                ? null
                : reader.GetInt32(repetitionIndex),
            reader.GetBoolean(reader.GetOrdinal("is_completed")),
            reader.IsDBNull(ratingIndex)
                ? null
                : reader.GetInt32(ratingIndex),
            reader.IsDBNull(commentIndex)
                ? null
                : reader.GetString(commentIndex)
        );
    }
}