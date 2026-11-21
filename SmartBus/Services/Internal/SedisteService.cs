using SmartBus.DTOs.SedisteDto;

namespace SmartBus.Services.Internal;

public class SedisteService : ISedisteService
{
    private AppDbContext Context;
    public SedisteService(AppDbContext context)
    {
        Context = context;
    }

    public ResponseSedisteDto MapToDto(Sediste sediste)
    {
        return new ResponseSedisteDto
        {
            Id = sediste.Id,
            Broj = sediste.Broj,
            Status = sediste.Status,
        };
    }
    public async Task<ResponseSedisteDto> Create(CreateSedisteDto dto)
    {
        if(dto.AutobusId == 0) throw new ArgumentException("Los id autobusa");
        if(dto.Broj <= 0) throw new ArgumentException("Los broj sedista");
        var autobus = await Context.Autobusi.FindAsync(dto.AutobusId);
        if(autobus == null) throw new Exception("Nije pronadjen autobus");
        var sediste = new Sediste
        {
            Broj = dto.Broj,
            Status = "slobodno",
            AutobusId = autobus.Id
        };
        await Context.Sedista.AddAsync(sediste);
        await Context.SaveChangesAsync();
        return MapToDto(sediste);
    }

    public async Task<ResponseSedisteDto> Read(int sedisteId)
    {
        var sediste = await Context.Sedista.FindAsync(sedisteId);
        if(sediste == null) throw new Exception("Nema sedista");
        return MapToDto(sediste);
    }

    public async Task<IList<ResponseSedisteDto>> ReadForBus(int autobusId)
    {
        var listaSedista = new List<ResponseSedisteDto>();
        var autobus = await Context.Autobusi.FindAsync(autobusId);
        if(autobus == null) throw new Exception("Nema autobusa");
        var sedista = await Context.Sedista
            .Where(s => s.AutobusId == autobusId)
            .ToListAsync();
        if(sedista == new List<Sediste>()) return listaSedista;
        foreach(var s in sedista)
        {
            listaSedista.Add(MapToDto(s));
        }
        return listaSedista;
    }

    public async Task<ResponseSedisteDto> Update(UpdateSedisteDto dto)
    {
        if(dto.Id == 0) throw new ArgumentException("Los id sedista");
        if(dto.NoviBroj <= 0) throw new ArgumentException("Los novi broj sedista");
        if(string.IsNullOrWhiteSpace(dto.NoviStatus)) throw new ArgumentException("Novi status ne sme biti prazan string");
        var sediste = await Context.Sedista.FindAsync(dto.Id);
        if(sediste == null) throw new Exception("Nema sedista");
        if(sediste.Broj != dto.NoviBroj) sediste.Broj = dto.NoviBroj;
        if(!sediste.Status!.Equals(dto.NoviStatus)) sediste.Status = dto.NoviStatus;
        await Context.SaveChangesAsync();
        return MapToDto(sediste);
    }

    public async Task<ResponseSedisteDto> Delete(int sedisteId)
    {
        var sediste = await Context.Sedista.FindAsync(sedisteId);
        if(sediste == null) throw new Exception("Nije pronadjeno sediste");
        var karte = await Context.Karte
            .Where(k => k.SedisteId == sedisteId)
            .ToListAsync();
        if(karte != new List<Karta>())
        {
            foreach(var k in karte)
            {
                Context.Karte.Remove(k);
            }
        }
        Context.Sedista.Remove(sediste);
        await Context.SaveChangesAsync();
        return new ResponseSedisteDto();
    }
}