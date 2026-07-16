using System.Data;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;

namespace OnlineGym.Application.Database.Repositories;

public class ClientRepository:IClientRepository
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

    private void AddParameter(IDbCommand command, string paramName, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = "@" + paramName;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
    public Client? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT client_id,account_id,first_name,last_name,height,weight,goal,health_issues,workouts_per_week
        FROM clients
        WHERE client_id = @client_id;";

        AddParameter(command, "client_id", id);

        using IDataReader reader = command.ExecuteReader();

        return reader.Read() ? MapFromReader(reader) : null;
    }
    private Client MapFromReader(IDataReader reader)
    {
        int goalIndex = reader.GetOrdinal("goal");
        int healthIssuesIndex = reader.GetOrdinal("health_issues");

        return new Client(
            reader.GetInt64(reader.GetOrdinal("client_id")),
            reader.GetInt64(reader.GetOrdinal("account_id")),
            reader.GetString(reader.GetOrdinal("first_name")),
            reader.GetString(reader.GetOrdinal("last_name")),
            Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("height"))),
            Convert.ToDouble(reader.GetDecimal(reader.GetOrdinal("weight"))),
            reader.IsDBNull(goalIndex)
                ? null
                : reader.GetString(goalIndex),
            reader.IsDBNull(healthIssuesIndex)
                ? null
                : reader.GetString(healthIssuesIndex),
            reader.GetInt32(reader.GetOrdinal("workouts_per_week"))
        );
    }
}