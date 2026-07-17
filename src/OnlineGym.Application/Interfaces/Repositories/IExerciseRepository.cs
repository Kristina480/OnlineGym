using System.Collections.Generic;
using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface IExerciseRepository
{
    long Insert(Exercise exercise);

    Exercise? GetById(long id);

    List<Exercise> GetByTrainerId(long trainerId);

    bool ExistsByIdAndTrainerId(
        long exerciseId,
        long trainerId);

    bool IsUsedInWorkout(long exerciseId);

    bool EquipmentExists(long equipmentId);

    bool MachineExists(long machineId);

    void Update(Exercise exercise);

    void Delete(long id);
}
