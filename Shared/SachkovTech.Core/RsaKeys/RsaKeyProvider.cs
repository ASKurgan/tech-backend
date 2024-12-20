using System.Security.Cryptography;

namespace SachkovTech.Core.RsaKeys;

public class RsaKeyProvider : IRsaKeyProvider
{
    private readonly bool _createNewKeys;
    private const string PrivateKeyPath = "etc/key.private";
    private const string PublicKeyPath = "etc/key.pub";
    private readonly RSA _rsa;

    public RsaKeyProvider(bool createNewKeys)
    {
        _createNewKeys = createNewKeys;
        _rsa = RSA.Create();
        EnsureKeysExist();
    }

    /// <summary>
    /// Возвращает RSA, инициализированный приватным ключом.
    /// </summary>
    /// <returns>RSA private key.</returns>
    public RSA GetPrivateRsa()
    {
        byte[] privateKeyBytes = File.ReadAllBytes(PrivateKeyPath);
        _rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
        return _rsa;
    }

    /// <summary>
    /// Возвращает RSA, инициализированный публичным ключом.
    /// </summary>
    /// <returns>RSA public key.</returns>
    public RSA GetPublicRsa()
    {
        byte[] publicKeyBytes = File.ReadAllBytes(PublicKeyPath);
        _rsa.ImportRSAPublicKey(publicKeyBytes, out _);
        return _rsa;
    }

    /// <summary>
    /// Проверяем, существуют ли файлы с ключами. Если нет, генерируем их.
    /// </summary>
    private void EnsureKeysExist()
    {
        if (_createNewKeys && (!File.Exists(PrivateKeyPath) || !File.Exists(PublicKeyPath)))
        {
            GenerateKeys();
        }
    }

    /// <summary>
    /// Генерируем пару ключей и записываем в файлы в двоичном формате.
    /// </summary>
    private void GenerateKeys()
    {
        byte[] privateKeyBytes = _rsa.ExportRSAPrivateKey();
        byte[] publicKeyBytes = _rsa.ExportRSAPublicKey();

        File.WriteAllBytes(PrivateKeyPath, privateKeyBytes);
        File.WriteAllBytes(PublicKeyPath, publicKeyBytes);
    }
}