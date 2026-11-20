using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class Polazak
{
    public int Id { get; set; }

    public int AutobusId { get; set; }

    public int StanicaOd { get; set; }

    public int StanicaDo { get; set; }

    public DateTime VremePolaska { get; set; }

    public int Peron { get; set; }

    public decimal Cena { get; set; }

    public string Status { get; set; } = null!;

    public virtual Autobus Autobus { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Karta> Karta { get; set; } = new List<Karta>();
    [JsonIgnore]
    public virtual ICollection<Obavestenje> Obavestenje { get; set; } = new List<Obavestenje>();

    public virtual Stanica StanicaDoNavigation { get; set; } = null!;

    public virtual Stanica StanicaOdNavigation { get; set; } = null!;
}
