using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface IMessageRepository
{
    long Insert(Message message);
    List<Message> GetByRecipientAccountId(long recipientAccountId);

}