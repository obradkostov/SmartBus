namespace SmartBus.DTOs.StanicaDto;

public class ResponseStanicaDto
{
    public int Id { get; set; } = 0;
    public string Naziv { get; set; } = string.Empty;
    public int BrojPerona { get; set; } = 0;
}

public class CreateStanicaDto
{
    public string Naziv { get; set; } = string.Empty;
    public int BrojPerona { get; set; } = 0;
}