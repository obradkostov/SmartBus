namespace SmartBus.Services.Internal;

public class PolazakService : IPolazakService
{
    private AppDbContext Context;
    public PolazakService(AppDbContext context)
    {
        Context = context;
    }
    private static ResponsePolazakDto MapToDto(Polazak polazak) => new()
    {
        Id = polazak.Id,
        Autobus = new ResponseAutobusDto
        {
            Id = polazak.Autobus.Id,
            Prevoznik = polazak.Autobus.Prevoznik!,
            BrojSedista = polazak.Autobus.BrojSedista,
            OceneAutobusa = polazak.Autobus.OcenaAutobus.Select(o => new OcenaAutobusDto
            {
                AutobusId = o.AutobusId,
                KorisnikId = o.KorisnikId,
                Ocena = o.Ocena
            }).ToList()
        },
        StanicaOd = polazak.StanicaOdNavigation,
        StanicaDo = polazak.StanicaDoNavigation,
        VremePolaska = polazak.VremePolaska,
        Peron = polazak.Peron,
        Cena = polazak.Cena,
        Status = polazak.Status
    };

    public async Task<ResponsePolazakDto> Create(CreatePolazakDto dto)
    {
        if (dto.AutobusId == 0) throw new ArgumentException("[PolazakService/Create] Los id autobusa.");
        var autobus = await Context.Autobusi.FindAsync(dto.AutobusId);
        if (autobus == null) throw new Exception("[PolazakService/Create] Nema autobusa.");
        if (dto.StanicaOdId == dto.StanicaDoId) throw new ArgumentException("[Polazak/Create] Stanice ne smeju biti iste");
        if (dto.StanicaOdId == 0) throw new ArgumentException("[PolazakService/Create] Los id stanice polaska.");
        var stanicaod = await Context.Stanice.FindAsync(dto.StanicaOdId);
        if (stanicaod == null) throw new Exception("[PolazakService/Create] Nema pocetne stanice.");
        if (dto.StanicaDoId == 0) throw new ArgumentException("[PolazakService/Create] Los id stanice dolaska.");
        var stanicado = await Context.Stanice.FindAsync(dto.StanicaDoId);
        if (stanicado == null) throw new Exception("[PolazakService/Create] Nema krajnje stanice.");
        if (dto.Cena <= 0) throw new Exception("[PolazakService/Create] Cena nije validna.");
        if (dto.Peron <= 0 || dto.Peron > stanicaod.BrojPerona) throw new Exception("[PolazakService/Create] Broj perona nije validan.");
        if (dto.VremePolaska < DateTime.Now) throw new Exception("[PolazakService/Create] Datum nije validan.");
        Polazak polazak = new Polazak
        {
            AutobusId = dto.AutobusId,
            Autobus = autobus,
            StanicaOd = dto.StanicaOdId,
            StanicaOdNavigation = stanicaod,
            StanicaDo = dto.StanicaDoId,
            StanicaDoNavigation = stanicado,
            Peron = dto.Peron,
            Cena = dto.Cena,
            VremePolaska = dto.VremePolaska
        };
        await Context.Polasci.AddAsync(polazak);
        await Context.SaveChangesAsync();
        return MapToDto(polazak);
    }

    // TODO: Ne radi, prepraviti delete funkciju
    public async Task<ResponsePolazakDto> Delete(int polazakId)
    {
        var polazak = await Context.Polasci.FindAsync(polazakId);
        if (polazak == null) throw new Exception("[PolazakService/Delete] Nema polaska.");
        var karte = await Context.Karte
            .Where(p => p.PolazakId == polazakId)
            .ToListAsync();
        if (karte != null)
        {
            foreach (var k in karte)
            {
                Context.Karte.Remove(k);
            }
        }
        var obavestenja = await Context.Obavestenja
            .Where(o => o.PolazakId == polazakId)
            .ToListAsync();
        if (obavestenja != null)
        {
            foreach (var o in obavestenja)
            {
                Context.Obavestenja.Remove(o);
            }
        }
        Context.Polasci.Remove(polazak);
        await Context.SaveChangesAsync();
        return new ResponsePolazakDto();
    }

    /* public async Task<IList<ResponsePolazakDto>> Filter(string gradPolaska, string gradDolaska, DateTime datum)
    {
        if(string.IsNullOrWhiteSpace(gradPolaska)) throw new ArgumentException("Grad polaska nije odabran");
        if(string.IsNullOrWhiteSpace(gradDolaska)) throw new ArgumentException("Grad dolaska nije odabran");
        if(new DateTime() > datum) throw new ArgumentException("Datum mora biti validan");

        var polasci = await Context.Polasci
            .Include(p => p.Autobus)
            .ThenInclude(a => a.OcenaAutobus)
            .Include(p => p.StanicaOdNavigation)
            .Include(p => p.StanicaDoNavigation)
            //ovaj where je KATASTROFA
            //jer ima 5 poredjanja stringova
            //treba optimizovati ovaj deo nekako (preko nekih id-jeva)
            .Where(p => p.StanicaOdNavigation.Naziv.Equals(gradPolaska) &&
                        p.StanicaDoNavigation.Naziv.Equals(gradDolaska) &&
                        p.VremePolaska.Date.Equals(datum) &&
                        (p.Status.Equals("aktivan") ||
                        p.Status.Equals("izmenjen")))
            .ToListAsync();
        if (polasci == null)
        {
            throw new ArgumentException("Nema polazaka.");
        }

        IList<ResponsePolazakDto> listaPolazaka = new List<ResponsePolazakDto>();
        foreach (var p in polasci)
        {
            listaPolazaka.Add(MapToDto(p));
        }
        return listaPolazaka;
    } */
    public async Task<IList<ResponsePolazakDto>> Filter(string gradPolaska, string gradDolaska, DateTime datum)
    {
        if (string.IsNullOrWhiteSpace(gradPolaska)) throw new ArgumentException("[PolazakService/Filter] Grad polaska nije odabran");
        if (string.IsNullOrWhiteSpace(gradDolaska)) throw new ArgumentException("[PolazakService/Filter] Grad dolaska nije odabran");
        if (datum == default) throw new ArgumentException("[PolazakService/Filter] Datum mora biti validan");

        var samoDatum = datum.Date;

        var polasci = await Context.Polasci
            .Include(p => p.Autobus)
                .ThenInclude(a => a.OcenaAutobus)
            .Include(p => p.StanicaOdNavigation)
            .Include(p => p.StanicaDoNavigation)
            .Where(p => p.StanicaOdNavigation.Naziv == gradPolaska &&
                        p.StanicaDoNavigation.Naziv == gradDolaska &&
                        p.VremePolaska.Date == samoDatum &&
                        (p.Status == "aktivan" || p.Status == "izmenjen"))
            .ToListAsync();

        if (polasci == null || !polasci.Any())
        {
            return new List<ResponsePolazakDto>();
        }

        return polasci.Select(p => MapToDto(p)).ToList();
    }
    public async Task<ResponsePolazakDto> Read(int polazakId)
    {
        var polazak = await Context.Polasci
                .Include(p => p.Autobus)
                .Include(p => p.StanicaOdNavigation)
                .Include(p => p.StanicaDoNavigation)
                .FirstOrDefaultAsync(p => p.Id == polazakId);
        if (polazak == null) throw new ArgumentException("[PolazakService/Read] Nema polaska");
        return MapToDto(polazak);
    }

    public async Task<ResponsePolazakPagingDto> ReadToday(int strana, int velicinaStrane)
    {
        var listaPolasciDana = new ResponsePolazakPagingDto();
        var polasciCount = await Context.Polasci
            .Where(p => p.VremePolaska.Date == DateTime.Today.Date &&
                p.VremePolaska.TimeOfDay >= DateTime.Now.TimeOfDay)
            .CountAsync();
        if (polasciCount == 0) return listaPolasciDana;
        var polasciDana = await Context.Polasci
            .Include(p => p.Autobus)
            .Include(p => p.StanicaOdNavigation)
            .Include(p => p.StanicaDoNavigation)
            .Where(p => p.VremePolaska.Date == DateTime.Today.Date &&
                p.VremePolaska.TimeOfDay >= DateTime.Now.TimeOfDay)
            .Skip(velicinaStrane * (strana - 1))
            .Take(velicinaStrane)
            .ToListAsync();
        if (polasciDana == new List<Polazak>()) return listaPolasciDana;
        foreach (var polazak in polasciDana)
        {
            listaPolasciDana.Polasci.Add(MapToDto(polazak));
        }
        listaPolasciDana.TrenutnaStrana = strana;
        listaPolasciDana.UkupnoPolazaka = polasciCount;
        listaPolasciDana.UkupnoStrana = (int)Math.Ceiling((double)polasciCount / velicinaStrane);
        return listaPolasciDana;
    }

    public async Task<IList<ResponsePolazakDto>> Read()
    {
        var polasci = await Context.Polasci
            .Include(p => p.Autobus)
                .ThenInclude(a => a.OcenaAutobus)
            .Include(p => p.StanicaOdNavigation)
            .Include(p => p.StanicaDoNavigation)
            .ToListAsync();

        if (polasci == null || !polasci.Any())
        {
            return new List<ResponsePolazakDto>();
        }

        return polasci.Select(p => MapToDto(p)).ToList();
    }

    public async Task<ResponsePolazakDto> Update(UpdatePolazakDto dto)
    {
        var polazak = await Context.Polasci.FindAsync(dto.Id);
        if (polazak == null) throw new ArgumentException("[PolazakService/Update] Nema polaska");
        if (polazak.AutobusId != dto.AutobusId)
            polazak.AutobusId = dto.AutobusId;
        if (polazak.StanicaOd != dto.StanicaOdId)
            polazak.StanicaOd = dto.StanicaOdId;
        if (polazak.StanicaDo != dto.StanicaDoId)
            polazak.StanicaDo = dto.StanicaDoId;
        if (polazak.VremePolaska != dto.VremePolaska)
        {
            if (dto.VremePolaska < DateTime.Now) throw new ArgumentException("[PolazakService/Update] Vreme polaska nije validno");
            polazak.VremePolaska = dto.VremePolaska;
        }
        if (polazak.Peron != dto.Peron)
        {
            if (dto.Peron <= 0 && dto.Peron > polazak.StanicaOdNavigation.BrojPerona) throw new ArgumentException("[Polazak/Update] Broj perona nije validan.");
            polazak.Peron = dto.Peron;
        }
        if (polazak.Cena != dto.Cena)
        {
            if (dto.Cena <= 0) throw new ArgumentException("[PolazakService/Update] Cena nije validna.");
            polazak.Cena = dto.Cena;
        }
        await Context.SaveChangesAsync();
        return MapToDto(polazak);
    }
}