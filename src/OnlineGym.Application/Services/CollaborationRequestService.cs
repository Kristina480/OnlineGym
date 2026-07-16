using OnlineGym.Application.Domain;
using OnlineGym.Application.Domain.Enums;
using OnlineGym.Application.Interfaces.Repositories;

namespace OnlineGym.Application.Services;

public class CollaborationRequestService
{
    private readonly ICollaborationRequestRepository _requestRepository;
    private readonly ICollaborationRepository _collaborationRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ITrainerRepository _trainerRepository;
    private readonly IPricingPackageRepository _pricingPackageRepository;

    public CollaborationRequestService(
        ICollaborationRequestRepository requestRepository,
        ICollaborationRepository collaborationRepository,
        IClientRepository clientRepository,
        ITrainerRepository trainerRepository,
        IPricingPackageRepository pricingPackageRepository)
    {
        _requestRepository = requestRepository;
        _collaborationRepository = collaborationRepository;
        _clientRepository = clientRepository;
        _trainerRepository = trainerRepository;
        _pricingPackageRepository = pricingPackageRepository;
    }

    public bool SendRequest(long clientId, long trainerId)
    {
        if (_clientRepository.GetById(clientId) is null)
            throw new InvalidOperationException(
                $"Client with ID {clientId} does not exist.");

        if (_trainerRepository.GetById(trainerId) is null)
            throw new InvalidOperationException(
                $"Trainer with ID {trainerId} does not exist.");

        if (_requestRepository.HasPendingRequest(clientId, trainerId))
            throw new InvalidOperationException(
                "A pending request for this trainer already exists.");

        if (_collaborationRepository.HasActiveCollaboration(clientId, trainerId))
            throw new InvalidOperationException(
                "YOU already has an active collaboration with this trainer.");

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
}