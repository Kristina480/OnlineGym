using OnlineGym.Application.Domain;

namespace OnlineGym.Application.Interfaces.Repositories;

public interface IMachineRepository
{
    long Insert(Machine machine);
    Machine? GetById(long id);
    List<Machine> GetAll();
    void Update(Machine machine);
    void Delete(long id);
}