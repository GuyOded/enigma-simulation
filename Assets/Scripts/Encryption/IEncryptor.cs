
namespace Encryption
{
    public interface IEncryptor
    {
        string Encrypt(string payload);
        string Decrypt(string payload);
    }
}
