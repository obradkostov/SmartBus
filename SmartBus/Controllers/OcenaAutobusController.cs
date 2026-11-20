[ApiController]
[Route("[controller]")]
public class OcenaAutobusController : ControllerBase
{
    private readonly IOcenaAutobusService OcenaAutobusService;
    public OcenaAutobusController(IOcenaAutobusService ocenaAutobusService)
    {
        OcenaAutobusService = ocenaAutobusService;
    }

    [Authorize]
    [HttpPost("DodajOcenu")]
    public async Task<ActionResult> DodajOcenu(OcenaAutobusDto dto)
    {
        try
        {
            var novaProsecna = await OcenaAutobusService.Create(dto);
            return Ok(novaProsecna);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
}