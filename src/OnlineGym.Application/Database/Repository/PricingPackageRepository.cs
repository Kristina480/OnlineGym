using System.Data;
using OnlineGym.Application.Domain;
using OnlineGym.Application.Interfaces.Repositories;

namespace OnlineGym.Application.Database.Repositories;

public class PricingPackageRepository:IPricingPackageRepository
{
     public long Insert(PricingPackage pricingPackage)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO pricing_packages
            (trainer_id, workouts_per_week, monthly_price)
            VALUES
            (@trainer_id, @workouts_per_week, @monthly_price)
            RETURNING pricing_package_id;";

        DataBaseHelper.AddParameter(command, "@trainer_id", pricingPackage.TrainerId);
        DataBaseHelper.AddParameter(command, "@workouts_per_week", pricingPackage.WorkoutsPerWeek);
        DataBaseHelper.AddParameter(command, "@monthly_price", pricingPackage.MonthlyPrice);

        object? result = command.ExecuteScalar();
        if (result is null || result == DBNull.Value)
            throw new InvalidOperationException("Pricing package was not created.");

        return Convert.ToInt64(result);
    }

    public PricingPackage? GetById(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT pricing_package_id, trainer_id, workouts_per_week, monthly_price
            FROM pricing_packages
            WHERE pricing_package_id = @pricing_package_id;";

        DataBaseHelper.AddParameter(command, "@pricing_package_id", id);

        using IDataReader reader = command.ExecuteReader();
        return reader.Read() ? MapFromReader(reader) : null;
    }

    public List<PricingPackage> GetByTrainerId(long trainerId)
    {
        List<PricingPackage> pricingPackages = new();

        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            SELECT pricing_package_id, trainer_id, workouts_per_week, monthly_price
            FROM pricing_packages
            WHERE trainer_id = @trainer_id
            ORDER BY workouts_per_week;";

        DataBaseHelper.AddParameter(command, "@trainer_id", trainerId);

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
            pricingPackages.Add(MapFromReader(reader));

        return pricingPackages;
    }

    public void Update(PricingPackage pricingPackage)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE pricing_packages
            SET trainer_id = @trainer_id,
                workouts_per_week = @workouts_per_week,
                monthly_price = @monthly_price
            WHERE pricing_package_id = @pricing_package_id;";

        DataBaseHelper.AddParameter(command, "@pricing_package_id", pricingPackage.Id);
        DataBaseHelper.AddParameter(command, "@trainer_id", pricingPackage.TrainerId);
        DataBaseHelper.AddParameter(command, "@workouts_per_week", pricingPackage.WorkoutsPerWeek);
        DataBaseHelper.AddParameter(command, "@monthly_price", pricingPackage.MonthlyPrice);

        command.ExecuteNonQuery();
    }

    public void Delete(long id)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
            DELETE FROM pricing_packages
            WHERE pricing_package_id = @pricing_package_id;";

        DataBaseHelper.AddParameter(command, "@pricing_package_id", id);
        command.ExecuteNonQuery();
    }

    private PricingPackage MapFromReader(IDataReader reader)
    {
        return new PricingPackage(
            reader.GetInt64(reader.GetOrdinal("pricing_package_id")),
            reader.GetInt64(reader.GetOrdinal("trainer_id")),
            reader.GetInt32(reader.GetOrdinal("workouts_per_week")),
            reader.GetDecimal(reader.GetOrdinal("monthly_price"))
        );
    }
}