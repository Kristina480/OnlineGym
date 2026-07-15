namespace OnlineGym.Application.Domain;

public class Client
{
    public long ClientId { get; set; }
    public long AccountId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
    public string? Goal { get; set; }
    public string? HealthIssues { get; set; }
    public int WorkoutsPerWeek { get; set; }

    public Client(long clientId, long accountId, string firstName, string lastName, double height, double weight,
        string? goal, string? healthIssues, int workoutsPerWeek)
    {
        ClientId = clientId;
        AccountId = accountId;
        FirstName = firstName;
        LastName = lastName;
        Height = height;
        Weight = weight;
        Goal = goal;
        HealthIssues = healthIssues;
        WorkoutsPerWeek = workoutsPerWeek;
    }
}