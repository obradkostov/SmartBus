namespace SmartBus.DTOs.PolazakDto;

// Isti dto se koristi za izmenu i dodavanje
public class ResponsePolazakDto
{
    public int Id { get; set; }
    public ResponseAutobusDto Autobus { get; set; } = new ResponseAutobusDto();
    public Stanica StanicaOd { get; set; } = new Stanica();
    public Stanica StanicaDo { get; set; } = new Stanica();
    public DateTime VremePolaska { get; set; }
    public int Peron { get; set; }
    public decimal Cena { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ResponsePolazakPagingDto
{
    public IList<ResponsePolazakDto> Polasci { get; set; } = new List<ResponsePolazakDto>();
    public int UkupnoPolazaka { get; set; }
    public int TrenutnaStrana { get; set; }
    public int UkupnoStrana { get; set; }
}

public class CreatePolazakDto
{
    public int AutobusId { get; set; } = 0;
    public int StanicaOdId { get; set; } = 0;
    public int StanicaDoId { get; set; } = 0;
    public DateTime VremePolaska { get; set; } = DateTime.Now;
    public int Peron { get; set; } = 0;
    public decimal Cena { get; set; } = 0;
}

public class UpdatePolazakDto
{
    public int Id { get; set; }
    public int AutobusId { get; set; }
    public int StanicaOdId { get; set; }
    public int StanicaDoId { get; set; }
    public DateTime VremePolaska { get; set; }
    public int Peron { get; set; }
    public decimal Cena { get; set; }
}