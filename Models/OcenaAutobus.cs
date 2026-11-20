using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class OcenaAutobus
{
    public int Id { get; set; }

    public int AutobusId { get; set; }

    public int KorisnikId { get; set; }

    public int Ocena { get; set; }

    public virtual Autobus Autobus { get; set; } = null!;

    public virtual Korisnik Korisnik { get; set; } = null!;
}
