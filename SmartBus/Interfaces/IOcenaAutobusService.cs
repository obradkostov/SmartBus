namespace SmartBus.Interfaces;

public interface IOcenaAutobusService
{
    Task<double> Create(OcenaAutobusDto dto);
}