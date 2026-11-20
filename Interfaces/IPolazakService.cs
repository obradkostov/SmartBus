namespace SmartBus.Interfaces;

public interface IPolazakService
{
    Task<ResponsePolazakDto> Create(CreatePolazakDto dto);
    Task<ResponsePolazakDto> Read(int polazakId);
    Task<ResponsePolazakPagingDto> ReadToday(int stranica, int velicinaStranice);
    Task<IList<ResponsePolazakDto>> Read();
    Task<ResponsePolazakDto> Update(UpdatePolazakDto dto);
    Task<ResponsePolazakDto> Delete(int polazakId);
    Task<IList<ResponsePolazakDto>> Filter(string gradPolaska, string gradDolaska, DateTime datum);
}