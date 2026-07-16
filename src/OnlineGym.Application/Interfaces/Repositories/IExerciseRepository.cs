using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface IExerciseRepository
{
    long Insert(Exercise exercise);
    Exercise? GetById(long id);
    List<Exercise> GetByTrainerId(long trainerId);
    bool ExistsByIdAndTrainerId(long exerciseId, long trainerId);
    void Update(Exercise exercise);
    void Delete(long id);
}