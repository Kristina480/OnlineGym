namespace OnlineGym.Application.Domain;

public class Message
{
    public long Id { get; set; }
    public long SenderAccountId { get; set; }
    public long RecipientAccountId { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
    public string SenderName { get; set; } = string.Empty;


    public Message(
        long id,
        long senderAccountId,
        long recipientAccountId,
        string content,
        bool isRead,
        DateTime sentAt)
    {
        Id = id;
        SenderAccountId = senderAccountId;
        RecipientAccountId = recipientAccountId;
        Content = content;
        IsRead = isRead;
        SentAt = sentAt;
    }
}