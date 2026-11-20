namespace SmartBus.DTOs.KorisnikDto;

public class ResponseKorisnikDto
{
    public int Id { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Uloga { get; set; } = string.Empty;
}

public class UpdateKorisnikDto
{
    public int Id { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Lozinka { get; set; } = string.Empty;
    public string Uloga { get; set; } = string.Empty;

}

public class RegisterKorisnikDto
{
    public string Ime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Lozinka { get; set; } = string.Empty;
}

public class LoginKorisnikDto
{
    public string Email { get; set; } = string.Empty;
    public string Lozinka { get; set; } = string.Empty;
}

public class ChangePasswordKorisnikDto
{
    public string Email { get; set; } = string.Empty;
    public string StaraLozinka { get; set; } = string.Empty;
    public string NovaLozinka { get; set; } = string.Empty;
}