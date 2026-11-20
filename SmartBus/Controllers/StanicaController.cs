[ApiController]
[Route("[controller]")]

public class StanicaController : ControllerBase
{
    private IStanicaService StanicaService;
    public StanicaController(IStanicaService stanicaService)
    {
        StanicaService = stanicaService;
    }

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("DodajStanicu")]
    public async Task<ActionResult> DodajStanicu([FromBody] CreateStanicaDto dto)
    {
        try
        {
            var stanica = await StanicaService.Create(dto);
            return Ok(stanica);
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
    [HttpGet("VratiStanicu/{stanicaId}")]
    public async Task<ActionResult> VratiStanicu(int stanicaId)
    {
        try
        {
            var stanica = await StanicaService.Read(stanicaId);
            return Ok(stanica);
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
    [HttpGet("VratiStanice")]
    public async Task<ActionResult> VratiStanice()
    {
        try
        {
            var stanice = await StanicaService.Read();
            return Ok(stanice);
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
    [HttpPut("IzmeniStanicu")]
    public async Task<ActionResult> IzmeniStanicu([FromBody] ResponseStanicaDto dto)
    {
        try
        {
            var stanica = await StanicaService.Update(dto);
            return Ok(stanica);
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
    [HttpDelete("ObrisiStanicu/{stanicaId}")]
    public async Task<ActionResult> ObrisiStanicu(int stanicaId)
    {
        try
        {
            var stanica = await StanicaService.Delete(stanicaId);
            return Ok(stanica);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}