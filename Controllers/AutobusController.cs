using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("[controller]")]

public class AutobusController : ControllerBase
{
    private IAutobusService AutobusService;
    public AutobusController(IAutobusService autobusService)
    {
        AutobusService = autobusService;
    }

    #region CRUD

    #region Create

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("DodajAutobus")]
    public async Task<ActionResult> DodajAutobus([FromBody] Autobus autobus)
    {
        try
        {
            var a = await AutobusService.Create(autobus);
            return Ok(a);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    #endregion

    #region Read

    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("VratiAutobus/{autobusId}")]
    public async Task<ActionResult> VratiAutobus(int autobusId)
    {
        try
        {
            var k = await AutobusService.Read(autobusId);
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
    [HttpGet("VratiAutobuse")]
    public async Task<ActionResult> VratiAutobuse()
    {
        try
        {
            var autobusi = await AutobusService.Read();
            return Ok(autobusi);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    #endregion

    #region Update

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPut("IzmeniAutobus")]
    public async Task<ActionResult> IzmeniAutobus([FromBody] ResponseAutobusDto dto)
    {
        try
        {
            var a = await AutobusService.Update(dto);
            return Ok(a);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    #endregion

    #region Delete

    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete("ObrisiAutobus/{autobusId}")]
    public async Task<ActionResult> ObrisiAutobus(int autobusId)
    {
        try
        {
            var a = await AutobusService.Delete(autobusId);
            return Ok(a);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    #endregion

    #endregion

    #region Methods

    #endregion

}