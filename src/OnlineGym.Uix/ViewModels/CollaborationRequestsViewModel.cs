using System.Collections.Generic;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Services;
using OnlineGym.Application.Services;

namespace OnlineGym.Uix.ViewModels;

public class CollaborationRequestsViewModel
{
    private readonly long _trainerId;
    private readonly ICollaborationRequestService _service;
    private readonly PricingPackageRepository _pricingPackageRepository = new();

    public CollaborationRequestsViewModel(long trainerId)
    {
        _trainerId = trainerId;

        IMessageService messageService =
            new MessageService(new MessageRepository());

        _service = new CollaborationRequestService(
            new CollaborationRequestRepository(),
            new CollaborationRepository(),
            new ClientRepository(),
            new TrainerRepository(),
            new PricingPackageRepository(),
            messageService);
    }

    public List<CollaborationRequest> GetPendingRequests()
    {
        return _service.GetPendingRequests(_trainerId);
    }

    public List<PricingPackage> GetPricingPackages()
    {
        return _pricingPackageRepository.GetByTrainerId(_trainerId);
    }

    public long ApproveRequest(long requestId, long pricingPackageId)
    {
        return _service.ApproveRequest(
            requestId,
            _trainerId,
            pricingPackageId);
    }

    public void RejectRequest(long requestId)
    {
        _service.RejectRequest(requestId, _trainerId);
    }
}
