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
}