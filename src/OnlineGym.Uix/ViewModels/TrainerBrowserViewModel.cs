using System.Collections.Generic;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Services;
using OnlineGym.Application.Services;

namespace OnlineGym.Uix.ViewModels;

public class TrainerBrowserViewModel
{
    private readonly long _clientId;
    private readonly ICollaborationRequestService _service;

    public TrainerBrowserViewModel(long clientId)
    {
        _clientId = clientId;

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

    public List<Trainer> GetAvailableTrainers()
    {
        return _service.GetAvailableTrainers(_clientId);
    }

    public void SendRequest(long trainerId)
    {
        _service.SendRequest(_clientId, trainerId);
    }
}
