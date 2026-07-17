using System.Data;
using OnlineGym.Application.Domain;
namespace OnlineGym.Application.Database.Repositories;

public class RegistrationRequestRepository
{
    public List<Trainer> GetNotRegisteredTrainers()
    {
        IDbConnection connection=PostgresConnection.CreateConnection();
        IDbCommand command=connection.CreateCommand();
        command.CommandText = @"SELECT t.trainer_id, t.account_id, t.first_name, t.last_name,t.specialization, t.average_rating, t.education, t.recommendations
                                FROM trainers t JOIN registration_requests r ON  t.trainer_id = r.trainer_id
                                WHERE r.status='PENDING' ORDER BY t.average_rating DESC";
        
        using var reader=command.ExecuteReader();
        List<Trainer> trainers = new List<Trainer>();
        while (reader.Read())
        {
            trainers.Add(new Trainer(reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.GetDouble(5),
                reader.IsDBNull(6) ? null : reader.GetString(6),
                reader.IsDBNull(7) ? null : reader.GetString(7)));
        }
        return trainers;
    }
    public List<Trainer> GetApprovedTrainers()
    {
        IDbConnection connection=PostgresConnection.CreateConnection();
        IDbCommand command=connection.CreateCommand();
        command.CommandText = @"SELECT t.trainer_id, t.account_id, t.first_name, t.last_name,t.specialization, t.average_rating, t.education, t.recommendations
                                FROM trainers t JOIN registration_requests r ON  t.trainer_id = r.trainer_id
                                WHERE r.status='APPROVED' ORDER BY t.average_rating DESC";
        
        using var reader=command.ExecuteReader();
        List<Trainer> trainers = new List<Trainer>();
        while (reader.Read())
        {
            trainers.Add(new Trainer(reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.GetDouble(5),
                reader.IsDBNull(6) ? null : reader.GetString(6),
                reader.IsDBNull(7) ? null : reader.GetString(7)));
        }
        return trainers;
    }
    
    public bool UpdateStatus(long trainerId,string status)
    {
        IDbConnection connection=PostgresConnection.CreateConnection();
        IDbCommand command=connection.CreateCommand();
        command.CommandText = @"UPDATE registration_requests SET status=@status::request_status_enum WHERE trainer_id=@trainerId";
        AddParameter(command, "trainerId", trainerId);
        AddParameter(command, "status", status);
        
        int result = command.ExecuteNonQuery();
        return result > 0;
    }

    public bool HasLicense(long trainerId)
    {
        IDbConnection connection=PostgresConnection.CreateConnection();
        IDbCommand command=connection.CreateCommand();
        command.CommandText = @"SELECT COUNT(*) FROM licenses where trainer_id=@trainerId";
        AddParameter(command, "trainerId", trainerId);
        long result = Convert.ToInt64(command.ExecuteScalar());
        return result > 0;
    }
    private void AddParameter(IDbCommand command, string paramName, object value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = "@" + paramName;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}