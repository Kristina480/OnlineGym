using OnlineGym.Application.Domain;
using OnlineGym.Application.Domain.Enums;
using OnlineGym.Application.Interfaces.Repositories;
using OnlineGym.Application.Interfaces.Services;

namespace OnlineGym.Application.Services;

public class CollaborationRequestService
{
    private readonly ICollaborationRequestRepository _requestRepository;
    private readonly ICollaborationRepository _collaborationRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ITrainerRepository _trainerRepository;
    private readonly IPricingPackageRepository _pricingPackageRepository;
    private readonly IMessageService _messageService;

    public CollaborationRequestService(
        ICollaborationRequestRepository requestRepository,
        ICollaborationRepository collaborationRepository,
        IClientRepository clientRepository,
        ITrainerRepository trainerRepository,
        IPricingPackageRepository pricingPackageRepository,
            IMessageService messageService)
    {
        _requestRepository = requestRepository;
        _collaborationRepository = collaborationRepository;
        _clientRepository = clientRepository;
        _trainerRepository = trainerRepository;
        _pricingPackageRepository = pricingPackageRepository;
    }

    public bool SendRequest(long clientId, long trainerId)
    {
        EnsureClientAndTrainerExist(clientId, trainerId);

        if (_requestRepository.HasPendingRequest(clientId, trainerId))
            throw new InvalidOperationException(
                "A pending request for this trainer already exists.");

        if (_collaborationRepository.HasActiveCollaboration(
                clientId,
                trainerId))
        {
            throw new InvalidOperationException(
                "The client already has an active collaboration with this trainer.");
        }

        CollaborationRequest request = new CollaborationRequest(
            0,
            clientId,
            trainerId,
            DateTime.Today,
            RequestStatus.Pending
        );

        return _requestRepository.Insert(request);
    }
    public List<CollaborationRequest> GetPendingRequests(long trainerId)
    {
        if (_trainerRepository.GetById(trainerId) is null)
            throw new InvalidOperationException(
                $"Trainer with ID {trainerId} does not exist.");

        return _requestRepository.GetByTrainerIdAndStatus(
            trainerId,
            RequestStatus.Pending
        );
    }
    
     public long ApproveRequest(
        long requestId,
        long trainerId,
        long pricingPackageId)
    {
        CollaborationRequest request =
            GetPendingRequestForTrainer(requestId, trainerId);

        if (_collaborationRepository.HasActiveCollaboration(
                request.ClientId,
                trainerId))
        {
            throw new InvalidOperationException(
                "An active collaboration already exists.");
        }

        PricingPackage? pricingPackage =
            _pricingPackageRepository.GetById(pricingPackageId);

        if (pricingPackage is null)
            throw new InvalidOperationException(
                $"Pricing package with ID {pricingPackageId} does not exist.");

        if (pricingPackage.TrainerId != trainerId)
            throw new InvalidOperationException(
                "The selected pricing package does not belong to this trainer.");

        Collaboration collaboration = new Collaboration(
            0,
            trainerId,
            request.ClientId,
            request.Id,
            pricingPackage.Id,
            DateTime.Today,
            null,
            CollaborationStatus.Active,
            pricingPackage.WorkoutsPerWeek,
            pricingPackage.MonthlyPrice
        );

        long collaborationId =
            _collaborationRepository.Insert(collaboration);

        request.Status = RequestStatus.Approved;
        _requestRepository.Update(request);

        SendDecisionMessage(
            request.ClientId,
            trainerId,
            "Vaš zahtev za saradnju je prihvaćen.");

        return collaborationId;
    }

    public void RejectRequest(long requestId, long trainerId)
    {
        CollaborationRequest request =
            GetPendingRequestForTrainer(requestId, trainerId);

        request.Status = RequestStatus.Rejected;
        _requestRepository.Update(request);

        SendDecisionMessage(
            request.ClientId,
            trainerId,
            "Vaš zahtev za saradnju je odbijen.");
    }

    private void EnsureClientAndTrainerExist(
        long clientId,
        long trainerId)
    {
        if (_clientRepository.GetById(clientId) is null)
            throw new InvalidOperationException(
                $"Client with ID {clientId} does not exist.");

        if (_trainerRepository.GetById(trainerId) is null)
            throw new InvalidOperationException(
                $"Trainer with ID {trainerId} does not exist.");
    }

    private CollaborationRequest GetPendingRequestForTrainer(
        long requestId,
        long trainerId)
    {
        CollaborationRequest? request =
            _requestRepository.GetById(requestId);

        if (request is null)
            throw new InvalidOperationException(
                $"Collaboration request with ID {requestId} does not exist.");

        if (request.TrainerId != trainerId)
            throw new InvalidOperationException(
                "The request does not belong to this trainer.");

        if (request.Status != RequestStatus.Pending)
            throw new InvalidOperationException(
                "Only pending requests can be processed.");

        return request;
    }

    private void SendDecisionMessage(
        long clientId,
        long trainerId,
        string content)
    {
        Trainer trainer = _trainerRepository.GetById(trainerId)
            ?? throw new InvalidOperationException(
                "Trainer was not found.");

        Client client = _clientRepository.GetById(clientId)
            ?? throw new InvalidOperationException(
                "Client was not found.");

        _messageService.SendMessage(
            trainer.AccountId,
            client.AccountId,
            content);
    }
}