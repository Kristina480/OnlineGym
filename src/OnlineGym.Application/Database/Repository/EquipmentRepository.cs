using System.Data;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;

namespace OnlineGym.Application.Database.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    public long Insert(Equipment equipment)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO equipment (name)
            VALUES (@name)
            RETURNING equipment_id;";

        DataBaseHelper.AddParameter(command, "@name", equipment.Name);

        object? result = command.ExecuteScalar();

        if (result is null || result == DBNull.Value)
        {
            throw new InvalidOperationException("Rekvizit nije kreiran.");
        }

        return Convert.ToInt64(result);
    }

    public Equipment? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT equipment_id, name
            FROM equipment
            WHERE equipment_id = @equipment_id;";

        DataBaseHelper.AddParameter(command, "@equipment_id", id);

        using IDataReader reader = command.ExecuteReader();

        return reader.Read()
            ? MapFromReader(reader)
            : null;
    }

    public List<Equipment> GetAll()
    {
        List<Equipment> equipmentList = new();

        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT equipment_id, name
            FROM equipment
            ORDER BY name;";

        using IDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            equipmentList.Add(MapFromReader(reader));
        }

        return equipmentList;
    }

    public void Update(Equipment equipment)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE equipment
            SET name = @name
            WHERE equipment_id = @equipment_id;";

        DataBaseHelper.AddParameter(command, "@name", equipment.Name);
        DataBaseHelper.AddParameter(command, "@equipment_id", equipment.Id);

        int affectedRows = command.ExecuteNonQuery();

        if (affectedRows == 0)
        {
            throw new InvalidOperationException(
                "Rekvizit koji želite da izmenite ne postoji."
            );
        }
    }

    public void Delete(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        using IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM equipment
            WHERE equipment_id = @equipment_id;";

        DataBaseHelper.AddParameter(command, "@equipment_id", id);

        int affectedRows = command.ExecuteNonQuery();

        if (affectedRows == 0)
        {
            throw new InvalidOperationException(
                "Rekvizit koji želite da obrišete ne postoji."
            );
        }
    }

    private static Equipment MapFromReader(IDataReader reader)
    {
        return new Equipment(
            reader.GetInt64(reader.GetOrdinal("equipment_id")),
            reader.GetString(reader.GetOrdinal("name"))
        );
    }
}