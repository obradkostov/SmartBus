namespace SmartBus.Interfaces;

public interface IKartaService
{
    Task<ResponseKartaDto> Create(CreateKartaDto dto);
    Task<IList<ResponseKartaDto>> Read(int korisnikId);
    Task<ResponseKartaDto> Update(UpdateKartaDto dto);
    Task<ResponseKartaDto> Delete(int kartaId);
    //treba napraviti da se karta validira po broju karte a ne po id
    //(treba dodati u bazi u tabeli karta kolonu za broj karte)
    //Task<ResponseKartaDto> Validate(int kartaId);
    Task<List<int>> ZauzetaSedista(int polazakId);
    Task<string?> GetSignature(int kartaId, int korisnikId);
    Task<ValidacijaResponseDto> ValidirajKartu(ValidacijaKarteDto dto, int kontrolorId);
    Task<bool> OtkaziKartu(int kartaId);
}