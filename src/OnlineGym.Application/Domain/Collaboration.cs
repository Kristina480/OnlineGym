using OnlineGym.Application.Domain.Enums;

namespace OnlineGym.Application.Domain;

public class Collaboration
{
    public long Id { get; set; }

    public long TrainerId { get; set; }

    public long ClientId { get; set; }

    public long? RequestId { get; set; }

    public long PricingPackageId { get; set; }

    public DateTime StartDate { get;  set; }

    public DateTime? EndDate { get; set; }

    public CollaborationStatus Status { get; set; }

    public int WorkoutsPerWeek { get; set; }

    public decimal MonthlyPrice { get; set; }

    public Collaboration(
        long id,
        long trainerId,
        long clientId,
        long? requestId,
        long pricingPackageId,
        DateTime startDate,
        DateTime? endDate,
        CollaborationStatus status,
        int workoutsPerWeek,
        decimal monthlyPrice)
    {
        Id = id;
        TrainerId = trainerId;
        ClientId = clientId;
        RequestId = requestId;
        PricingPackageId = pricingPackageId;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        WorkoutsPerWeek = workoutsPerWeek;
        MonthlyPrice = monthlyPrice;
    }
}