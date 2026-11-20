using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class Obavestenje
{
    public int Id { get; set; }

    public int? PolazakId { get; set; }

    public string? Naslov { get; set; }

    public string Poruka { get; set; } = null!;

    public DateTime VremeKreiranja { get; set; }

    public string Tip { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<ObavestenjeKorisnik> ObavestenjeKorisnik { get; set; } = new List<ObavestenjeKorisnik>();

    public virtual Polazak? Polazak { get; set; }
}
