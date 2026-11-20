using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("[controller]")]

public class KorisnikController : ControllerBase
{
    private IKorisnikService KorisnikService;
    private IRefreshTokenService RefreshTokenService;
    public KorisnikController(IKorisnikService korisnikService, IRefreshTokenService refreshTokenService)
    {
        KorisnikService = korisnikService;
        RefreshTokenService = refreshTokenService;
    }

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("DodajKorisnika")]
    public async Task<ActionResult> DodajKorisnika([FromBody] Korisnik korisnik)
    {
        try
        {
            var k = await KorisnikService.Create(korisnik);
            return Ok(k);
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
    [HttpGet("VratiKorisnika/{korisnikId}")]
    public async Task<ActionResult> VratiKorisnika(int korisnikId)
    {
        try
        {
            var k = await KorisnikService.Read(korisnikId);
            return Ok(k);
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
    [HttpGet("VratiKorisnikaEmail/{email}")]
    public async Task<ActionResult> VratiKorisnikaEmail(string email)
    {
        try
        {
            var k = await KorisnikService.Read(email);
            return Ok(k);
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
    [HttpPut("IzmeniKorisnika")]
    public async Task<ActionResult> IzmeniKorisnika([FromBody] UpdateKorisnikDto dto)
    {
        try
        {
            var k = await KorisnikService.Update(dto);
            return Ok(k);
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
    [HttpPut("IzmeniKorisnickoIme/{id}/{ime}")]
    public async Task<ActionResult> IzmeniKorisnickoIme(int id, string ime)
    {
        try
        {
            var k = await KorisnikService.Update(id, ime);
            return Ok(k);
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
    [HttpDelete("ObrisiKorisnika/{korisnikId}")]
    public async Task<ActionResult> ObrisiKorisnika(int korisnikId)
    {
        try
        {
            var k = await KorisnikService.Delete(korisnikId);
            return Ok(k);
        }
        catch (Exception e)
        {
            return BadRequest(e.InnerException?.Message);
        }
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("Register")]
    public async Task<ActionResult> Register([FromBody] RegisterKorisnikDto dto)
    {
        try
        {
            var k = await KorisnikService.Register(dto);
            return Ok(k);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("Login")]
    public async Task<ActionResult> Login([FromBody] LoginKorisnikDto dto)
    {
        try
        {
            var k = await KorisnikService.Login(dto);
            return Ok(k);
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
    [HttpPut("PromeniLozinku")]
    public async Task<ActionResult> PromeniLozinku([FromBody] ChangePasswordKorisnikDto dto)
    {
        try
        {
            var k = await KorisnikService.ChangePassword(dto);
            return Ok(k);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("Refresh")]
    public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        try
        {
            var rezultat = await RefreshTokenService.OsveziToken(dto.RefreshToken);
            return Ok(rezultat);
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
    [HttpPost("Logout")]
    public async Task<ActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
    {
        try
        {
            await RefreshTokenService.OpozoviToken(dto.RefreshToken);
            return Ok("Uspesno odjavljivanje.");
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
    [HttpGet("VratiKontrolore")]
    public async Task<ActionResult> VratiKontrolore()
    {
        try
        {
            var kontrolori = await KorisnikService.Read();
            return Ok(kontrolori);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}