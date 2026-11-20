using SmartBus.DTOs.SedisteDto;

namespace  SmartBus.Interfaces;

public interface ISedisteService
{
    Task<ResponseSedisteDto> Create(CreateSedisteDto dto);
    Task<ResponseSedisteDto> Read(int sedisteId);
    Task<IList<ResponseSedisteDto>> ReadForBus(int autobusId);
    Task<ResponseSedisteDto> Update(UpdateSedisteDto dto);
    Task<ResponseSedisteDto> Delete(int sedisteId);
}