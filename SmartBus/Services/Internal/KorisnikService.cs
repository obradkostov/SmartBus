

namespace SmartBus.Services.Internal;

public class KorisnikService : IKorisnikService
{
    private AppDbContext Context;
    private IRefreshTokenService RefreshTokenService;
    public KorisnikService(AppDbContext context, IRefreshTokenService refreshTokenService)
    {
        Context = context;
        RefreshTokenService = refreshTokenService;
    }

    private static ResponseKorisnikDto MapToDto(Korisnik korisnik) => new()
    {
        Id = korisnik.Id,
        Ime = korisnik.Ime,
        Email = korisnik.Email,
        Uloga = korisnik.Uloga
    };

    private static UpdateKorisnikDto MapToUpdateDto(Korisnik korisnik) => new()
    {
        Id = korisnik.Id,
        Ime = korisnik.Ime,
        Email = korisnik.Email,
        Lozinka = korisnik.Lozinka,
        Uloga = korisnik.Uloga
    };

    public async Task<ResponseKorisnikDto> Create(Korisnik korisnik)
    {
        if (string.IsNullOrWhiteSpace(korisnik.Ime)) throw new ArgumentException("[Korisnik/Create] Nije uneto ime.");
        if (string.IsNullOrWhiteSpace(korisnik.Email)) throw new ArgumentException("[Korisnik/Create] Nije unet email.");
        if (string.IsNullOrWhiteSpace(korisnik.Lozinka)) throw new ArgumentException("[Korisnik/Create] Nije uneta lozinka.");
        await Context.Korisnici.AddAsync(korisnik);
        await Context.SaveChangesAsync();
        return MapToDto(korisnik);
    }

    public async Task<ResponseKorisnikDto> Read(int korisnikId)
    {
        var korisnik = await Context.Korisnici.FindAsync(korisnikId);
        if (korisnik == null) throw new ArgumentException("[Korisnik/Read] Nema korisnika.");
        return MapToDto(korisnik);
    }

    public async Task<UpdateKorisnikDto> Read(string korisnikEmail)
    {
        if(string.IsNullOrWhiteSpace(korisnikEmail)) throw new ArgumentException("[Korisnik/Read] Nema email-a");
        var korisnik = await Context.Korisnici
                .Where(k => k.Email.Equals(korisnikEmail))
                .FirstOrDefaultAsync();
        if (korisnik == null) throw new ArgumentException("[Korisnik/Read] Nema korisnika sa tom email adresom.");
        return MapToUpdateDto(korisnik);
    }

    public async Task<ResponseKorisnikDto> Update(UpdateKorisnikDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Ime)) throw new ArgumentException("[Korisnik/Update] Nema imena");
        if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("[Korisnik/Update] Nema email-a");
        if (string.IsNullOrWhiteSpace(dto.Lozinka)) throw new ArgumentException("[Korisnik/Update] Nema nove lozinke");
        if (string.IsNullOrWhiteSpace(dto.Uloga)) throw new ArgumentException("[Korisnik/Update] Nema uloge");
        var korisnik = await Context.Korisnici.FindAsync(dto.Id);
        if (korisnik == null) throw new ArgumentException("[Korisnik/Update] Nema korisnika.");
        if(korisnik.Ime != dto.Ime)
            korisnik.Ime = dto.Ime;
        if(korisnik.Email != dto.Email)
            korisnik.Email = dto.Email;
        if(korisnik.Lozinka != dto.Lozinka)
            korisnik.Lozinka = dto.Lozinka;
        if(korisnik.Uloga != dto.Uloga)
            korisnik.Uloga = dto.Uloga;
        await Context.SaveChangesAsync();
        return MapToDto(korisnik);
    }

    public async Task<ResponseKorisnikDto> Delete(int korisnikId)
    {
        var korisnik = await Context.Korisnici.FindAsync(korisnikId);
        if (korisnik == null) throw new ArgumentException("[Korisnik/Delete] Nema korisnika");
        var karte = await Context.Karte
            .Where(k => k.KorisnikId == korisnikId)
            .ToListAsync();
        if (karte != null)
        {
            foreach (var k in karte)
            {
                Context.Karte.Remove(k);
            }
        }
        var obavestenja = await Context.ObavestenjaKorisnici
            .Where(ok => ok.KorisnikId == korisnikId)
            .ToListAsync();
        if (obavestenja != null)
        {
            foreach (var o in obavestenja)
            {
                Context.ObavestenjaKorisnici.Remove(o);
            }
        }
        var refreshTokeni = await Context.RefreshTokeni
            .Where(rt => rt.KorisnikId == korisnikId)
            .ToListAsync();
        if (refreshTokeni != null)
        {
            foreach (var rt in refreshTokeni)
            {
                Context.RefreshTokeni.Remove(rt);
            }
        }
        // Ovde moze da se napravi da se ocene ne brisu iz tabele
        // Da se zamene sa null ili nekom drugom vrednosti kako se
        // ne bi promenila ocena za neki polazak
        var ocene = await Context.OceneAutobusa
            .Where(o => o.KorisnikId == korisnikId)
            .ToListAsync();
        if (ocene != null)
        {
            foreach (var o in ocene)
            {
                Context.OceneAutobusa.Remove(o);
            }
        }
        Context.Korisnici.Remove(korisnik);
        await Context.SaveChangesAsync();
        return new ResponseKorisnikDto();
    }

    public async Task<ResponseKorisnikDto> Register(RegisterKorisnikDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Ime) ||
        string.IsNullOrWhiteSpace(dto.Email) ||
        string.IsNullOrWhiteSpace(dto.Lozinka))
            throw new ArgumentException("[Korisnik/Register] Sva polja su obavezna.");
        var exists = await Context.Korisnici.AnyAsync(k => k.Email == dto.Email);
        if (exists)
            throw new ArgumentException("[Korisnik/Register] Email je vec registrovan.");
        var korisnik = new Korisnik
        {
            Ime = dto.Ime,
            Email = dto.Email,
            Lozinka = dto.Lozinka,
            Uloga = "putnik"
        };
        await Context.Korisnici.AddAsync(korisnik);
        await Context.SaveChangesAsync();
        return MapToDto(korisnik);
    }

    public async Task<LoginResponseDto> Login(LoginKorisnikDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Lozinka))
        {
            throw new ArgumentException("Sva polja moraju biti popunjena");
        }

        var emailPostoji = await Context.Korisnici
            .Where(k => k.Email.Equals(dto.Email))
            .FirstOrDefaultAsync();

        if (emailPostoji == null)
        {
            throw new ArgumentException("Pogresan email");
        }

        var korisnik = await Context.Korisnici
            .Where(k => k.Email.Equals(dto.Email) &&
                        k.Lozinka.Equals(dto.Lozinka))
            .FirstOrDefaultAsync();

        if (korisnik == null)
        {
            throw new ArgumentException("Pogresna lozinka");
        }
        return await RefreshTokenService.GenerisiTokene(korisnik);
    }

    public async Task<ResponseKorisnikDto> ChangePassword(ChangePasswordKorisnikDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Nema emaila");
        if (string.IsNullOrWhiteSpace(dto.StaraLozinka)) throw new ArgumentException("Nema stare lozinke");
        if (string.IsNullOrWhiteSpace(dto.NovaLozinka)) throw new ArgumentException("Nema nove lozinke");
        if (dto.StaraLozinka == dto.NovaLozinka) throw new ArgumentException("Stara i nova lozinka ne smeju da budu iste");
        var korisnik = await Context.Korisnici
            .FirstOrDefaultAsync(k => k.Email == dto.Email && k.Lozinka == dto.StaraLozinka);
        if (korisnik == null) throw new ArgumentException("Pogrešna stara lozinka ili email adresa.");

        korisnik.Lozinka = dto.NovaLozinka;
        await Context.SaveChangesAsync();
        try
        {
            return MapToDto(korisnik);
        }
        catch (Exception)
        {
            throw new Exception("Greška prilikom mapiranja korisnika u DTO. Proverite MapToDto metodu.");
        }
    }

    public async Task<ResponseKorisnikDto> Update(int id, string ime)
    {
        var korisnik = await Context.Korisnici.FindAsync(id);
        if(korisnik == null) throw new Exception("Korisnik ne postoji.");
        if(string.IsNullOrWhiteSpace(ime)) throw new ArgumentException("[Korisnik/Update] Ime nije validno.");
        korisnik.Ime = ime;
        await Context.SaveChangesAsync();
        return MapToDto(korisnik);
    }

    public async Task<IList<ResponseKorisnikDto>> Read()
    {
        var listaKontrolora = new List<ResponseKorisnikDto>();
        var kontrolori = await Context.Korisnici
            .Where(k => k.Uloga.Equals("kontrolor"))
            .ToListAsync();
        if(!kontrolori.Any())
            throw new Exception("[KorisnikService/Read] Nema kontrolora.");
        foreach(var k in kontrolori)
        {
            listaKontrolora.Add(MapToDto(k));
        }
        return listaKontrolora;
    }
}