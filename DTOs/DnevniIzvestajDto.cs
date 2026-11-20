namespace SmartBus.Dtos;

public class ResponseDnevniIzvestajDto
{
    public string Ime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly Datum { get; set; }
    public string Smena { get; set; } = string.Empty;
    public string Relacije { get; set; } = string.Empty;
    //ukupan broj pregledanih karata u toku dana
    public int PregledaneKarte { get; set; }
    //ukupan broj validnih karata
    public int ValidneKarte { get; set; }
    //ukupan broj nevalidnih karti
    public int NevalidneKarte { get; set; }
    public int PutniciBezKarte { get; set; }
    public int PrekrsajniNalozi { get; set; }
    public decimal IznosPlacenihKazni { get; set; }
    //ukupno odbijenih placanja kazni
    public int OdbijenaPlacanja { get; set; }
    //ukupan broj lica koji su dobili prekrsajne prijave(onaj koji nije platio kaznu BIVA prijavljen u MUP)
    public int PrijavljenaLica { get; set; }
    public int VerbalniIncidenti { get; set; }
    public int FizickiIncidenti { get; set; }
    public int PoziviPolicije { get; set; }
    public string OpisIncidenata { get; set; } = string.Empty;
    public string NepravilnostiNaVozilu { get; set; } = string.Empty;
    public string ProblemiSaSistemom { get; set; } = string.Empty;
}

public class CreateDnevniIzvestajDto
{
    public string Email { get; set; } = string.Empty;
    //public int Legitimacija { get; set; }
    public string Smena { get; set; } = string.Empty;
    public string Relacije { get; set; } = string.Empty;
    //ukupan broj pregledanih karata u toku dana
    public int PregledaneKarte { get; set; }
    //ukupan broj validnih karata
    public int ValidneKarte { get; set; }
    //ukupan broj nevalidnih karti
    public int NevalidneKarte { get; set; }
    public int PutniciBezKarte { get; set; }
    public int PrekrsajniNalozi { get; set; }
    public decimal IznosPlacenihKazni { get; set; }
    //ukupno odbijenih placanja kazni
    public int OdbijenaPlacanja { get; set; }
    //ukupan broj lica koji su dobili prekrsajne prijave(onaj koji nije platio kaznu BIVA prijavljen u MUP)
    public int PrijavljenaLica { get; set; }
    public int VerbalniIncidenti { get; set; }
    public int FizickiIncidenti { get; set; }
    public int PoziviPolicije { get; set; }
    public string OpisIncidenata { get; set; } = string.Empty;
    public string NepravilnostiNaVozilu { get; set; } = string.Empty;
    public string ProblemiSaSistemom { get; set; } = string.Empty;
}

public class ListaIzvestajaDto
{
    public int TrenutnaStrana { get; set; }
    public int UkupnoStrana { get; set; }
    public int UkupnoIzvestaja { get; set; }
    public IList<ResponseDnevniIzvestajDto> ListaIzvestaja { get; set; } = new List<ResponseDnevniIzvestajDto>();
}

public class FiltrirajIzvestajeDto
{
    public int Strana { get; set; }
    public int VelicinaStrane { get; set; }
    public DateOnly? Datum { get; set; }
    public int? KontrolorId { get; set; }
    public string? Smena { get; set; }
}

public class StatistikaDanaDto
{
    public int IzvestajiDanas { get; set; }
    public int PregledaneKarte { get; set; }
    public int PrekrsajniNalozi { get; set; }
    public decimal NaplaceneKazne { get; set; }
}