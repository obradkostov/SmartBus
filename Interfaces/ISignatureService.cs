namespace SmartBus.Interfaces;

public interface ISignatureService
{
    string GenerisiSignature(int kartaId, DateTime vremePolaska, int putnikId);
    bool ProveriSignature(string signature, int kartaId, DateTime vremePolaska, int putnikId);
}