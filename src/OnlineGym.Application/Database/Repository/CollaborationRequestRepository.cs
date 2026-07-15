using System.Data;
using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Database.Repositories;

public class CollaborationRequestRepository
{
     public bool Insert(CollaborationRequest request)
     {
         using IDbConnection connection = PostgresConnection.CreateConnection();
         IDbCommand command = connection.CreateCommand();
         command.CommandText = @"INSERT INTO collaboration_requests
(client_id, trainer_id, request_date, status)
VALUES
(@client_id, @trainer_id, @request_date,
 @status::request_status_enum);";

         DataBaseHelper.AddParameter(command, "@client_id", request.ClientId);
         DataBaseHelper.AddParameter(command, "@trainer_id", request.TrainerId);
         DataBaseHelper.AddParameter(command, "@request_date", request.RequestDate);
         DataBaseHelper.AddParameter(command, "@status", request.Status.ToString().ToUpper());
        
         // check if row was added - successful insert
         int rowsAffected = command.ExecuteNonQuery();
         return rowsAffected > 0;
     }
 }