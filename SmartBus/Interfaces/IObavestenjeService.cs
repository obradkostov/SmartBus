using SmartBus.DTOs;

namespace SmartBus.Interfaces;

public interface IObavestenjeService
{
    Task<ResponseObavestenjeDto> Create(CreateObavestenjeDto dto);
    Task<IList<ResponseObavestenjeDto>> Read();
}
