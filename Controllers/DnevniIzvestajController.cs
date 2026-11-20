using SmartBus.Dtos;

[ApiController]
[Route("[controller]")]
public class DnevniIzvestajController : ControllerBase
{
    private IDnevniIzvestajService DnevniIzvestajService;

    public DnevniIzvestajController(IDnevniIzvestajService dnevniIzvestajService)
    {
        DnevniIzvestajService = dnevniIzvestajService;
    }

    [Authorize(Roles = "kontrolor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("DodajDnevniIzvestaj")]
    public async Task<ActionResult> DodajDnevniIzvestaj([FromBody] CreateDnevniIzvestajDto dto)
    {
        try
        {
            var izvestaj = await DnevniIzvestajService.Create(dto);
            return Ok(izvestaj);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [AllowAnonymous]
    [HttpPost("VratiIzvestajePaginacija")]
    public async Task<ActionResult> VratiIzvestajePaginacija([FromBody] FiltrirajIzvestajeDto dto)
    {
        try
        {
            var izvestaji = await DnevniIzvestajService.Read(dto);
            return Ok(izvestaji);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("VratiStatistiku")]
    public async Task<ActionResult> VratiStatistiku()
    {
        try
        {
            var statistika = await DnevniIzvestajService.Statistics();
            return Ok(statistika);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
}