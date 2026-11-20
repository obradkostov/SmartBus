using SmartBus.Dtos;
using Stripe;

namespace SmartBus.Services.Internal;

public class DnevniIzvestajService : IDnevniIzvestajService
{
    private AppDbContext Context;

    public DnevniIzvestajService(AppDbContext context)
    {
        Context = context;
    }

    private ResponseDnevniIzvestajDto MapToDto(Korisnik kontrolor, DnevniIzvestaj izvestaj)
    {
        return new ResponseDnevniIzvestajDto
        {
            Ime = kontrolor.Ime,
            Email = kontrolor.Email,
            Datum = izvestaj.Datum,
            Smena = izvestaj.Smena,
            Relacije = izvestaj.Relacije,
            PregledaneKarte = izvestaj.PregledaneKarte,
            ValidneKarte = izvestaj.ValidneKarte,
            NevalidneKarte = izvestaj.NevalidneKarte,
            PutniciBezKarte = izvestaj.PutniciBezKarte,
            PrekrsajniNalozi = izvestaj.PrekrsajniNalozi,
            IznosPlacenihKazni = izvestaj.IznosPlacenihKazni,
            OdbijenaPlacanja = izvestaj.OdbijenaPlacanja,
            PrijavljenaLica = izvestaj.PrijavljenaLica,
            VerbalniIncidenti = izvestaj.VerbalniIncidenti,
            FizickiIncidenti = izvestaj.FizickiIncidenti,
            PoziviPolicije = izvestaj.PoziviPolicije,
            OpisIncidenata = izvestaj.OpisIncidenata ?? "NEMA",
            NepravilnostiNaVozilu = izvestaj.NepravilnostiNaVozilu ?? "NEMA",
            ProblemiSaSistemom = izvestaj.ProblemiSaSistemom ?? "NEMA"
        };
    }
    public async Task<ResponseDnevniIzvestajDto> Create(CreateDnevniIzvestajDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("Nema emaila.");
        var kontrolor = await Context.Korisnici
            .Where(k => k.Email.Equals(dto.Email) && k.Uloga.Equals("kontrolor"))
            .FirstOrDefaultAsync();
        if (kontrolor == null)
            throw new Exception("Pogresan email.");
        if (!dto.Smena.Equals("jutarnja") &&
            !dto.Smena.Equals("popodnevna") &&
            !dto.Smena.Equals("nocna"))
            throw new ArgumentException("Pogresna smena.");
        if (string.IsNullOrWhiteSpace(dto.Relacije))
            throw new ArgumentException("Nema relacija.");
        if (dto.PregledaneKarte < 0)
            throw new ArgumentException("Pogresan broj pregledanih karata.");
        if (dto.ValidneKarte < 0)
            throw new ArgumentException("Pogresan broj validnih karata.");
        if (dto.NevalidneKarte < 0)
            throw new ArgumentException("Pogresan broj nevalidnih karata.");
        if (dto.PutniciBezKarte < 0)
            throw new ArgumentException("Pogresan broj putnika bez karte.");
        if (dto.PrekrsajniNalozi < 0)
            throw new ArgumentException("Pogresan broj prekrsajnih naloga.");
        if (dto.IznosPlacenihKazni < 0)
            throw new ArgumentException("Pogresan iznos placenih kazni.");
        if (dto.OdbijenaPlacanja < 0)
            throw new ArgumentException("Pogresan broj odbijenih placanja.");
        if (dto.PrijavljenaLica < 0)
            throw new ArgumentException("Pogresan broj prijavljenih lica.");
        if (dto.VerbalniIncidenti < 0)
            throw new ArgumentException("Pogresan broj verbalnih incidenata.");
        if (dto.FizickiIncidenti < 0)
            throw new ArgumentException("Pogresan broj fizickih incidenata.");
        if (dto.PoziviPolicije < 0)
            throw new ArgumentException("Pogresan broj poziva policije.");
        if (string.IsNullOrWhiteSpace(dto.OpisIncidenata))
            dto.OpisIncidenata = "NEMA";
        if (string.IsNullOrWhiteSpace(dto.NepravilnostiNaVozilu))
            dto.NepravilnostiNaVozilu = "NEMA";
        if (string.IsNullOrWhiteSpace(dto.ProblemiSaSistemom))
            dto.ProblemiSaSistemom = "NEMA";
        var izvestaj = new DnevniIzvestaj
        {
            KontrolorId = kontrolor.Id,
            Datum = DateOnly.FromDateTime(DateTime.Now),
            Smena = dto.Smena,
            Relacije = dto.Relacije,
            PregledaneKarte = dto.PregledaneKarte,
            ValidneKarte = dto.ValidneKarte,
            NevalidneKarte = dto.NevalidneKarte,
            PutniciBezKarte = dto.PutniciBezKarte,
            PrekrsajniNalozi = dto.PrekrsajniNalozi,
            IznosPlacenihKazni = dto.IznosPlacenihKazni,
            OdbijenaPlacanja = dto.OdbijenaPlacanja,
            PrijavljenaLica = dto.PrijavljenaLica,
            VerbalniIncidenti = dto.VerbalniIncidenti,
            FizickiIncidenti = dto.FizickiIncidenti,
            PoziviPolicije = dto.PoziviPolicije,
            OpisIncidenata = dto.OpisIncidenata,
            NepravilnostiNaVozilu = dto.NepravilnostiNaVozilu,
            ProblemiSaSistemom = dto.ProblemiSaSistemom
        };
        await Context.DnevniIzvestaji.AddAsync(izvestaj);
        await Context.SaveChangesAsync();
        return MapToDto(kontrolor, izvestaj);
    }

    public async Task<ListaIzvestajaDto> Read(FiltrirajIzvestajeDto dto)
    {
        var listaIzvestajaDto = new ListaIzvestajaDto();
        if (dto.Strana <= 0) throw new ArgumentException("[DnevniIzvestaj/Read] Broj strane mora biti veci od 0.");
        if (dto.VelicinaStrane <= 0) throw new ArgumentException("[DnevniIzvestaj/Read] Velicina strane mora biti veca od 0.");

        var query = Context.DnevniIzvestaji.AsQueryable();

        if (dto.Datum.HasValue)
            query = query.Where(i => i.Datum == dto.Datum);

        if (dto.KontrolorId.HasValue)
            query = query.Where(i => i.KontrolorId == dto.KontrolorId);

        if (!string.IsNullOrWhiteSpace(dto.Smena))
            query = query.Where(i => i.Smena == dto.Smena);

        var izvestajCount = await query.CountAsync();

        var izvestaji = await query
            .Skip((dto.Strana - 1) * dto.VelicinaStrane)
            .Take(dto.VelicinaStrane)
            .ToListAsync();

        foreach (var i in izvestaji)
        {
            var kontrolor = await Context.Korisnici.FindAsync(i.KontrolorId);
            if (kontrolor == null) throw new Exception("[DnevniIzvestaj/Read] Nema kontrolora.");
            listaIzvestajaDto.ListaIzvestaja.Add(MapToDto(kontrolor, i));
        }

        listaIzvestajaDto.TrenutnaStrana = dto.Strana;
        listaIzvestajaDto.UkupnoIzvestaja = izvestajCount;
        listaIzvestajaDto.UkupnoStrana = (int)Math.Ceiling((double)izvestajCount / dto.VelicinaStrane);

        return listaIzvestajaDto;
    }

    public async Task<StatistikaDanaDto> Statistics()
    {
        var statistika = new StatistikaDanaDto();
        statistika.IzvestajiDanas = await Context.DnevniIzvestaji
            .CountAsync(i => i.Datum == DateOnly.FromDateTime(DateTime.Today));
        statistika.PregledaneKarte = await Context.DnevniIzvestaji
            .SumAsync(i => i.ValidneKarte + i.NevalidneKarte);
        statistika.PrekrsajniNalozi = await Context.DnevniIzvestaji
            .SumAsync(i => i.PrekrsajniNalozi);
        statistika.NaplaceneKazne = await Context.DnevniIzvestaji
            .SumAsync(i => i.IznosPlacenihKazni);
        return statistika;
    }
}