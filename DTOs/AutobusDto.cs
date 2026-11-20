using System.ComponentModel.DataAnnotations;

namespace SmartBus.DTOs.AutobusDto;

public class ResponseAutobusDto
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public int BrojSedista { get; set; }
    public string Prevoznik { get; set; } = string.Empty;
    public List<OcenaAutobusDto> OceneAutobusa { get; set; } = new List<OcenaAutobusDto>();
}

public class OcenaAutobusDto
{
    public int AutobusId { get; set; }
    public int KorisnikId { get; set; }
    public int Ocena { get; set; }
}