using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class Sediste
{
    public int Id { get; set; }

    public int AutobusId { get; set; }

    public int Broj { get; set; }

    public string Status { get; set; } = null!;

    public virtual Autobus Autobus { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Karta> Karta { get; set; } = new List<Karta>();
}
