using System.Data;
using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Database.Repositories;

public class ExerciseRepository
{
     public long Insert(Exercise exercise)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO exercises
            (trainer_id, equipment_id, machine_id, name, video_url)
            VALUES
            (@trainer_id, @equipment_id, @machine_id, @name, @video_url)
            RETURNING exercise_id;";

        DataBaseHelper.AddParameter(command, "@trainer_id", exercise.TrainerId);
        DataBaseHelper.AddParameter(command, "@equipment_id", exercise.EquipmentId ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@machine_id", exercise.MachineId ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@name", exercise.Name);
        DataBaseHelper.AddParameter(command, "@video_url", exercise.VideoUrl ?? (object)DBNull.Value);

        object? result = command.ExecuteScalar();
        if (result is null || result == DBNull.Value)
            throw new InvalidOperationException("Exercise was not created.");

        return Convert.ToInt64(result);
    }

    public Exercise? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT exercise_id, trainer_id, equipment_id, machine_id, name, video_url
            FROM exercises
            WHERE exercise_id = @exercise_id;";

        DataBaseHelper.AddParameter(command, "@exercise_id", id);

        using IDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapFromReader(reader) : null;
    }

    public List<Exercise> GetByTrainerId(long trainerId)
    {
        List<Exercise> exercises = new();

        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT exercise_id, trainer_id, equipment_id, machine_id, name, video_url
            FROM exercises
            WHERE trainer_id = @trainer_id
            ORDER BY name;";

        DataBaseHelper.AddParameter(command, "@trainer_id", trainerId);

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
            exercises.Add(MapFromReader(reader));

        return exercises;
    }

    public bool ExistsByIdAndTrainerId(long exerciseId, long trainerId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT EXISTS (
                SELECT 1
                FROM exercises
                WHERE exercise_id = @exercise_id
                  AND trainer_id = @trainer_id
            );";

        DataBaseHelper.AddParameter(command, "@exercise_id", exerciseId);
        DataBaseHelper.AddParameter(command, "@trainer_id", trainerId);

        return Convert.ToBoolean(command.ExecuteScalar());
    }

    public void Update(Exercise exercise)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE exercises
            SET trainer_id = @trainer_id,
                equipment_id = @equipment_id,
                machine_id = @machine_id,
                name = @name,
                video_url = @video_url
            WHERE exercise_id = @exercise_id;";

        DataBaseHelper.AddParameter(command, "@exercise_id", exercise.Id);
        DataBaseHelper.AddParameter(command, "@trainer_id", exercise.TrainerId);
        DataBaseHelper.AddParameter(command, "@equipment_id", exercise.EquipmentId ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@machine_id", exercise.MachineId ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@name", exercise.Name);
        DataBaseHelper.AddParameter(command, "@video_url", exercise.VideoUrl ?? (object)DBNull.Value);

        command.ExecuteNonQuery();
    }

    public void Delete(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM exercises
            WHERE exercise_id = @exercise_id;";

        DataBaseHelper.AddParameter(command, "@exercise_id", id);
        command.ExecuteNonQuery();
    }

    private Exercise MapFromReader(IDataReader reader)
    {
        int equipmentIdIndex = reader.GetOrdinal("equipment_id");
        int machineIdIndex = reader.GetOrdinal("machine_id");
        int videoUrlIndex = reader.GetOrdinal("video_url");

        return new Exercise(
            reader.GetInt64(reader.GetOrdinal("exercise_id")),
            reader.GetInt64(reader.GetOrdinal("trainer_id")),
            reader.IsDBNull(equipmentIdIndex) ? null : reader.GetInt64(equipmentIdIndex),
            reader.IsDBNull(machineIdIndex) ? null : reader.GetInt64(machineIdIndex),
            reader.GetString(reader.GetOrdinal("name")),
            reader.IsDBNull(videoUrlIndex) ? null : reader.GetString(videoUrlIndex)
        );
    }
}