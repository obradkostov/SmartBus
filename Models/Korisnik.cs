using System;
using System.Collections.Generic;

namespace SmartBus.Models;

public partial class Korisnik
{
    public int Id { get; set; }

    public string Ime { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Lozinka { get; set; } = null!;

    public string Uloga { get; set; } = null!;

    public DateTime VremeRegistracije { get; set; }
    [JsonIgnore]
    public virtual ICollection<Karta> KartaKontrolor { get; set; } = new List<Karta>();
    [JsonIgnore]
    public virtual ICollection<Karta> KartaKorisnik { get; set; } = new List<Karta>();
    [JsonIgnore]
    public virtual ICollection<ObavestenjeKorisnik> ObavestenjeKorisnik { get; set; } = new List<ObavestenjeKorisnik>();
    [JsonIgnore]
    public virtual ICollection<OcenaAutobus> OcenaAutobus { get; set; } = new List<OcenaAutobus>();
    [JsonIgnore]
    public virtual ICollection<RefreshToken> RefreshToken { get; set; } = new List<RefreshToken>();
}
