

[ApiController]
[Route("[controller]")]

public class PolazakController : ControllerBase
{
    private IPolazakService PolazakService;
    public PolazakController(IPolazakService polazakService)
    {
        PolazakService = polazakService;
    }

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("DodajPolazak")]
    public async Task<ActionResult> DodajPolazak([FromBody] CreatePolazakDto dto)
    {
        try
        {
            var polazak = await PolazakService.Create(dto);
            return Ok(polazak);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("Filtriraj/{gradPolaska}/{gradDolaska}/{datum}")]
    public async Task<ActionResult> Filtriraj(string gradPolaska, string gradDolaska, DateTime datum)
    {
        try
        {
            var polasci = await PolazakService.Filter(gradPolaska, gradDolaska, datum);
            return Ok(polasci);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("ObrisiPolazak/{polazakId}")]
    public async Task<ActionResult> ObrisiPolazak(int polazakId)
    {
        try
        {
            var polazak = await PolazakService.Delete(polazakId);
            return Ok("Uspesno obrisan polazak");
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("{id}")]
    public async Task<ActionResult> VratiPolazakPoId(int id)
    {
        try
        {
            var polazak = await PolazakService.Read(id);
            return Ok(polazak);
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
    [HttpGet("VratiPolaske")]
    public async Task<ActionResult> VratiPolaske()
    {
        try
        {
            var polasci = await PolazakService.Read();
            return Ok(polasci);
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
    [HttpGet("VratiPolaskeDana/{strana}/{velicinaStrane}")]
    public async Task<ActionResult> VratiPolaskeDana(int strana, int velicinaStrane)
    {
        try
        {
            var polasci = await PolazakService.ReadToday(strana, velicinaStrane);
            return Ok(polasci);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("IzmeniPolazak")]
    public async Task<ActionResult> IzmeniPolazak([FromBody] UpdatePolazakDto dto)
    {
        try
        {
            var polazak = await PolazakService.Update(dto);
            return Ok(polazak);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
}