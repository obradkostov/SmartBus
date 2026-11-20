namespace SmartBus.Interfaces;

public interface IStanicaService
{
    Task<ResponseStanicaDto> Create(CreateStanicaDto dto);
    Task<ResponseStanicaDto> Read(int stanicaId);
    Task<IList<ResponseStanicaDto>> Read();
    Task<ResponseStanicaDto> Update(ResponseStanicaDto dto);
    Task<ResponseStanicaDto> Delete(int stanicaId);
}