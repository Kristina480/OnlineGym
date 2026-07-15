using System.Data;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Domain.Enums;

namespace OnlineGym.Application.Database.Repositories;

public class WorkoutRepository
{
    public long Insert(Workout workout)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO workouts
        (collaboration_id, scheduled_at, status, is_completed, comment, workout_rating)
        VALUES
        (@collaboration_id, @scheduled_at, @status::workout_status_enum,
         @is_completed, @comment, @workout_rating)
        RETURNING workout_id;";

        DataBaseHelper.AddParameter(command, "@collaboration_id", workout.CollaborationId);
        DataBaseHelper.AddParameter(command, "@scheduled_at", workout.ScheduledAt);
        DataBaseHelper.AddParameter(command, "@status", workout.Status.ToString().ToUpper());
        DataBaseHelper.AddParameter(command, "@is_completed", workout.IsCompleted);
        DataBaseHelper.AddParameter(command, "@comment", workout.Comment ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@workout_rating", workout.WorkoutRating ?? (object)DBNull.Value);

        object? result = command.ExecuteScalar();

        if (result is null || result == DBNull.Value)
            throw new InvalidOperationException("Workout was not created.");

        return Convert.ToInt64(result);
    }

    public Workout? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT workout_id, collaboration_id, scheduled_at, status,
               is_completed, comment, workout_rating
        FROM workouts
        WHERE workout_id = @workout_id;";

        DataBaseHelper.AddParameter(command, "@workout_id", id);

        using IDataReader reader = command.ExecuteReader();

        return reader.Read() ? MapFromReader(reader) : null;
    }
    public List<Workout> GetByCollaborationId(long collaborationId)
    {
        List<Workout> workouts = new();

        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT workout_id, collaboration_id, scheduled_at, status,
               is_completed, comment, workout_rating
        FROM workouts
        WHERE collaboration_id = @collaboration_id
        ORDER BY scheduled_at;";

        DataBaseHelper.AddParameter(command, "@collaboration_id", collaborationId);

        using IDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            workouts.Add(MapFromReader(reader));
        }

        return workouts;
    }
    public List<Workout> GetByClientId(long clientId)
    {
        List<Workout> workouts = new();

        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT w.workout_id,
               w.collaboration_id,
               w.scheduled_at,
               w.status,
               w.is_completed,
               w.comment,
               w.workout_rating
        FROM workouts w
        JOIN collaborations c
             ON w.collaboration_id = c.collaboration_id
        WHERE c.client_id = @client_id
        ORDER BY w.scheduled_at;";

        DataBaseHelper.AddParameter(command, "@client_id", clientId);

        using IDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            workouts.Add(MapFromReader(reader));
        }

        return workouts;
    }
    public void Update(Workout workout)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        UPDATE workouts
        SET
            collaboration_id = @collaboration_id,
            scheduled_at = @scheduled_at,
            status = @status::workout_status_enum,
            is_completed = @is_completed,
            comment = @comment,
            workout_rating = @workout_rating
        WHERE workout_id = @workout_id;";

        DataBaseHelper.AddParameter(command, "@workout_id", workout.Id);
        DataBaseHelper.AddParameter(command, "@collaboration_id", workout.CollaborationId);
        DataBaseHelper.AddParameter(command, "@scheduled_at", workout.ScheduledAt);
        DataBaseHelper.AddParameter(command, "@status", workout.Status.ToString().ToUpper());
        DataBaseHelper.AddParameter(command, "@is_completed", workout.IsCompleted);
        DataBaseHelper.AddParameter(command, "@comment", workout.Comment ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@workout_rating", workout.WorkoutRating ?? (object)DBNull.Value);

        command.ExecuteNonQuery();
    }
    private Workout MapFromReader(IDataReader reader)
    {
        int commentIndex = reader.GetOrdinal("comment");
        int ratingIndex = reader.GetOrdinal("workout_rating");

        return new Workout(
            reader.GetInt64(reader.GetOrdinal("workout_id")),
            reader.GetInt64(reader.GetOrdinal("collaboration_id")),
            reader.GetDateTime(reader.GetOrdinal("scheduled_at")),
            Enum.Parse<WorkoutStatus>(
                reader.GetString(reader.GetOrdinal("status")),
                true),
            reader.GetBoolean(reader.GetOrdinal("is_completed")),
            reader.IsDBNull(commentIndex)
                ? null
                : reader.GetString(commentIndex),
            reader.IsDBNull(ratingIndex)
                ? null
                : reader.GetInt32(ratingIndex)
        );
    }
}