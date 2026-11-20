namespace SmartBus.DTOs.SedisteDto;

public class ResponseSedisteDto
{
    public int Id { get; set; } = 0;
    public int Broj { get; set; } = 0;
    public string Status { get; set; } = string.Empty;
}

public class CreateSedisteDto
{
    public int AutobusId { get; set; } = 0;
    public int Broj { get; set; } = 0;
}

public class UpdateSedisteDto
{
    public int Id { get; set; } = 0;
    public int NoviBroj { get; set; } = 0;
    public string NoviStatus { get; set; } = string.Empty;
}