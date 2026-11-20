
namespace SmartBus.Services.Internal;

public class AutobusService : IAutobusService
{
    private AppDbContext Context;
    private ISedisteService SedisteService;
    public AutobusService(AppDbContext context, ISedisteService sedisteService)
    {
        Context = context;
        SedisteService = sedisteService;
    }

    private static ResponseAutobusDto MapToDto(Autobus autobus) => new()
    {
        Id = autobus.Id,
        Model = autobus.Model,
        BrojSedista = autobus.BrojSedista,
        Prevoznik = autobus.Prevoznik ?? "null",
        OceneAutobusa = autobus.OcenaAutobus != null
        ? autobus.OcenaAutobus.Select(o => new OcenaAutobusDto 
        { 
            Ocena = o.Ocena, 
            AutobusId = o.AutobusId, 
            KorisnikId = o.KorisnikId 
        }).ToList()
    : new List<OcenaAutobusDto>()
    };

    public async Task<ResponseAutobusDto> Create(Autobus autobus)
    {
        if (string.IsNullOrWhiteSpace(autobus.Model)) throw new ArgumentException("[Autobus/Create] Nije unet model");
        if (autobus.BrojSedista < 6 || autobus.BrojSedista > 100) throw new ArgumentException("[Autobus/Create] Pogresno unet broj sedista");
        if (string.IsNullOrWhiteSpace(autobus.Prevoznik)) throw new ArgumentException("[Autobus/Create] Nije unet prevoznik");
        var a = await Context.Autobusi.AddAsync(autobus);
        await Context.SaveChangesAsync();
        for(int s = 1; s <= autobus.BrojSedista; s++)
        {
            var dto = new CreateSedisteDto
            {
                AutobusId = a.Entity.Id,
                Broj = s
            };
            await SedisteService.Create(dto);
        }
        return MapToDto(autobus);
    }

    public async Task<ResponseAutobusDto> Read(int autobusId)
    {
        var autobus = await Context.Autobusi.FindAsync(autobusId);
        if (autobus == null) throw new Exception("[Autobus/Read] Nema autobusa.");
        return MapToDto(autobus);
    }

    public async Task<IList<ResponseAutobusDto>> Read()
    {
        var listaAutobusa = new List<ResponseAutobusDto>();
        var autobusi = await Context.Autobusi.ToListAsync();
        if(autobusi == null) return listaAutobusa;
        foreach(var a in autobusi)
        {
            listaAutobusa.Add(MapToDto(a));
        }
        return listaAutobusa;
    }

    public async Task<ResponseAutobusDto> Update(ResponseAutobusDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Model)) throw new ArgumentException("[Autobus/Update] Nije unet model");
        if (dto.BrojSedista < 6 || dto.BrojSedista > 100) throw new ArgumentException("[Autobus/Update] Pogresno unet broj sedista");
        if (string.IsNullOrWhiteSpace(dto.Prevoznik)) throw new ArgumentException("[Autobus/Update] Nije unet prevoznik");
        var autobus = await Context.Autobusi.FindAsync(dto.Id);
        if (autobus == null) throw new Exception("[Autobus/Update] Nema autobusa.");
        autobus.Model = dto.Model;
        autobus.BrojSedista = dto.BrojSedista;
        autobus.Prevoznik = dto.Prevoznik;
        var sedista = await Context.Sedista
            .Where(s => s.AutobusId == autobus.Id)
            .ToListAsync();
        if(sedista != null)
        {
            foreach(var s in sedista)
            {
                await SedisteService.Delete(s.Id);
            }
        }
        for(int i = 1; i <= dto.BrojSedista; i++)
        {
            var dtoSediste = new CreateSedisteDto
            {
                AutobusId = autobus.Id,
                Broj = i
            };
            await SedisteService.Create(dtoSediste);
        }
        await Context.SaveChangesAsync();
        return MapToDto(autobus);
    }

    public async Task<ResponseAutobusDto> Delete(int autobusId)
    {
        var autobus = await Context.Autobusi.FindAsync(autobusId);
        if (autobus == null) throw new Exception("[Autobus/Delete] Nema autobusa.");
        var ocene = await Context.OceneAutobusa
            .Where(a => a.AutobusId == autobusId)
            .ToListAsync();
        if (ocene != null)
        {
            foreach (var o in ocene)
            {
                Context.OceneAutobusa.Remove(o);
            }
        }
        var polasci = await Context.Polasci
            .Where(p => p.AutobusId == autobusId)
            .ToListAsync();
        if (polasci != null)
        {
            foreach (var p in polasci)
            {
                Context.Polasci.Remove(p);
            }
        }
        var sedista = await Context.Sedista
            .Where(s => s.AutobusId == autobusId)
            .ToListAsync();
        if(sedista != null)
        {
            foreach(var s in sedista)
            {
                Context.Sedista.Remove(s);
            }
        }
        Context.Autobusi.Remove(autobus);
        await Context.SaveChangesAsync();
        return new ResponseAutobusDto();
    }

}