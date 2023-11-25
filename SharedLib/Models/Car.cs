namespace SharedLib.Models;
public class Car
{
    public int Id { get; set; }
    public string Model { get; set; }
    public string Description { get; set; }
    public string? PathImage { get; set; }


    public override string ToString() => $@"{Model.ToUpper()}"; 


}
