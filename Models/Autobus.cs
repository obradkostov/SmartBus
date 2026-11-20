using System;
using System.Collections.Generic;


namespace SmartBus.Models;

public partial class Autobus
{
    public int Id { get; set; }

    public string Model { get; set; } = null!;

    public int BrojSedista { get; set; }

    public string? Prevoznik { get; set; }
    [JsonIgnore]
    public virtual ICollection<OcenaAutobus> OcenaAutobus { get; set; } = new List<OcenaAutobus>();
    [JsonIgnore]
    public virtual ICollection<Polazak> Polazak { get; set; } = new List<Polazak>();
    [JsonIgnore]
    public virtual ICollection<Sediste> Sediste { get; set; } = new List<Sediste>();
}
