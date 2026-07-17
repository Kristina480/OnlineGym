using System;
using System.Collections.Generic;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;
using OnlineGym.Application.Interfaces.Services;

namespace OnlineGym.Application.Services;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository
        _exerciseRepository;

    private readonly ITrainerRepository
        _trainerRepository;

    public ExerciseService(
        IExerciseRepository exerciseRepository,
        ITrainerRepository trainerRepository)
    {
        _exerciseRepository =
            exerciseRepository ??
            throw new ArgumentNullException(
                nameof(exerciseRepository));

        _trainerRepository =
            trainerRepository ??
            throw new ArgumentNullException(
                nameof(trainerRepository));
    }

    public long CreateExercise(
        Exercise exercise,
        long trainerId)
    {
        EnsureTrainerExists(trainerId);

        if (exercise.TrainerId != trainerId)
        {
            throw new InvalidOperationException(
                "Vežba mora pripadati prijavljenom treneru.");
        }

        ValidateExercise(exercise);

        return _exerciseRepository
            .Insert(exercise);
    }

    public Exercise? GetById(long exerciseId)
    {
        return _exerciseRepository
            .GetById(exerciseId);
    }

    public List<Exercise> GetTrainerExercises(
        long trainerId)
    {
        EnsureTrainerExists(trainerId);

        return _exerciseRepository
            .GetByTrainerId(trainerId);
    }

    public void UpdateExercise(
        Exercise exercise,
        long trainerId)
    {
        EnsureTrainerExists(trainerId);

        Exercise? existingExercise =
            _exerciseRepository
                .GetById(exercise.Id);

        if (existingExercise is null)
        {
            throw new InvalidOperationException(
                "Vežba ne postoji.");
        }

        if (existingExercise.TrainerId
                != trainerId ||
            exercise.TrainerId
                != trainerId)
        {
            throw new InvalidOperationException(
                "Trener može menjati samo svoje vežbe.");
        }

        ValidateExercise(exercise);

        _exerciseRepository.Update(exercise);
    }

    public void DeleteExercise(
        long exerciseId,
        long trainerId)
    {
        EnsureTrainerExists(trainerId);

        Exercise? exercise =
            _exerciseRepository.GetById(exerciseId);

        if (exercise is null)
        {
            throw new InvalidOperationException(
                "Vežba ne postoji.");
        }

        if (exercise.TrainerId != trainerId)
        {
            throw new InvalidOperationException(
                "Trener može obrisati samo svoje vežbe.");
        }

        if (_exerciseRepository
            .IsUsedInWorkout(exerciseId))
        {
            throw new InvalidOperationException(
                "Vežba se ne može obrisati jer je već korišćena u treningu.");
        }

        _exerciseRepository.Delete(exerciseId);
    }

    private void EnsureTrainerExists(
        long trainerId)
    {
        if (_trainerRepository.GetById(trainerId)
            is null)
        {
            throw new InvalidOperationException(
                "Trener ne postoji.");
        }
    }

    private void ValidateExercise(
        Exercise exercise)
    {
        if (string.IsNullOrWhiteSpace(
                exercise.Name))
        {
            throw new ArgumentException(
                "Naziv vežbe je obavezan.");
        }

        if (exercise.EquipmentId.HasValue &&
            !_exerciseRepository.EquipmentExists(
                exercise.EquipmentId.Value))
        {
            throw new InvalidOperationException(
                "Rekvizit sa unetim ID-em ne postoji.");
        }

        if (exercise.MachineId.HasValue &&
            !_exerciseRepository.MachineExists(
                exercise.MachineId.Value))
        {
            throw new InvalidOperationException(
                "Sprava sa unetim ID-em ne postoji.");
        }
    }
}
