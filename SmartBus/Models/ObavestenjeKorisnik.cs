using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class ObavestenjeKorisnik
{
    public int Id { get; set; }

    public int ObavestenjeId { get; set; }

    public int KorisnikId { get; set; }

    public bool Procitano { get; set; }

    public virtual Korisnik Korisnik { get; set; } = null!;

    public virtual Obavestenje Obavestenje { get; set; } = null!;
}
