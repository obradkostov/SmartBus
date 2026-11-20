
namespace SmartBus.Services.Internal;

public class StanicaService : IStanicaService
{
    private AppDbContext Context;
    public StanicaService(AppDbContext context)
    {
        Context = context;
    }

    public ResponseStanicaDto MapToDto(Stanica stanica)
    {
        return new ResponseStanicaDto
        {
            Id = stanica.Id,
            Naziv = stanica.Naziv,
            BrojPerona = stanica.BrojPerona
        };
    }
    public async Task<ResponseStanicaDto> Create(CreateStanicaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Naziv)) throw new ArgumentException("Nema naziva");
        if (dto.BrojPerona <= 0) throw new ArgumentException("Nevalidan broj perona");
        var stanica = await Context.Stanice
            .Where(s => s.Naziv.Equals(dto.Naziv))
            .FirstOrDefaultAsync();
        if (stanica != null) throw new Exception("Takva stanica vec postoji");
        var s = new Stanica
        {
            Naziv = dto.Naziv,
            BrojPerona = dto.BrojPerona
        };
        await Context.Stanice.AddAsync(s);
        await Context.SaveChangesAsync();
        return MapToDto(s);
    }

    public async Task<ResponseStanicaDto> Read(int stanicaId)
    {
        var stanica = await Context.Stanice
            .FindAsync(stanicaId);
        if (stanica == null) throw new Exception("Nema stanice");
        return MapToDto(stanica);
    }

    public async Task<IList<ResponseStanicaDto>> Read()
    {
        var listaStanica = new List<ResponseStanicaDto>();
        var stanice = await Context.Stanice.ToListAsync();
        if (stanice == null) return listaStanica;
        foreach (var s in stanice)
        {
            listaStanica.Add(MapToDto(s));
        }
        return listaStanica;
    }

    public async Task<ResponseStanicaDto> Update(ResponseStanicaDto dto)
    {
        if (dto.Id == 0) throw new ArgumentException("Pogresan id stanice");
        var stanica = await Context.Stanice.FindAsync(dto.Id);
        if (stanica == null) throw new Exception("Nema stanica");
        if (string.IsNullOrWhiteSpace(dto.Naziv)) throw new ArgumentException("Nema naziva");
        var testNaziva = await Context.Stanice
            .Where(s => s.Naziv.Equals(dto.Naziv))
            .FirstOrDefaultAsync();
        if (testNaziva != null) throw new Exception("Stanica sa tim imenom vec postoji");
        if (dto.BrojPerona <= 0) throw new ArgumentException("Nevalidan broj perona");
        stanica.Naziv = dto.Naziv;
        stanica.BrojPerona = dto.BrojPerona;
        await Context.SaveChangesAsync();
        return MapToDto(stanica);
    }

    public async Task<ResponseStanicaDto> Delete(int stanicaId)
    {
        var stanica = await Context.Stanice.FindAsync(stanicaId);
        if (stanica == null) throw new ArgumentException("Nema stanice sa tim id-jem");
        Context.Stanice.Remove(stanica);
        await Context.SaveChangesAsync();
        return new ResponseStanicaDto();
    }
}