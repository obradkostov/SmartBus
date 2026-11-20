namespace SmartBus.Services.Internal;

public class KartaService : IKartaService
{
    private AppDbContext Context;
    private readonly ISignatureService SignatureService;

    public KartaService(AppDbContext context, ISignatureService signatureService)
    {
        Context = context;
        SignatureService = signatureService;
    }

    private static ResponseKartaDto MapToDto(Karta karta) =>
        new()
        {
            Id = karta.Id,
            AutobusId = karta.Polazak.AutobusId,
            ImeIPrezime = karta.Korisnik.Ime,
            Email = karta.Korisnik.Email,
            Peron = karta.Polazak.Peron,
            Sediste = karta.Sediste.Broj,
            Cena = karta.PlacenaCena,
            Status = karta.Status,
            VremePolaska = karta.Polazak.VremePolaska,
            Prevoznik = karta.Polazak.Autobus?.Prevoznik ?? "—",
            StanicaOd = karta.Polazak.StanicaOdNavigation?.Naziv ?? "—",
            StanicaDo = karta.Polazak.StanicaDoNavigation?.Naziv ?? "—",
        };

    public async Task<ResponseKartaDto> Create(CreateKartaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("Nema email adrese");
        var polazak = await Context.Polasci.FindAsync(dto.PolazakId);
        if (polazak == null)
            throw new Exception("Nema polaska");
        var autobus = await Context.Autobusi.FindAsync(polazak.AutobusId);
        if (autobus == null)
            throw new Exception("Nema autobusa");
        if (dto.Sediste <= 0 || dto.Sediste > autobus.BrojSedista)
            throw new ArgumentException("Taj broj sedista ne postoji");
        var sediste = await Context
            .Sedista.Include(s => s.Autobus)
            .Where(s => s.AutobusId == autobus.Id && s.Broj == dto.Sediste)
            .FirstOrDefaultAsync();
        if (sediste == null)
            throw new Exception("Nema sedista");
        if (sediste.Status.Equals("zauzeto"))
            throw new Exception("Sediste je zauzeto");
        sediste.Status = "zauzeto";
        var korisnik = await Context
            .Korisnici.Where(k => k.Email.Equals(dto.Email))
            .FirstOrDefaultAsync();
        if (korisnik == null)
            throw new Exception("Nema korisnika");
        var karta = new Karta
        {
            Korisnik = korisnik,
            KorisnikId = korisnik.Id,
            Polazak = polazak,
            PolazakId = polazak.Id,
            Sediste = sediste,
            SedisteId = sediste.Id,
            BrojSedista = sediste.Broj,
            PlacenaCena = polazak.Cena,
            Status = "kupljena",
            VremeKupovine = DateTime.Now,
        };
        await Context.Karte.AddAsync(karta);
        await Context.SaveChangesAsync();
        return MapToDto(karta);
    }

    public async Task<IList<ResponseKartaDto>> Read(int korisnikId)
    {
        var korisnik = await Context.Korisnici.FindAsync(korisnikId);
        if (korisnik == null)
            throw new Exception("Nema korisnika");
        var karte = await Context
            .Karte.Include(k => k.Korisnik)
            .Include(k => k.Sediste)
            .Include(k => k.Polazak)
                .ThenInclude(p => p.Autobus)
            .Include(k => k.Polazak)
                .ThenInclude(p => p.StanicaOdNavigation)
            .Include(k => k.Polazak)
                .ThenInclude(p => p.StanicaDoNavigation)
            .Where(k => k.KorisnikId == korisnikId)
            .ToListAsync();
        IList<ResponseKartaDto> listaKarata = new List<ResponseKartaDto>();
        foreach (var k in karte)
        {
            listaKarata.Add(MapToDto(k));
        }
        return listaKarata;
    }

    public async Task<List<int>> ZauzetaSedista(int polazakId)
    {
        return await Context
            .Karte.Where(k => k.PolazakId == polazakId && k.Status != "otkazana")
            .Select(k => k.BrojSedista)
            .ToListAsync();
    }

    public async Task<ResponseKartaDto> Update(UpdateKartaDto dto)
    {
        var karta = await Context
            .Karte.Include(k => k.Korisnik)
            .Include(k => k.Polazak)
            .Where(k => k.Id == dto.Id)
            .FirstOrDefaultAsync();
        if (karta == null)
            throw new Exception("Nema karte");
        karta.BrojSedista = dto.Sediste;
        karta.PlacenaCena = dto.Cena;
        karta.Status = dto.Status;
        await Context.SaveChangesAsync();
        return MapToDto(karta);
    }

    public async Task<ResponseKartaDto> Delete(int kartaId)
    {
        var karta = await Context.Karte.FindAsync(kartaId);
        if (karta == null)
            throw new Exception("Nema karte");
        Context.Karte.Remove(karta);
        await Context.SaveChangesAsync();
        return new ResponseKartaDto();
    }

    public async Task<ResponseKartaDto> Validate(int kartaId)
    {
        var karta = await Context.Karte.FindAsync(kartaId);
        if (karta == null)
            throw new Exception("Karta nije pronadjena");
        if (karta.Status.Equals("validirana") || karta.Status.Equals("otkazana"))
            throw new Exception("Karta nije validna");
        karta.Status = "validirana";
        await Context.SaveChangesAsync();
        return MapToDto(karta);
    }

    public async Task<ValidacijaResponseDto> ValidirajKartu(ValidacijaKarteDto dto, int kontrolorId)
    {
        var karta = await Context
            .Karte.Include(k => k.Korisnik)
            .Include(k => k.Polazak)
                .ThenInclude(p => p.StanicaOdNavigation)
            .Include(k => k.Polazak)
                .ThenInclude(p => p.StanicaDoNavigation)
            .FirstOrDefaultAsync(k => k.Id == dto.KartaId);
        if (karta == null)
            return new ValidacijaResponseDto { Validirana = false, Poruka = "Karta ne postoji." };

        // signature
        bool signatureProvera = SignatureService.ProveriSignature(
            dto.Signature,
            karta.Id,
            karta.Polazak.VremePolaska,
            karta.KorisnikId
        );

        if (!signatureProvera)
            return new ValidacijaResponseDto { Validirana = false, Poruka = "Neispravan potpis." };

        if (karta.Polazak.VremePolaska < DateTime.UtcNow)
            return new ValidacijaResponseDto
            {
                Validirana = false,
                Poruka = "Polazak je vec prosao.",
            };

        if (karta.Status == "validirana")
            return new ValidacijaResponseDto
            {
                Validirana = false,
                Poruka = "Karta je vec validirana.",
            };

        if (karta.Status == "otkazana")
            return new ValidacijaResponseDto { Validirana = false, Poruka = "Karta je otkazana." };

        karta.Status = "validirana";
        karta.VremeValidacije = DateTime.UtcNow;
        karta.KontrolorId = kontrolorId;
        await Context.SaveChangesAsync();

        return new ValidacijaResponseDto
        {
            Validirana = true,
            Poruka = "Karta je uspesno validirana.",
            KartaInfo = new KartaInfoDto
            {
                ImePutnika = karta.Korisnik.Ime,
                BrojSedista = karta.BrojSedista,
                VremePolaska = karta.Polazak.VremePolaska,
                StanicaOd = karta.Polazak.StanicaOdNavigation.Naziv,
                StanicaDo = karta.Polazak.StanicaDoNavigation.Naziv,
            },
        };
    }

    public async Task<bool> OtkaziKartu(int kartaId)
    {
        var karta = await Context
            .Karte.Include(k => k.Sediste)
            .FirstOrDefaultAsync(k => k.Id == kartaId);

        if (karta == null)
            throw new Exception("Karta ne postoji");
        if (karta.Status == "otkazana")
            throw new Exception("Karta je već otkazana");
        if(karta.Status == "validirana")
            throw new Exception("Karta je vec validirana");

        karta.Status = "otkazana";

        if (karta.Sediste != null)
        {
            karta.Sediste.Status = "slobodno";
        }

        await Context.SaveChangesAsync();
        return true;
    }

    public async Task<string?> GetSignature(int kartaId, int korisnikId)
    {
        var karta = await Context
            .Karte.Include(k => k.Polazak)
            .FirstOrDefaultAsync(k => k.Id == kartaId && k.KorisnikId == korisnikId);

        if (karta == null)
            return null;

        return SignatureService.GenerisiSignature(karta.Id, karta.Polazak.VremePolaska, korisnikId);
    }
}
