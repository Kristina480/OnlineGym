using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Services;

public interface ICollaborationRequestService
{
    bool SendRequest(long clientId, long trainerId);

    List<CollaborationRequest> GetPendingRequests(long trainerId);

    long ApproveRequest(
        long requestId,
        long trainerId,
        long pricingPackageId);

    void RejectRequest(long requestId, long trainerId);
    
    List<Trainer> GetAvailableTrainers(long clientId);
}