using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace SmartBus.Services.Internal;

public class RefreshTokenService : IRefreshTokenService
{
    private AppDbContext Context;
    private readonly IConfiguration Configuration;
    public RefreshTokenService(AppDbContext context, IConfiguration configuration)
    {
        Context = context;
        Configuration = configuration;
    }
    public async Task<LoginResponseDto> GenerisiTokene(Korisnik korisnik)
    {
        var accessToken = GenerisiAccessToken(korisnik);
        var refreshToken = GenerisiRefreshToken();

        var refreshTokenEntitet = new RefreshToken
        {
            KorisnikId = korisnik.Id,
            Token = refreshToken,
            DatumIsteka = DateTime.UtcNow.AddDays(
                int.Parse(Configuration["Jwt:RefreshTokenExpiryDays"]!)
            ),
            Opozvan = false
        };

        await Context.RefreshTokeni.AddAsync(refreshTokenEntitet);
        await Context.SaveChangesAsync();

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            KorisnikId = korisnik.Id,
            Ime = korisnik.Ime,
            Email = korisnik.Email,
            Uloga = korisnik.Uloga
        };
    }

    public async Task OpozoviToken(string refreshToken)
    {
        var tokenEntitet = await Context.RefreshTokeni
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if(tokenEntitet == null) throw new Exception("Refresh token ne postoji.");

        tokenEntitet.Opozvan = true;
        await Context.SaveChangesAsync();
    }

    public async Task<RefreshTokenResponseDto> OsveziToken(string refreshToken)
    {
        var tokenEntitet = await Context.RefreshTokeni
            .Include(rt => rt.Korisnik)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if(tokenEntitet == null) throw new Exception("Refresh token ne postoji.");
        if(tokenEntitet.Opozvan) throw new Exception("Refresh token je opozvan.");
        if(tokenEntitet.DatumIsteka < DateTime.UtcNow) throw new Exception("Refresh token je istekao.");

        tokenEntitet.Opozvan = true;

        var noviRefreshToken = GenerisiRefreshToken();
        var noviTokenEntitet = new RefreshToken
        {
          KorisnikId = tokenEntitet.KorisnikId,
          Token = noviRefreshToken,
          DatumIsteka = DateTime.UtcNow.AddDays(
            int.Parse(Configuration["Jwt:RefreshTokenExpiryDays"]!)
          ),
          Opozvan = false
        };

        await Context.RefreshTokeni.AddAsync(noviTokenEntitet);
        await Context.SaveChangesAsync();

        return new RefreshTokenResponseDto
        {
            AccessToken = GenerisiAccessToken(tokenEntitet.Korisnik),
            RefreshToken = noviRefreshToken
        };
    }

    #region Helpers

    private string GenerisiAccessToken(Korisnik korisnik)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]!)
        );

        var kredencijali = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, korisnik.Id.ToString()),
            new Claim(ClaimTypes.Email, korisnik.Email),
            new Claim(ClaimTypes.Role, korisnik.Uloga)
        };

        var token = new JwtSecurityToken(
            issuer: Configuration["Jwt:Issuer"], 
            audience: Configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(Configuration["Jwt:AccessTokenExpiryMinutes"]!)
            ),
            signingCredentials: kredencijali
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerisiRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    #endregion
}