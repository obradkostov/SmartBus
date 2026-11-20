using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class DnevniIzvestaj
{
    public int Id { get; set; }

    public int KontrolorId { get; set; }

    public DateOnly Datum { get; set; }

    public string Smena { get; set; } = null!;

    public string Relacije { get; set; } = null!;

    public int PregledaneKarte { get; set; }

    public int ValidneKarte { get; set; }

    public int NevalidneKarte { get; set; }

    public int PutniciBezKarte { get; set; }

    public int PrekrsajniNalozi { get; set; }

    public decimal IznosPlacenihKazni { get; set; }

    public int OdbijenaPlacanja { get; set; }

    public int PrijavljenaLica { get; set; }

    public int VerbalniIncidenti { get; set; }

    public int FizickiIncidenti { get; set; }

    public int PoziviPolicije { get; set; }

    public string? OpisIncidenata { get; set; }

    public string? NepravilnostiNaVozilu { get; set; }

    public string? ProblemiSaSistemom { get; set; }
}
