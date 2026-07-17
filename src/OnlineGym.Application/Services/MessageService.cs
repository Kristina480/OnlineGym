using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;
using OnlineGym.Application.Interfaces.Services;

namespace OnlineGym.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;

    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository =
            messageRepository ??
            throw new ArgumentNullException(
                nameof(messageRepository));
    }

    public long SendMessage(
        long senderAccountId,
        long recipientAccountId,
        string content)
    {
        ValidateAccountId(
            senderAccountId,
            "Pošiljalac");

        ValidateAccountId(
            recipientAccountId,
            "Primalac");

        if (senderAccountId == recipientAccountId)
        {
            throw new InvalidOperationException(
                "Korisnik ne može poslati poruku samom sebi.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException(
                "Sadržaj poruke je obavezan.");
        }

        Message message = new Message(
            0,
            senderAccountId,
            recipientAccountId,
            content.Trim(),
            false,
            DateTime.Now);

        return _messageRepository.Insert(message);
    }

    public List<Message> GetReceivedMessages(
        long recipientAccountId)
    {
        ValidateAccountId(
            recipientAccountId,
            "Primalac");

        return _messageRepository
            .GetByRecipientAccountId(
                recipientAccountId);
    }

    public void MarkAllAsRead(
        long recipientAccountId)
    {
        ValidateAccountId(
            recipientAccountId,
            "Primalac");

        _messageRepository
            .MarkAllAsRead(recipientAccountId);
    }

    private static void ValidateAccountId(
        long accountId,
        string field)
    {
        if (accountId <= 0)
        {
            throw new ArgumentException(
                $"{field} nema ispravan ID naloga.");
        }
    }
}
