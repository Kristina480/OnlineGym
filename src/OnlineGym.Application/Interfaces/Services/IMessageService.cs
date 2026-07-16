namespace OnlineGym.Application.Interfaces.Services;

public interface IMessageService
{
    long SendMessage(
        long senderAccountId,
        long recipientAccountId,
        string content);
}