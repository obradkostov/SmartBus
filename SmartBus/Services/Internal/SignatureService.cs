using System.Security.Cryptography;
using System.Text;

namespace SmartBus.Services.Internal;

public class SignatureService : ISignatureService
{
    private readonly string SecretKey;
    public SignatureService(IConfiguration configuration)
    {
        SecretKey = configuration["TicketSignature:Secret"]!;
    }
    public string GenerisiSignature(int kartaId, DateTime vremePolaska, int putnikId)
    {
        var podaci = $"{kartaId}{vremePolaska:yyyy-MM-dd:mm:ss}{putnikId}";

        using var hmac= new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(podaci));
        return Convert.ToHexString(hash).ToLower();
    }

    public bool ProveriSignature(string signature, int kartaId, DateTime vremePolaska, int putnikId)
    {
        if(string.IsNullOrWhiteSpace(signature)) return false;
        try
        {
            var ocekivaniSignature = GenerisiSignature(kartaId, vremePolaska, putnikId);
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(ocekivaniSignature),
                Convert.FromHexString(signature)
            );
        }
        catch (FormatException)
        {
            return false;
        }
    }
}