using OnlineGym.Application.Domain;
using OnlineGym.Application.Domain.Enums;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface ICollaborationRequestRepository
{
    bool Insert(CollaborationRequest request);

    CollaborationRequest? GetById(long id);

    List<CollaborationRequest> GetByTrainerId(long trainerId);

    List<CollaborationRequest> GetByTrainerIdAndStatus(
        long trainerId,
        RequestStatus status);

    bool HasPendingRequest(long clientId, long trainerId);

    void Update(CollaborationRequest request);
}