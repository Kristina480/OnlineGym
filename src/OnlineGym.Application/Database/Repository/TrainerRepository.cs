using System.Data;
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

    private void AddParameter(IDbCommand command, string paramName, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = "@" + paramName;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}