using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface ITrainerRepository
{
    public bool UsernameExists(string username);
    public long SaveUserAccount(string username, string password);

    public long SaveTrainer(long accountId, string firstName, string lastName, string? specialization,
        string? education, string? recommendations);

    public void CreateRegistrationRequest(long trainerId);

    public void RegisterTrainer(string username, string password, string firstName, string lastName,
        string? specialization, string? education, string? recommendations);
    Trainer? GetById(long id);
    List<Trainer> GetApprovedTrainers();
    
}