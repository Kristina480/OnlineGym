using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface IRatingRepository
{
    long Insert(Rating rating);

    Rating? GetByClientAndTrainer(long clientId, long trainerId);

    void UpdateTrainerAverageRating(long trainerId);
}