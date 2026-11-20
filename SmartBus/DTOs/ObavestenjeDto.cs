namespace SmartBus.DTOs;

public class CreateObavestenjeDto
{
    public int PolazakId { get; set; }
    public string Naslov { get; set; } = string.Empty;
    public string Poruka { get; set; } = string.Empty;
    public string Tip { get; set; } = string.Empty;
}

public class ResponseObavestenjeDto
{
    public int Id { get; set; }
    public int? PolazakId { get; set; }
    public string Naslov { get; set; } = string.Empty;
    public string Poruka { get; set; } = string.Empty;
    public DateTime VremeKreiranja { get; set; }
    public string Tip { get; set; } = string.Empty;
}
