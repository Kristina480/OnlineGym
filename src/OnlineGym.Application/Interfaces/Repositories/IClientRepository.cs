using System.Data;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface IClientRepository
{
    public bool UsernameExists(string username);
    public long SaveUserAccount(string username, string password);

    public void SaveClient(long accountId, string firstName, string lastName, double height, double weight,
        string? goal, string? healthIssues, int workoutsPerWeek);

    public void RegisterClient(string username, string password, string firstName, string lastName, double height,
        double weight, string? goal, string? healthIssues, int workoutsPerWeek);


}