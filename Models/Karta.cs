using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class Karta
{
    public int Id { get; set; }

    public int KorisnikId { get; set; }

    public int PolazakId { get; set; }

    public int SedisteId { get; set; }

    public decimal PlacenaCena { get; set; }

    public string Status { get; set; } = null!;

    public DateTime VremeKupovine { get; set; }

    public DateTime? VremeValidacije { get; set; }

    public int? KontrolorId { get; set; }

    public int BrojSedista { get; set; }

    public virtual Sediste Sediste { get; set; } = null!;

    public virtual Korisnik Korisnik { get; set; } = null!;

    public virtual Korisnik Kontrolor { get; set; } = null!;

    public virtual Polazak Polazak { get; set; } = null!;
}
