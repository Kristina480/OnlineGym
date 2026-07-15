using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface ICollaborationRepository
{
    long Create(Collaboration collaboration);

    Collaboration? GetById(long id);

    Collaboration? GetActiveByClientAndTrainer(
        long clientId,
        long trainerId);

    bool HasActiveCollaboration(
        long clientId,
        long trainerId);
}