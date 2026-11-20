namespace SmartBus.Services.Internal;

public class OcenaAutobusService : IOcenaAutobusService
{
    private readonly AppDbContext Context;
    public OcenaAutobusService(AppDbContext context)
    {
        Context = context;
    }
    public async Task<double> Create(OcenaAutobusDto dto)
    {
        var autobus = await Context.Autobusi.FindAsync(dto.AutobusId);
        if (autobus == null) throw new Exception("[OcenaAutobus/Create] Ne postoji autobus.");
        var korisnik = await Context.Korisnici.FindAsync(dto.KorisnikId);
        if (korisnik == null) throw new Exception("[OcenaAutobus/Create] Ne postoji korisnik.");
        var ocena = await Context.OceneAutobusa
            .Where(o => o.KorisnikId == dto.KorisnikId && o.AutobusId == dto.AutobusId)
            .FirstOrDefaultAsync();
        if(ocena != null) throw new Exception("[OcenaAutobus/Create] Korisnik je vec ocenio autobus.");
        if (dto.Ocena < 1 || dto.Ocena > 6) throw new Exception("[OcenaAutobus/Create] Ocena nije validan broj.");
        var novaOcena = new OcenaAutobus
        {
            AutobusId = dto.AutobusId,
            KorisnikId = dto.KorisnikId,
            Ocena = dto.Ocena
        };
        await Context.OceneAutobusa.AddAsync(novaOcena);
        await Context.SaveChangesAsync();
        // Vraca se nova prosecna ocena
        var ocene = await Context.OceneAutobusa
            .Where(o => o.AutobusId == dto.AutobusId)
            .ToListAsync();
        int brojOcena = 0;
        int sumaOcena = 0;
        foreach(var o in ocene)
        {
            sumaOcena += o.Ocena;
            brojOcena ++;
        }
        return(sumaOcena/brojOcena);
    }
}