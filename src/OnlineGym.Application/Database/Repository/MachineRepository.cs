using System.Data;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;

namespace OnlineGym.Application.Database.Repositories;

public class MachineRepository : IMachineRepository
{
    public long Insert(Machine machine)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO machines (name)
            VALUES (@name)
            RETURNING machine_id;";

        DataBaseHelper.AddParameter(command, "@name", machine.Name);

        object? result = command.ExecuteScalar();

        if (result is null || result == DBNull.Value)
        {
            throw new InvalidOperationException("Sprava nije kreirana.");
        }

        return Convert.ToInt64(result);
    }

    public Machine? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT machine_id, name
            FROM machines
            WHERE machine_id = @machine_id;";

        DataBaseHelper.AddParameter(command, "@machine_id", id);

        using IDataReader reader = command.ExecuteReader();

        return reader.Read()
            ? MapFromReader(reader)
            : null;
    }

    public List<Machine> GetAll()
    {
        List<Machine> machines = new();

        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT machine_id, name
            FROM machines
            ORDER BY name;";

        using IDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            machines.Add(MapFromReader(reader));
        }

        return machines;
    }

    public void Update(Machine machine)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE machines
            SET name = @name
            WHERE machine_id = @machine_id;";

        DataBaseHelper.AddParameter(command, "@name", machine.Name);
        DataBaseHelper.AddParameter(command, "@machine_id", machine.Id);

        int affectedRows = command.ExecuteNonQuery();

        if (affectedRows == 0)
        {
            throw new InvalidOperationException(
                "Sprava koju želite da izmenite ne postoji."
            );
        }
    }

    public void Delete(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM machines
            WHERE machine_id = @machine_id;";

        DataBaseHelper.AddParameter(command, "@machine_id", id);

        int affectedRows = command.ExecuteNonQuery();

        if (affectedRows == 0)
        {
            throw new InvalidOperationException(
                "Sprava koju želite da obrišete ne postoji."
            );
        }
    }

    private static Machine MapFromReader(IDataReader reader)
    {
        return new Machine(
            reader.GetInt64(reader.GetOrdinal("machine_id")),
            reader.GetString(reader.GetOrdinal("name"))
        );
    }
}