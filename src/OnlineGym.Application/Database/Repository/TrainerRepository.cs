using System.Data;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;

namespace OnlineGym.Application.Database.Repositories;

public class TrainerRepository:ITrainerRepository
{
    public bool UsernameExists(string username)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM user_accounts WHERE username = @username";
        AddParameter(command, "username", username);

        long count = Convert.ToInt64(command.ExecuteScalar());
        return count > 0;
    }

    public long SaveUserAccount(string username, string password)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO user_accounts(username,password,user_type) " +
                              "VALUES(@username,@password,'TRAINER') " +
                              "RETURNING account_id";
        AddParameter(command, "username", username);
        AddParameter(command, "password", password);

        object? result = command.ExecuteScalar();
        return Convert.ToInt64(result);
    }

    public long SaveTrainer(long accountId, string firstName, string lastName, string? specialization, string? education, string? recommendations)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO trainers(account_id, first_name, last_name, specialization, average_rating, education, recommendations) " +
                              "VALUES(@account_id,@first_name,@last_name,@specialization,0,@education,@recommendations) " +
                              "RETURNING trainer_id";
        AddParameter(command, "account_id", accountId);
        AddParameter(command, "first_name", firstName);
        AddParameter(command, "last_name", lastName);
        AddParameter(command, "specialization", specialization ?? (object)DBNull.Value);
        AddParameter(command, "education", education ?? (object)DBNull.Value);
        AddParameter(command, "recommendations", recommendations ?? (object)DBNull.Value);

        object? result = command.ExecuteScalar();
        return Convert.ToInt64(result);
    }

    public void CreateRegistrationRequest(long trainerId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO registration_requests(trainer_id, administrator_id, request_date, status) " +
                              "VALUES(@trainer_id,1,CURRENT_DATE,'PENDING')";

        AddParameter(command, "trainer_id", trainerId);
        command.ExecuteNonQuery();
    }

    public void RegisterTrainer(string username, string password, string firstName, string lastName, string? specialization, string? education, string? recommendations)
    {
        long accountId = SaveUserAccount(username, password);
        long trainerId = SaveTrainer(accountId, firstName, lastName, specialization, education, recommendations);
        CreateRegistrationRequest(trainerId);
    }

    public void SaveLicense(long trainerId, string name, string documentType, DateTime issueDate)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO licenses(trainer_id, name, document_type, issue_date) " +
                              "VALUES(@trainer_id, @name, @document_type, @issue_date)";

        AddParameter(command, "trainer_id", trainerId);
        AddParameter(command, "name", name);
        AddParameter(command, "document_type", documentType);
        AddParameter(command, "issue_date", issueDate);

        command.ExecuteNonQuery();
    }
    // OnlineGym.Application.Database.Repositories.TrainerRepository.cs

    public Trainer? GetTrainerByAccountId(long accountId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText =
            @"SELECT trainer_id, account_id, first_name, last_name, specialization, average_rating, education, recommendations 
                            FROM trainers WHERE account_id = @account_id";
        AddParameter(command, "account_id", accountId);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Trainer(reader.GetInt64(0), reader.GetInt64(1), reader.GetString(2), reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4), reader.GetDouble(5),
                reader.IsDBNull(6) ? null : reader.GetString(6),
                reader.IsDBNull(7) ? null : reader.GetString(7)
            );
        }

        return null;
    }

    public string? GetRegistrationStatus(long trainerId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT status FROM registration_requests WHERE trainer_id = @trainer_id";
        AddParameter(command, "trainer_id", trainerId);
        object? result = command.ExecuteScalar();
        if (result == null || result==DBNull.Value)
        {
            return null;
        }
        return result.ToString();
    }
    public bool DeleteTrainer(long trainerId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM trainers WHERE trainer_id = @trainer_id";
        AddParameter(command, "trainer_id", trainerId);
    
        int result = command.ExecuteNonQuery();
        return result > 0;
    }
    public bool DeleteTrainerAccount(long accountId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM user_accounts WHERE account_id = @account_id";
        AddParameter(command, "account_id", accountId);
    
        int result = command.ExecuteNonQuery();
        return result > 0;
    }

    private void AddParameter(IDbCommand command, string paramName, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = "@" + paramName;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
    
    public Trainer? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT trainer_id,
               account_id,
               first_name,
               last_name,
               specialization,
               average_rating,
               education,
               recommendations
        FROM trainers
        WHERE trainer_id = @trainer_id;";

        AddParameter(command, "trainer_id", id);

        using IDataReader reader = command.ExecuteReader();

        return reader.Read() ? MapFromReader(reader) : null;
    }
    private Trainer MapFromReader(IDataReader reader)
    {
        int specializationIndex = reader.GetOrdinal("specialization");
        int educationIndex = reader.GetOrdinal("education");
        int recommendationsIndex = reader.GetOrdinal("recommendations");

        return new Trainer(
            reader.GetInt64(reader.GetOrdinal("trainer_id")),
            reader.GetInt64(reader.GetOrdinal("account_id")),
            reader.GetString(reader.GetOrdinal("first_name")),
            reader.GetString(reader.GetOrdinal("last_name")),
            reader.IsDBNull(specializationIndex)
                ? null
                : reader.GetString(specializationIndex),
            Convert.ToDouble(
                reader.GetDecimal(reader.GetOrdinal("average_rating"))),
            reader.IsDBNull(educationIndex)
                ? null
                : reader.GetString(educationIndex),
            reader.IsDBNull(recommendationsIndex)
                ? null
                : reader.GetString(recommendationsIndex)
        );
    }
    public List<Trainer> GetApprovedTrainers()
    {
        List<Trainer> trainers = new();
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
        SELECT DISTINCT
            t.trainer_id,
            t.account_id,
            t.first_name,
            t.last_name,
            t.specialization,
            t.average_rating,
            t.education,
            t.recommendations
        FROM trainers t
        INNER JOIN registration_requests rr
            ON rr.trainer_id = t.trainer_id
        WHERE rr.status = 'APPROVED'
        ORDER BY t.first_name, t.last_name;";
        using IDataReader reader = command.ExecuteReader();
        while (reader.Read()) trainers.Add(MapFromReader(reader));
        return trainers;
    }
}