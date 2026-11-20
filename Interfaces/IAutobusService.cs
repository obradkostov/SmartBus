namespace SmartBus.Interfaces;

public interface IAutobusService
{
    Task<ResponseAutobusDto> Create(Autobus autobus);
    Task<ResponseAutobusDto> Read(int autobusId);
    Task<IList<ResponseAutobusDto>> Read();
    Task<ResponseAutobusDto> Update(ResponseAutobusDto dto);
    Task<ResponseAutobusDto> Delete(int autobusId);
}