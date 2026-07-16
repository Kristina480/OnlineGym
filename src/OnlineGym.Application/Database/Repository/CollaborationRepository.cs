using System.Data;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Domain.Enums;
using OnlineGym.Application.Interfaces.Repositories;

namespace OnlineGym.Application.Database.Repositories;

public class CollaborationRepository: ICollaborationRepository
{
    public long Insert(Collaboration collaboration)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO collaborations
        (trainer_id, client_id, request_id, pricing_package_id, start_date, end_date, status, workouts_per_week, monthly_price)
        VALUES
        (@trainer_id, @client_id, @request_id, @pricing_package_id, @start_date, @end_date,
         @status::collaboration_status_enum, @workouts_per_week, @monthly_price)
        RETURNING collaboration_id;";

        DataBaseHelper.AddParameter(command, "@trainer_id", collaboration.TrainerId);
        DataBaseHelper.AddParameter(command, "@client_id", collaboration.ClientId);
        DataBaseHelper.AddParameter(command, "@request_id", collaboration.RequestId ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@pricing_package_id", collaboration.PricingPackageId);
        DataBaseHelper.AddParameter(command, "@start_date", collaboration.StartDate);
        DataBaseHelper.AddParameter(command, "@end_date", collaboration.EndDate ?? (object)DBNull.Value);
        DataBaseHelper.AddParameter(command, "@status", collaboration.Status.ToString().ToUpper());
        DataBaseHelper.AddParameter(command, "@workouts_per_week", collaboration.WorkoutsPerWeek);
        DataBaseHelper.AddParameter(command, "@monthly_price", collaboration.MonthlyPrice);

        object? result = command.ExecuteScalar();

        if (result is null || result == DBNull.Value)
            throw new InvalidOperationException("Collaboration was not created.");

        return Convert.ToInt64(result);
    }

    public Collaboration? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT collaboration_id, trainer_id, client_id, request_id, pricing_package_id,
               start_date, end_date, status, workouts_per_week, monthly_price
        FROM collaborations
        WHERE collaboration_id = @collaboration_id;";

        DataBaseHelper.AddParameter(command, "@collaboration_id", id);

        using IDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapFromReader(reader) : null;
    }

    public Collaboration? GetActiveByClientAndTrainer(long clientId, long trainerId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT collaboration_id, trainer_id, client_id, request_id, pricing_package_id,
               start_date, end_date, status, workouts_per_week, monthly_price
        FROM collaborations
        WHERE client_id = @client_id
          AND trainer_id = @trainer_id
          AND status = 'ACTIVE'::collaboration_status_enum
        LIMIT 1;";

        DataBaseHelper.AddParameter(command, "@client_id", clientId);
        DataBaseHelper.AddParameter(command, "@trainer_id", trainerId);

        using IDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapFromReader(reader) : null;
    }

    public bool HasActiveCollaboration(long clientId, long trainerId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT EXISTS (
            SELECT 1
            FROM collaborations
            WHERE client_id = @client_id
              AND trainer_id = @trainer_id
              AND status = 'ACTIVE'::collaboration_status_enum
        );";

        DataBaseHelper.AddParameter(command, "@client_id", clientId);
        DataBaseHelper.AddParameter(command, "@trainer_id", trainerId);

        return Convert.ToBoolean(command.ExecuteScalar());
    }

    
    private Collaboration MapFromReader(IDataReader reader)
    {
        int requestIdIndex = reader.GetOrdinal("request_id");
        int endDateIndex = reader.GetOrdinal("end_date");

        return new Collaboration(
            reader.GetInt64(reader.GetOrdinal("collaboration_id")),
            reader.GetInt64(reader.GetOrdinal("trainer_id")),
            reader.GetInt64(reader.GetOrdinal("client_id")),
            reader.IsDBNull(requestIdIndex) ? null : reader.GetInt64(requestIdIndex),
            reader.GetInt64(reader.GetOrdinal("pricing_package_id")),
            reader.GetDateTime(reader.GetOrdinal("start_date")),
            reader.IsDBNull(endDateIndex) ? null : reader.GetDateTime(endDateIndex),
            Enum.Parse<CollaborationStatus>(
                reader.GetString(reader.GetOrdinal("status")),
                true),
            reader.GetInt32(reader.GetOrdinal("workouts_per_week")),
            reader.GetDecimal(reader.GetOrdinal("monthly_price"))
        );
    }
    

}