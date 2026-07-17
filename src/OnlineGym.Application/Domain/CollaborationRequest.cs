using OnlineGym.Application.Domain.Enums;

namespace OnlineGym.Application.Domain;

public class CollaborationRequest
{
    public long Id { get; set; }

    public long ClientId { get; set; }

    public long TrainerId { get; set; }

    public DateTime RequestDate { get; private set; }

    public RequestStatus Status { get; set; }
    public string ClientName { get; set; } = string.Empty;

    public CollaborationRequest(
        long id,
        long clientId,
        long trainerId,
        DateTime requestDate,
        RequestStatus status)
    {
        Id = id;
        ClientId = clientId;
        TrainerId = trainerId;
        RequestDate = requestDate;
        Status = status;
    }
}