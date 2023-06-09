namespace BPHN.BusinessLayer.IServices
{
    public interface IKeyGenerator
    {
        string Encryption(string input);
        string Decryption(string input);
    }
}
