using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface IEquipmentRepository
{
    long Insert(Equipment equipment);
    Equipment? GetById(long id);
    List<Equipment> GetAll();
    void Update(Equipment equipment);
    void Delete(long id);
}