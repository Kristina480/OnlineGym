using System.Data;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Domain.Enums;
using OnlineGym.Application.Interfaces.Repositories;

namespace OnlineGym.Application.Database.Repositories;

public class CollaborationRequestRepository:ICollaborationRequestRepository
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
     public List<CollaborationRequest> GetByTrainerId(long trainerId)
     {
         List<CollaborationRequest> requests = new();

         using IDbConnection connection = PostgresConnection.CreateConnection();
         IDbCommand command = connection.CreateCommand();

         command.CommandText = @"
        SELECT *
        FROM collaboration_requests
        WHERE trainer_id = @trainer_id;";

         DataBaseHelper.AddParameter(command, "@trainer_id", trainerId);

         using IDataReader reader = command.ExecuteReader();

         while (reader.Read())
         {
             requests.Add(MapFromReader(reader));
         }

         return requests;
     }
     
     public List<CollaborationRequest> GetByTrainerIdAndStatus(
         long trainerId,
         RequestStatus status)
     {
         List<CollaborationRequest> requests = new();

         using IDbConnection connection = PostgresConnection.CreateConnection();
         IDbCommand command = connection.CreateCommand();

         command.CommandText = @"
        SELECT *
        FROM collaboration_requests
        WHERE trainer_id = @trainer_id
          AND status = @status::request_status_enum;";

         DataBaseHelper.AddParameter(command, "@trainer_id", trainerId);
         DataBaseHelper.AddParameter(command, "@status", status.ToString().ToUpper());

         using IDataReader reader = command.ExecuteReader();

         while (reader.Read())
         {
             requests.Add(MapFromReader(reader));
         }

         return requests;
     }
     public bool HasPendingRequest(long clientId, long trainerId)
     {
         using IDbConnection connection = PostgresConnection.CreateConnection();
         IDbCommand command = connection.CreateCommand();

         command.CommandText = @"
        SELECT COUNT(*)
        FROM collaboration_requests
        WHERE client_id = @client_id
          AND trainer_id = @trainer_id
          AND status = 'PENDING'::request_status_enum;";

         DataBaseHelper.AddParameter(command, "@client_id", clientId);
         DataBaseHelper.AddParameter(command, "@trainer_id", trainerId);

         long count = (long)command.ExecuteScalar();

         return count > 0;
     }
     public void Update(CollaborationRequest request)
     {
         using IDbConnection connection = PostgresConnection.CreateConnection();
         IDbCommand command = connection.CreateCommand();

         command.CommandText = @"
        UPDATE collaboration_requests
        SET
            client_id = @client_id,
            trainer_id = @trainer_id,
            request_date = @request_date,
            status = @status::request_status_enum
        WHERE request_id = @request_id;";

         DataBaseHelper.AddParameter(command, "@request_id", request.Id);
         DataBaseHelper.AddParameter(command, "@client_id", request.ClientId);
         DataBaseHelper.AddParameter(command, "@trainer_id", request.TrainerId);
         DataBaseHelper.AddParameter(command, "@request_date", request.RequestDate);
         DataBaseHelper.AddParameter(command, "@status", request.Status.ToString().ToUpper());

         command.ExecuteNonQuery();
     }
     
 }