using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface IWorkoutRepository
{
    long Insert(Workout workout);

    Workout? GetById(long id);

    List<Workout> GetByCollaborationId(long collaborationId);

    List<Workout> GetByClientId(long clientId);

    void Update(Workout workout);
}