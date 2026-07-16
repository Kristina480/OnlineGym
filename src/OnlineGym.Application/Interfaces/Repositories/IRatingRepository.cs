using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface IRatingRepository
{
    long Insert(Rating rating);

    void UpdateTrainerAverageRating(long trainerId);
}