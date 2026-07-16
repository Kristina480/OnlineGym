using System.Data;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;

namespace OnlineGym.Application.Database.Repositories;

public class RatingRepository : IRatingRepository
{
    public long Insert(Rating rating)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO ratings
        (client_id, trainer_id, rating, comment, rating_date)
        VALUES
        (@client_id, @trainer_id, @rating, @comment, @rating_date)
        RETURNING rating_id;";

        DataBaseHelper.AddParameter(command, "@client_id", rating.ClientId);
        DataBaseHelper.AddParameter(command, "@trainer_id", rating.TrainerId);
        DataBaseHelper.AddParameter(command, "@rating", rating.RatingValue);
        DataBaseHelper.AddParameter(command, "@comment", rating.Comment ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@rating_date", rating.RatingDate);

        object? result = command.ExecuteScalar();

        if (result is null || result == DBNull.Value)
            throw new InvalidOperationException("Rating was not created.");

        UpdateTrainerAverageRating(rating.TrainerId);

        return Convert.ToInt64(result);
    }

    public Rating? GetByClientAndTrainer(long clientId, long trainerId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT rating_id, client_id, trainer_id, rating, comment, rating_date
        FROM ratings
        WHERE client_id = @client_id AND trainer_id = @trainer_id
        LIMIT 1;";

        DataBaseHelper.AddParameter(command, "@client_id", clientId);
        DataBaseHelper.AddParameter(command, "@trainer_id", trainerId);

        using IDataReader reader = command.ExecuteReader();

        return reader.Read() ? MapFromReader(reader) : null;
    }

    public void UpdateTrainerAverageRating(long trainerId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        UPDATE trainers
        SET average_rating = (
            SELECT COALESCE(AVG(rating), 0)
            FROM ratings
            WHERE trainer_id = @trainer_id
        )
        WHERE trainer_id = @trainer_id;";

        DataBaseHelper.AddParameter(command, "@trainer_id", trainerId);

        command.ExecuteNonQuery();
    }

    private Rating MapFromReader(IDataReader reader)
    {
        return new Rating(
            reader.GetInt64(reader.GetOrdinal("rating_id")),
            reader.GetInt64(reader.GetOrdinal("client_id")),
            reader.GetInt64(reader.GetOrdinal("trainer_id")),
            reader.GetInt32(reader.GetOrdinal("rating")),
            reader.IsDBNull(reader.GetOrdinal("comment")) ? null : reader.GetString(reader.GetOrdinal("comment")),
            reader.GetDateTime(reader.GetOrdinal("rating_date"))
        );
    }
}