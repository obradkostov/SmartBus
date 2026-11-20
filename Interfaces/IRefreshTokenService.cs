namespace SmartBus.Interfaces;

public interface IRefreshTokenService
{
    Task<LoginResponseDto> GenerisiTokene(Korisnik korisnik);
    Task<RefreshTokenResponseDto> OsveziToken(string refreshToken);
    Task OpozoviToken(string refreshToken);
}