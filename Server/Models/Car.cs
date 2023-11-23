namespace Server.Models;
public class Car
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public override string ToString() => $@"Id: {Id}
Car name: {Name}
Description: {Description} ";
}

