using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;
using OnlineGym.Application.Interfaces.Services;

namespace OnlineGym.Application.Services;

public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public long SendMessage(
            long senderAccountId,
            long recipientAccountId,
            string content)
        {
            if (senderAccountId <= 0)
                throw new ArgumentException("Sender account ID is invalid.");

            if (recipientAccountId <= 0)
                throw new ArgumentException("Recipient account ID is invalid.");

            if (senderAccountId == recipientAccountId)
                throw new InvalidOperationException(
                    "A user cannot send a message to themselves.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Message content is required.");

            Message message = new Message(
                0,
                senderAccountId,
                recipientAccountId,
                content.Trim(),
                false,
                DateTime.Now
            );

            return _messageRepository.Insert(message);
        }
}