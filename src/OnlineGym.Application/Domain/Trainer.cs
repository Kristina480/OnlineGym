namespace OnlineGym.Application.Domain;

public class Trainer
{
    public long TrainerId { get; set; }
    public long AccountId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Specialization { get; set; }
    public double AverageRating { get; set; }
    public string? Education { get; set; }
    public string? Recommendations { get; set; }

    public Trainer(long trainerId, long accountId, string firstName, string lastName, string? specialization,
                   double averageRating, string? education, string? recommendations)
    {
        TrainerId = trainerId;
        AccountId = accountId;
        FirstName = firstName;
        LastName = lastName;
        Specialization = specialization;
        AverageRating = averageRating;
        Education = education;
        Recommendations = recommendations;
    }
}