using System;
using System.Collections.Generic;
using System.Linq;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Domain.Enums;
using OnlineGym.Application.Interfaces.Repositories;
using OnlineGym.Application.Interfaces.Services;

namespace OnlineGym.Application.Services;

public class CollaborationRequestService
    : ICollaborationRequestService
{
    private readonly ICollaborationRequestRepository
        _requestRepository;

    private readonly ICollaborationRepository
        _collaborationRepository;

    private readonly IClientRepository
        _clientRepository;

    private readonly ITrainerRepository
        _trainerRepository;

    private readonly IPricingPackageRepository
        _pricingPackageRepository;

    private readonly IMessageService
        _messageService;

    public CollaborationRequestService(
        ICollaborationRequestRepository requestRepository,
        ICollaborationRepository collaborationRepository,
        IClientRepository clientRepository,
        ITrainerRepository trainerRepository,
        IPricingPackageRepository pricingPackageRepository,
        IMessageService messageService)
    {
        _requestRepository =
            requestRepository ??
            throw new ArgumentNullException(
                nameof(requestRepository));

        _collaborationRepository =
            collaborationRepository ??
            throw new ArgumentNullException(
                nameof(collaborationRepository));

        _clientRepository =
            clientRepository ??
            throw new ArgumentNullException(
                nameof(clientRepository));

        _trainerRepository =
            trainerRepository ??
            throw new ArgumentNullException(
                nameof(trainerRepository));

        _pricingPackageRepository =
            pricingPackageRepository ??
            throw new ArgumentNullException(
                nameof(pricingPackageRepository));

        // OVA DODELA JE KLJUČNA.
        _messageService =
            messageService ??
            throw new ArgumentNullException(
                nameof(messageService));
    }

    public bool SendRequest(
        long clientId,
        long trainerId)
    {
        EnsureClientAndTrainerExist(
            clientId,
            trainerId);

        if (_requestRepository.HasPendingRequest(
                clientId,
                trainerId))
        {
            throw new InvalidOperationException(
                "Već postoji zahtev na čekanju za ovog trenera.");
        }

        if (_collaborationRepository
            .HasActiveCollaboration(
                clientId,
                trainerId))
        {
            throw new InvalidOperationException(
                "Već imate aktivnu saradnju sa ovim trenerom.");
        }

        CollaborationRequest request =
            new CollaborationRequest(
                0,
                clientId,
                trainerId,
                DateTime.Today,
                RequestStatus.Pending);

        return _requestRepository.Insert(request);
    }

    public List<Trainer> GetAvailableTrainers(
        long clientId)
    {
        if (_clientRepository.GetById(clientId)
            is null)
        {
            throw new InvalidOperationException(
                "Klijent ne postoji.");
        }

        return _trainerRepository
            .GetApprovedTrainers()
            .Where(trainer =>
                !_requestRepository.HasPendingRequest(
                    clientId,
                    trainer.TrainerId) &&
                !_collaborationRepository.HasActiveCollaboration(
                    clientId,
                    trainer.TrainerId))
            .ToList();
    }

    public List<CollaborationRequest>
        GetPendingRequests(long trainerId)
    {
        if (_trainerRepository.GetById(trainerId)
            is null)
        {
            throw new InvalidOperationException(
                "Trener ne postoji.");
        }

        List<CollaborationRequest> requests =
            _requestRepository
                .GetByTrainerIdAndStatus(
                    trainerId,
                    RequestStatus.Pending);

        foreach (CollaborationRequest request in requests)
        {
            Client? client =
                _clientRepository.GetById(request.ClientId);

            request.ClientName = client is null
                ? "Nepoznat klijent"
                : $"{client.FirstName} {client.LastName}";
        }

        return requests;
    }

    public long ApproveRequest(
        long requestId,
        long trainerId,
        long pricingPackageId)
    {
        CollaborationRequest request =
            GetPendingRequestForTrainer(
                requestId,
                trainerId);

        if (_collaborationRepository
            .HasActiveCollaboration(
                request.ClientId,
                trainerId))
        {
            throw new InvalidOperationException(
                "Aktivna saradnja već postoji.");
        }

        PricingPackage? pricingPackage =
            _pricingPackageRepository
                .GetById(pricingPackageId);

        if (pricingPackage is null)
        {
            throw new InvalidOperationException(
                "Paket cena ne postoji.");
        }

        if (pricingPackage.TrainerId
            != trainerId)
        {
            throw new InvalidOperationException(
                "Izabrani paket ne pripada ovom treneru.");
        }

        Collaboration collaboration =
            new Collaboration(
                0,
                trainerId,
                request.ClientId,
                request.Id,
                pricingPackage.Id,
                DateTime.Today,
                null,
                CollaborationStatus.Active,
                pricingPackage.WorkoutsPerWeek,
                pricingPackage.MonthlyPrice);

        long collaborationId =
            _collaborationRepository
                .Insert(collaboration);

        request.Status =
            RequestStatus.Approved;

        _requestRepository.Update(request);

        TrySendDecisionMessage(
            request.ClientId,
            trainerId,
            requestApproved: true);

        return collaborationId;
    }

    public void RejectRequest(
        long requestId,
        long trainerId)
    {
        CollaborationRequest request =
            GetPendingRequestForTrainer(
                requestId,
                trainerId);

        request.Status =
            RequestStatus.Rejected;

        _requestRepository.Update(request);

        TrySendDecisionMessage(
            request.ClientId,
            trainerId,
            requestApproved: false);
    }

    private void EnsureClientAndTrainerExist(
        long clientId,
        long trainerId)
    {
        if (_clientRepository.GetById(clientId)
            is null)
        {
            throw new InvalidOperationException(
                "Klijent ne postoji.");
        }

        if (_trainerRepository.GetById(trainerId)
            is null)
        {
            throw new InvalidOperationException(
                "Trener ne postoji.");
        }
    }

    private CollaborationRequest
        GetPendingRequestForTrainer(
            long requestId,
            long trainerId)
    {
        CollaborationRequest? request =
            _requestRepository.GetById(requestId);

        if (request is null)
        {
            throw new InvalidOperationException(
                "Zahtev ne postoji.");
        }

        if (request.TrainerId != trainerId)
        {
            throw new InvalidOperationException(
                "Zahtev ne pripada ovom treneru.");
        }

        if (request.Status
            != RequestStatus.Pending)
        {
            throw new InvalidOperationException(
                "Samo zahtev na čekanju može biti obrađen.");
        }

        return request;
    }

    private void TrySendDecisionMessage(
        long clientId,
        long trainerId,
        bool requestApproved)
    {
        try
        {
            Trainer trainer =
                _trainerRepository.GetById(trainerId)
                ?? throw new InvalidOperationException(
                    "Trener ne postoji.");

            Client client =
                _clientRepository.GetById(clientId)
                ?? throw new InvalidOperationException(
                    "Klijent ne postoji.");

            string content = requestApproved
                ? $"Vaš zahtev za saradnju sa trenerom {trainer.FirstName} {trainer.LastName} je prihvaćen."
                : $"Vaš zahtev za saradnju sa trenerom {trainer.FirstName} {trainer.LastName} je odbijen.";

            _messageService.SendMessage(
                trainer.AccountId,
                client.AccountId,
                content);
        }
        catch (Exception exception)
        {
            // Odluka je već sačuvana. Greška u slanju
            // poruke ne sme da vrati zahtev u pogrešno stanje
            // niti da korisniku prikaže NullReferenceException.
            Console.Error.WriteLine(
                "Zahtev je obrađen, ali poruka nije poslata:");
            Console.Error.WriteLine(exception);
        }
    }
}
