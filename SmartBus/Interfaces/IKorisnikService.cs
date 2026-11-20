namespace SmartBus.Interfaces;

public interface IKorisnikService
{
    Task<ResponseKorisnikDto> Create(Korisnik korisnik);
    Task<ResponseKorisnikDto> Read(int korisnikId);
    Task<UpdateKorisnikDto> Read(string korisnikEmail);
    Task<IList<ResponseKorisnikDto>> Read();
    //za update treba proslediti neki dto a ne ceo objekat korisnik
    Task<ResponseKorisnikDto> Update(UpdateKorisnikDto dto);
    Task<ResponseKorisnikDto> Update(int id, string ime);
    Task<ResponseKorisnikDto> Delete(int korisnikId);
    Task<ResponseKorisnikDto> Register(RegisterKorisnikDto dto);
    Task<LoginResponseDto> Login(LoginKorisnikDto dto);
    Task<ResponseKorisnikDto> ChangePassword(ChangePasswordKorisnikDto dto);
}