using SmartBus.DTOs;

[ApiController]
[Route("[controller]")]
public class ObavestenjeController : ControllerBase
{
    private readonly IObavestenjeService ObavestenjeService;

    public ObavestenjeController(IObavestenjeService obavestenjeService)
    {
        ObavestenjeService = obavestenjeService;
    }

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("DodajObavestenje")]
    public async Task<ActionResult> DodajObavestenje([FromBody] CreateObavestenjeDto dto)
    {
        try
        {
            var obavestenje = await ObavestenjeService.Create(dto);
            return Ok(obavestenje);
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
    [HttpGet("VratiObavestenja")]
    public async Task<IActionResult> VratiObavestenja()
    {
        try
        {
            var obavestenja = await ObavestenjeService.Read();
            return Ok(obavestenja);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}