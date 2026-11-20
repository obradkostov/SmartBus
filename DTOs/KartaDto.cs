using System.ComponentModel.DataAnnotations;

namespace SmartBus.DTOs.KartaDto;

public class ResponseKartaDto
{
    public int Id { get; set; }
    public int AutobusId { get; set; }
    public string ImeIPrezime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Peron { get; set; }
    public int Sediste { get; set; }
    public decimal Cena { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime VremePolaska { get; set; }
    public string Prevoznik { get; set; } = string.Empty;
    public string StanicaOd { get; set; } = string.Empty;
    public string StanicaDo { get; set; } = string.Empty;
}
public class CreateKartaDto
{
    public string Email { get; set; } = string.Empty;
    public int PolazakId { get; set; }
    public int Sediste { get; set; }
}

public class UpdateKartaDto
{
    public int Id { get; set; }
    public int Sediste { get; set; }
    public int Cena { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ValidacijaKarteDto
{
    [Range(1, int.MaxValue, ErrorMessage = "KartaId mora biti pozitivan")]
    public required int KartaId { get; set; }
    public required string Signature { get; set; }
    public DateTime SkeniranoU { get; set; } = DateTime.UtcNow;
}

public class ValidacijaResponseDto
{
    public bool Validirana { get; set; }
    public string Poruka { get; set; } = string.Empty;
    public KartaInfoDto? KartaInfo { get; set; }
}

public class KartaInfoDto
{
    public string ImePutnika { get; set; } = string.Empty;
    public int BrojSedista { get; set; }
    public DateTime VremePolaska { get; set; }
    public string StanicaOd { get; set; } = string.Empty;
    public string StanicaDo { get; set; } = string.Empty;
}