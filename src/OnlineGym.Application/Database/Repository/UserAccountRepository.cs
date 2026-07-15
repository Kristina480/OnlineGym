using System.Data;
namespace OnlineGym.Application.Database.Repositories;

public class UserAccountRepository
{
    public long? GetIdByCredentials(string username, string password)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText =
            "SELECT account_id FROM user_accounts " +
            "WHERE username = @username AND password = @password";
        
        AddParameter(command, "username", username);
        AddParameter(command, "password", password);
        
        object? result = command.ExecuteScalar();
        if (result != null)
        {
            return Convert.ToInt64(result);
        }
        return null;
    }


    public string? GetUserTypeById(long accountId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText =
            "SELECT user_type FROM user_accounts " +
            "WHERE account_id = @account_id";

        AddParameter(command, "account_id", accountId);
        
        object? result = command.ExecuteScalar();
        if (result != null)
        {
            return result.ToString();
        }
        return null;
    }
    private void AddParameter(IDbCommand command, string name, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();

        parameter.ParameterName = "@" + name;
        parameter.Value = value;

        command.Parameters.Add(parameter);
    }
}