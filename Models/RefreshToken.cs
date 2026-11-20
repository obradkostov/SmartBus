using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int KorisnikId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime DatumIsteka { get; set; }

    public bool Opozvan { get; set; }

    public virtual Korisnik Korisnik { get; set; } = null!;
}
