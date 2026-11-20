using SmartBus.Dtos;

namespace SmartBus.Interfaces;

public interface IDnevniIzvestajService
{
    Task<ResponseDnevniIzvestajDto> Create(CreateDnevniIzvestajDto dto);
    Task<ListaIzvestajaDto> Read(FiltrirajIzvestajeDto dto);
    Task<StatistikaDanaDto> Statistics();
}