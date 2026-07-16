using System.Data;
using OnlineGym.Application.Domain;
namespace OnlineGym.Application.Database.Repositories;

public class ClientRepository
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
                              "VALUES(@username,@password,'CLIENT') " +
                              "RETURNING account_id";
        AddParameter(command, "username", username);
        AddParameter(command, "password", password);

        object? result = command.ExecuteScalar();
        return Convert.ToInt64(result);
    }

    public void SaveClient(long accountId, string firstName, string lastName, double height, double weight, string? goal, string? healthIssues, int workoutsPerWeek)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = "INSERT INTO clients(account_id, first_name, last_name, height, weight, goal, health_issues, workouts_per_week) " +
                              "VALUES(@account_id,@first_name,@last_name,@height,@weight,@goal,@health_issues,@workouts_per_week)";

        AddParameter(command, "account_id", accountId);
        AddParameter(command, "first_name", firstName);
        AddParameter(command, "last_name", lastName);
        AddParameter(command, "height", height);
        AddParameter(command, "weight", weight);
        AddParameter(command, "goal", goal ?? (object)DBNull.Value);
        AddParameter(command, "health_issues", healthIssues ?? (object)DBNull.Value);
        AddParameter(command, "workouts_per_week", workoutsPerWeek);

        command.ExecuteNonQuery();
    }

    public void RegisterClient(string username, string password, string firstName, string lastName, double height, double weight, string? goal, string? healthIssues, int workoutsPerWeek)
    {
        long accountId = SaveUserAccount(username, password);
        SaveClient(accountId, firstName, lastName, height, weight, goal, healthIssues, workoutsPerWeek);
    }

    public Client? GetClientByAccountId(long accountId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT client_id, account_id, first_name, last_name,height, weight, goal, health_issues, workouts_per_week " +
                              "FROM clients WHERE account_id = @account_id";
        AddParameter(command, "account_id", accountId);
        using var reader=command.ExecuteReader();
        if (reader.Read())
        {
            return new Client(reader.GetInt64(0),reader.GetInt64(1),reader.GetString(2),reader.GetString(3),
                reader.GetDouble(4), reader.GetDouble(5),reader.IsDBNull(6) ? null : reader.GetString(6),
                reader.IsDBNull(7) ? null : reader.GetString(7),reader.GetInt32(8));
        }
        return null;
    }
    public void UpdateClient(long clientId, string firstName, string lastName, double height, double weight, string? goal, string? healthIssues, int workoutsPerWeek)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"UPDATE clients SET first_name = @first_name, last_name = @last_name, height = @height,weight=@weight,goal=@goal,
                   health_issues = @health_issues, workouts_per_week = @workouts_per_week WHERE client_id = @client_id";
    
        AddParameter(command, "client_id", clientId);
        AddParameter(command, "first_name", firstName);
        AddParameter(command, "last_name", lastName);
        AddParameter(command, "height", height);
        AddParameter(command, "weight", weight);
        AddParameter(command, "goal", goal ?? (object)DBNull.Value);
        AddParameter(command, "health_issues", healthIssues ?? (object)DBNull.Value);
        AddParameter(command, "workouts_per_week", workoutsPerWeek);
    
        command.ExecuteNonQuery();
    }

    private void AddParameter(IDbCommand command, string paramName, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = "@" + paramName;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}