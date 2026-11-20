using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("[controller]")]

public class SedisteController : ControllerBase
{
    private ISedisteService SedisteService;
    public SedisteController(ISedisteService sedisteService)
    {
        SedisteService = sedisteService;
    }

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("DodajSediste")]
    public async Task<ActionResult> DodajSediste([FromBody] CreateSedisteDto dto)
    {
        try
        {
            var sediste = await SedisteService.Create(dto);
            return Ok(sediste);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("VratiSediste/{sedisteId}")]
    public async Task<ActionResult> VratiSediste(int sedisteId)
    {
        try
        {
            var sediste = await SedisteService.Read(sedisteId);
            return Ok(sediste);
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
    [HttpGet("VratiSedista/{autobusId}")]
    public async Task<ActionResult> VratiSedista(int autobusId)
    {
        try
        {
            var sedista = await SedisteService.ReadForBus(autobusId);
            return Ok(sedista);
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
    [HttpPut("IzmeniSediste")]
    public async Task<ActionResult> IzmeniSediste([FromBody] UpdateSedisteDto dto)
    {
        try
        {
            var sediste = await SedisteService.Update(dto);
            return Ok();
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
    [HttpDelete("ObrisiSediste/{sedisteId}")]
    public async Task<ActionResult> ObrisiSediste(int sedisteId)
    {
        try
        {
            var sediste = await SedisteService.Delete(sedisteId);
            return Ok(sediste);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}