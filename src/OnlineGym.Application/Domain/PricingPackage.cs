namespace OnlineGym.Application.Domain;

public class PricingPackage
{
    public long Id { get; set; }
    public long TrainerId { get; set; }
    public int WorkoutsPerWeek { get; set; }
    public decimal MonthlyPrice { get; set; }

    public PricingPackage(long id, long trainerId, int workoutsPerWeek, decimal monthlyPrice)
    {
        Id = id;
        TrainerId = trainerId;
        WorkoutsPerWeek = workoutsPerWeek;
        MonthlyPrice = monthlyPrice;
    }
}