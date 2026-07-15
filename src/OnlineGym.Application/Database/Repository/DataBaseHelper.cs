using System.Data;

namespace OnlineGym.Application.Database.Repositories;

public class DataBaseHelper
{
    public static void AddParameter(IDbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
    
    public static string ToPascalCase(string value)
    {
        return string.Concat(
            value.Split('_')
                .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
        );
    }
    
    public static string ToUpperSnakeCase(string value)
    {
        return string.Concat(value.Select((c, i) =>
                i > 0 && char.IsUpper(c)
                    ? "_" + c
                    : c.ToString()))
            .ToUpper();
    }
}