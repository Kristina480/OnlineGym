using System.Data;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Domain.Enums;

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
     public CollaborationRequest? GetById(long id)
     {
         using IDbConnection connection = PostgresConnection.CreateConnection();
         IDbCommand command = connection.CreateCommand();

         command.CommandText = @"
        SELECT *
        FROM collaboration_requests
        WHERE request_id = @request_id;";

         DataBaseHelper.AddParameter(command, "@request_id", id);

         using IDataReader reader = command.ExecuteReader();

         if (reader.Read())
         {
             return MapFromReader(reader);
         }

         return null;
     }
     
     private CollaborationRequest MapFromReader(IDataReader reader)
     {
         return new CollaborationRequest(
             reader.GetInt64(reader.GetOrdinal("request_id")),
             reader.GetInt64(reader.GetOrdinal("client_id")),
             reader.GetInt64(reader.GetOrdinal("trainer_id")),
             reader.GetDateTime(reader.GetOrdinal("request_date")),
             Enum.Parse<RequestStatus>(
                 reader.GetString(reader.GetOrdinal("status")),
                 true)
         );
     }
 }