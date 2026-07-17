namespace OnlineGym.Application.Domain;

public class Equipment
{
    public long Id { get; set; }

    public string Name { get; set; }

    public Equipment(long id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString()
    {
        return $"{Id} - {Name}";
    }
}