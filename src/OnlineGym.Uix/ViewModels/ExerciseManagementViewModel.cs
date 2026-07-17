using System.Collections.Generic;
using OnlineGym.Application.Database.Repositories;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Services;
using OnlineGym.Application.Services;

namespace OnlineGym.Uix.ViewModels;

public class ExerciseManagementViewModel
{
    private readonly long _trainerId;

    private readonly IExerciseService _service;

    private readonly EquipmentRepository
        _equipmentRepository;

    private readonly MachineRepository
        _machineRepository;

    public ExerciseManagementViewModel(
        long trainerId)
    {
        _trainerId = trainerId;

        _service = new ExerciseService(
            new ExerciseRepository(),
            new TrainerRepository());

        _equipmentRepository =
            new EquipmentRepository();

        _machineRepository =
            new MachineRepository();
    }

    public List<Exercise> GetExercises()
    {
        return _service
            .GetTrainerExercises(_trainerId);
    }

    public List<Equipment> GetEquipment()
    {
        return _equipmentRepository.GetAll();
    }

    public List<Machine> GetMachines()
    {
        return _machineRepository.GetAll();
    }

    public long CreateExercise(
        string name,
        string? videoUrl,
        long? equipmentId,
        long? machineId)
    {
        Exercise exercise = new(
            0,
            _trainerId,
            equipmentId,
            machineId,
            name,
            videoUrl);

        return _service.CreateExercise(
            exercise,
            _trainerId);
    }

    public void DeleteExercise(
        long exerciseId)
    {
        _service.DeleteExercise(
            exerciseId,
            _trainerId);
    }
}