namespace SmartBus.DTOs.RefreshTokenDto;

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Lozinka { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int KorisnikId { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Uloga { get; set; } = string.Empty;
}

public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}


public class RefreshTokenDto
{
    public int Id { get; set; }
    public int KorisnikId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime DatumIsteka { get; set; }
    public bool Opozvan { get; set; }
}