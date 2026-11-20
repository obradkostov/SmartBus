using System.Security.Claims;

[ApiController]
[Route("[controller]")]
public class KartaController : ControllerBase
{
    private IKartaService KartaService;
    private ISignatureService SignatureService;

    public KartaController(IKartaService kartaService, ISignatureService signatureService)
    {
        KartaService = kartaService;
        SignatureService = signatureService;
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("DodajKartu")]
    public async Task<ActionResult> DodajKartu([FromBody] CreateKartaDto dto)
    {
        try
        {
            var karta = await KartaService.Create(dto);
            return Ok(karta);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("IstorijaKarata/{korisnikId}")]
    public async Task<ActionResult> IstorijaKarata(int korisnikId)
    {
        try
        {
            var karte = await KartaService.Read(korisnikId);
            return Ok(karte);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut("IzmeniKartu")]
    public async Task<ActionResult> IzmeniKartu([FromBody] UpdateKartaDto dto)
    {
        try
        {
            var karta = await KartaService.Update(dto);
            return Ok(karta);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("ObrisiKartu/{kartaId}")]
    public async Task<ActionResult> ObrisiKartu(int kartaId)
    {
        try
        {
            var karta = await KartaService.Delete(kartaId);
            return Ok(karta);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("ZauzetaSedista/{polazakId}")]
    public async Task<ActionResult> ZauzetaSedista(int polazakId)
    {
        try
        {
            var zauzetaSedista = await KartaService.ZauzetaSedista(polazakId);
            return Ok(zauzetaSedista);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "kontrolor")]
    [HttpPost("ValidirajKartu")]
    public async Task<ActionResult> ValidirajKartu([FromBody] ValidacijaKarteDto dto)
    {
        try
        {
            var kontrolorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var validacija = await KartaService.ValidirajKartu(dto, kontrolorId);
            return Ok(validacija);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpGet("Signature/{kartaId}")]
    public async Task<IActionResult> GetSignature(int kartaId)
    {
        var korisnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var signature = await KartaService.GetSignature(kartaId, korisnikId);

        if (signature == null)
            return NotFound("Karta ne postoji.");

        return Ok(new { signature });
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut("OtkaziKartu/{kartaId}")]
    public async Task<ActionResult> OtkaziKartu(int kartaId)
    {
        try
        {
            var uspesno = await KartaService.OtkaziKartu(kartaId);
            return Ok(new { poruka = "Karta je uspešno otkazana." });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // [HttpPost("DodajKartu")]
    // public async Task<ActionResult> ImeMetoda([FromBody] Karta karta)
    // {
    //     try
    //     {
    //         var sedisteZauzeto = await Context.Karte
    //         .AnyAsync(k => k.PolazakId == karta.PolazakId && k.Sediste == karta.Sediste);

    //     if (sedisteZauzeto)
    //     {
    //         return BadRequest("Sedište je već zauzeto za ovaj polazak! Molimo izaberite drugo.");
    //     }
    //         // TO DO: uraditi provere atributa
    //         await Context.Karte.AddAsync(karta);
    //         await Context.SaveChangesAsync();
    //         return Ok();
    //     }
    //     catch(Exception e)
    //     {
    //         var kompletnaGreska = e.InnerException != null ? e.InnerException.Message : e.Message;
    //         return BadRequest(kompletnaGreska);
    //     }
    // }

    // [HttpGet("VratiKartu/{kartaId}")]
    // public async Task<ActionResult> VratiKarte(int kartaId)
    // {
    //     try
    //     {
    //         var karta = await Context.Karte.FindAsync(kartaId);
    //         if(karta == null)
    //             return BadRequest("Nije pronadjena karta");
    //         return Ok(karta);
    //     }
    //     catch(Exception e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }

    // [HttpGet("VratiKarteKorisnika/{korisnikId}")]
    // public async Task<ActionResult> VratiKarteKorisnika(int korisnikId)
    // {
    //     try
    //     {
    //         var karte = await Context.Karte
    //             .Where(k => k.KorisnikId == korisnikId)
    //             .ToListAsync();
    //         if(karte == null)
    //             return BadRequest("Nije pronadjena ni jedna karta");
    //         return Ok(karte);
    //     }
    //     catch(Exception e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }

    // [HttpGet("MojeKarte/{korisnikId}")]
    // public async Task<IActionResult> VratiAktivneKarte(int korisnikId)
    // {
    //     var trenutnoVreme = DateTime.Now;
    //     var karte = await Context.Karte
    //         .Include(k => k.Polazak)
    //             .ThenInclude(p => p.StanicaOdNavigation)
    //         .Include(k => k.Polazak)
    //             .ThenInclude(p => p.StanicaDoNavigation)
    //         .Include(k => k.Polazak)
    //             .ThenInclude(p => p.Autobus)
    //         .Where(k => k.KorisnikId == korisnikId && k.Polazak.VremePolaska >= trenutnoVreme) // KLJUČNI USLOV
    //         .ToListAsync();
    //     return Ok(karte);
    // }
    // [HttpGet("VratiKartePolaska/{polazakId}")]
    // public async Task<ActionResult> VratiKartePolaska(int polazakId)
    // {
    //     try
    //     {
    //         var karte = await Context.Karte
    //             .Where(k => k.PolazakId == polazakId)
    //             .ToListAsync();
    //         if(karte == null)
    //         {
    //             return BadRequest("Nije pronadjena ni jedna karta");
    //         }
    //         return Ok(karte);
    //     }
    //     catch(Exception e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }
}
