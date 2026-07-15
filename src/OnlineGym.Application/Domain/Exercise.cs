namespace OnlineGym.Application.Domain;

public class Exercise
{
    public long Id { get; set; }
    public long TrainerId { get; set; }
    public long? EquipmentId { get; set; }
    public long? MachineId { get; set; }
    public string Name { get; set; }
    public string? VideoUrl { get; set; }

    public Exercise(long id, long trainerId, long? equipmentId, long? machineId, string name, string? videoUrl)
    {
        Id = id;
        TrainerId = trainerId;
        EquipmentId = equipmentId;
        MachineId = machineId;
        Name = name;
        VideoUrl = videoUrl;
    }
}