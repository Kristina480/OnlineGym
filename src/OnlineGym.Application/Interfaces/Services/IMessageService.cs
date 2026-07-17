using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Services;

public interface IMessageService
{
    long SendMessage(
        long senderAccountId,
        long recipientAccountId,
        string content);

    List<Message> GetReceivedMessages(long recipientAccountId);
    void MarkAllAsRead(long recipientAccountId);
}
