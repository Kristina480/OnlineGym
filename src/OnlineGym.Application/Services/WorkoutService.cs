using OnlineGym.Application.Domain;
using OnlineGym.Application.Domain.Enums;
using OnlineGym.Application.Interfaces.Repositories;
using OnlineGym.Application.Interfaces.Services;

namespace OnlineGym.Application.Services;

public class WorkoutService:IWorkoutService
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IWorkoutItemRepository _workoutItemRepository;
    private readonly ICollaborationRepository _collaborationRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ITrainerRepository _trainerRepository;

    public WorkoutService(
        IWorkoutRepository workoutRepository,
        IWorkoutItemRepository workoutItemRepository,
        ICollaborationRepository collaborationRepository,
        IExerciseRepository exerciseRepository,
        IClientRepository clientRepository,
        ITrainerRepository trainerRepository)
    {
        _workoutRepository = workoutRepository;
        _workoutItemRepository = workoutItemRepository;
        _collaborationRepository = collaborationRepository;
        _exerciseRepository = exerciseRepository;
        _clientRepository = clientRepository;
        _trainerRepository = trainerRepository;
    }

    public long CreateWorkout(
        long trainerId,
        long clientId,
        DateTime scheduledAt,
        string? comment,
        List<WorkoutItem> items)
    {
        ValidateWorkoutData(
            trainerId,
            clientId,
            scheduledAt,
            items);

        Collaboration? collaboration =
            _collaborationRepository.GetActiveByClientAndTrainer(
                clientId,
                trainerId);

        if (collaboration is null)
            throw new InvalidOperationException(
                "The trainer and client do not have an active collaboration.");

        ValidateExercises(items, trainerId);

        Workout workout = new Workout(
            0,
            collaboration.Id,
            scheduledAt,
            WorkoutStatus.Scheduled,
            false,
            comment,
            null
        );

        long workoutId = _workoutRepository.Insert(workout);

        foreach (WorkoutItem item in items)
        {
            item.WorkoutId = workoutId;
            item.IsCompleted = false;
            item.ItemRating = null;
        }

        _workoutItemRepository.CreateMany(items);
        return workoutId;
    }

    public Workout? GetById(long workoutId)
    {
        return _workoutRepository.GetById(workoutId);
    }

    public List<Workout> GetByClientId(long clientId)
    {
        if (_clientRepository.GetById(clientId) is null)
            throw new InvalidOperationException(
                $"Client with ID {clientId} does not exist.");

        return _workoutRepository.GetByClientId(clientId);
    }

    public List<WorkoutItem> GetWorkoutItems(long workoutId)
    {
        if (_workoutRepository.GetById(workoutId) is null)
            throw new InvalidOperationException(
                $"Workout with ID {workoutId} does not exist.");

        return _workoutItemRepository.GetByWorkoutId(workoutId);
    }

    private void ValidateWorkoutData(
        long trainerId,
        long clientId,
        DateTime scheduledAt,
        List<WorkoutItem> items)
    {
        if (_trainerRepository.GetById(trainerId) is null)
            throw new InvalidOperationException(
                $"Trainer with ID {trainerId} does not exist.");

        if (_clientRepository.GetById(clientId) is null)
            throw new InvalidOperationException(
                $"Client with ID {clientId} does not exist.");

        if (scheduledAt == default)
            throw new ArgumentException(
                "Workout date and time are required.");

        if (items is null || items.Count == 0)
            throw new ArgumentException(
                "A workout must contain at least one exercise.");

        if (items.Any(item => item.ExerciseId <= 0))
            throw new ArgumentException(
                "Every workout item must contain a valid exercise.");

        if (items.Any(item => item.RepetitionCount is < 0))
            throw new ArgumentException(
                "Repetition count cannot be negative.");

        if (items
            .GroupBy(item => item.ExerciseId)
            .Any(group => group.Count() > 1))
        {
            throw new InvalidOperationException(
                "The same exercise cannot be added more than once.");
        }
    }

    private void ValidateExercises(
        List<WorkoutItem> items,
        long trainerId)
    {
        foreach (WorkoutItem item in items)
        {
            if (!_exerciseRepository.ExistsByIdAndTrainerId(
                    item.ExerciseId,
                    trainerId))
            {
                throw new InvalidOperationException(
                    $"Exercise with ID {item.ExerciseId} does not belong to this trainer.");
            }
        }
    }
    public List<Client> GetActiveClients(long trainerId)
    {
        if (_trainerRepository.GetById(trainerId) is null) throw new InvalidOperationException("Trener ne postoji.");
        return _collaborationRepository.GetActiveByTrainerId(trainerId)
            .Select(c => _clientRepository.GetById(c.ClientId))
            .Where(c => c is not null)
            .Cast<Client>()
            .ToList();
    }
}