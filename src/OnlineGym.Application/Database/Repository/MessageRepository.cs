using System.Data;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;

namespace OnlineGym.Application.Database.Repositories;

public class MessageRepository: IMessageRepository
{
    public long Insert(Message message)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO messages
        (sender_account_id, recipient_account_id, content, is_read, sent_at)
        VALUES
        (@sender_account_id, @recipient_account_id, @content, @is_read, @sent_at)
        RETURNING message_id;";

        DataBaseHelper.AddParameter(command, "@sender_account_id", message.SenderAccountId);
        DataBaseHelper.AddParameter(command, "@recipient_account_id", message.RecipientAccountId);
        DataBaseHelper.AddParameter(command, "@content", message.Content);
        DataBaseHelper.AddParameter(command, "@is_read", message.IsRead);
        DataBaseHelper.AddParameter(command, "@sent_at", message.SentAt);

        object? result = command.ExecuteScalar();

        if (result is null || result == DBNull.Value)
            throw new InvalidOperationException("Message was not created.");

        return Convert.ToInt64(result);
    }

    public List<Message> GetByRecipientAccountId(long recipientAccountId)
    {
        List<Message> messages = new();

        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT message_id, sender_account_id, recipient_account_id,
               content, is_read, sent_at
        FROM messages
        WHERE recipient_account_id = @recipient_account_id
        ORDER BY sent_at DESC;";

        DataBaseHelper.AddParameter(command, "@recipient_account_id", recipientAccountId);

        using IDataReader reader = command.ExecuteReader();

        while (reader.Read())
            messages.Add(MapFromReader(reader));

        return messages;
    }

    private Message MapFromReader(IDataReader reader)
    {
        return new Message(
            reader.GetInt64(reader.GetOrdinal("message_id")),
            reader.GetInt64(reader.GetOrdinal("sender_account_id")),
            reader.GetInt64(reader.GetOrdinal("recipient_account_id")),
            reader.GetString(reader.GetOrdinal("content")),
            reader.GetBoolean(reader.GetOrdinal("is_read")),
            reader.GetDateTime(reader.GetOrdinal("sent_at"))
        );
    }
}