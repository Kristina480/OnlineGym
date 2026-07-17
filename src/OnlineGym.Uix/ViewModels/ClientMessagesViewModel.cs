using System.Collections.Generic;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Services;
using OnlineGym.Application.Services;

namespace OnlineGym.Uix.ViewModels;

public class ClientMessagesViewModel
{
    private readonly long _accountId;
    private readonly IMessageService _messageService;
    private readonly TrainerRepository _trainerRepository = new();
    private readonly ClientRepository _clientRepository = new();

    public ClientMessagesViewModel(long accountId)
    {
        _accountId = accountId;
        _messageService =
            new MessageService(
                new MessageRepository());
    }

    public List<Message> GetMessages()
    {
        List<Message> messages =
            _messageService.GetReceivedMessages(_accountId);

        Dictionary<long, string> senderNames = new();

        foreach (Message message in messages)
        {
            if (!senderNames.TryGetValue(
                    message.SenderAccountId,
                    out string? senderName))
            {
                senderName =
                    ResolveSenderName(message.SenderAccountId);

                senderNames[message.SenderAccountId] =
                    senderName;
            }

            message.SenderName = senderName;
        }

        return messages;
    }

    private string ResolveSenderName(long accountId)
    {
        Trainer? trainer =
            _trainerRepository.GetTrainerByAccountId(accountId);

        if (trainer is not null)
        {
            return $"{trainer.FirstName} {trainer.LastName}";
        }

        Client? client =
            _clientRepository.GetClientByAccountId(accountId);

        if (client is not null)
        {
            return $"{client.FirstName} {client.LastName}";
        }

        return "Nepoznat pošiljalac";
    }

    public void MarkAllAsRead()
    {
        _messageService.MarkAllAsRead(_accountId);
    }
}
