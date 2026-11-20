using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class Stanica
{
    public int Id { get; set; }

    public string Naziv { get; set; } = null!;

    public int BrojPerona { get; set; }
    [JsonIgnore]
    public virtual ICollection<Polazak> PolazakStanicaDoNavigation { get; set; } = new List<Polazak>();
    [JsonIgnore]
    public virtual ICollection<Polazak> PolazakStanicaOdNavigation { get; set; } = new List<Polazak>();
}
