using System.Security.Cryptography;
using SachkovTech.Core.RsaKeys;

namespace SachkovTech.Framework.Authorization;

internal class RsaKeyProvider : IRsaKeyProvider
{
    private readonly AuthOptions _authOptions;
    private readonly RSA _rsa;

    public RsaKeyProvider(AuthOptions authOptions)
    {
        _authOptions = authOptions;

        _rsa = RSA.Create();
        EnsureKeysExist();
    }

    /// <summary>
    /// Возвращает RSA, инициализированный приватным ключом.
    /// </summary>
    /// <returns>RSA private key.</returns>
    public RSA GetPrivateRsa()
    {
        byte[] privateKeyBytes = File.ReadAllBytes(_authOptions.PrivateKeyPath);
        _rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
        return _rsa;
    }

    /// <summary>
    /// Возвращает RSA, инициализированный публичным ключом.
    /// </summary>
    /// <returns>RSA public key.</returns>
    public RSA GetPublicRsa()
    {
        byte[] publicKeyBytes = File.ReadAllBytes(_authOptions.PublicKeyPath);
        _rsa.ImportRSAPublicKey(publicKeyBytes, out _);
        return _rsa;
    }

    /// <summary>
    /// Проверяем, существуют ли файлы с ключами. Если нет, генерируем их.
    /// </summary>
    private void EnsureKeysExist()
    {
        if (_authOptions.CreateNewKeys && (!File.Exists(_authOptions.PrivateKeyPath) || !File.Exists(_authOptions.PublicKeyPath)))
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

        File.WriteAllBytes(_authOptions.PrivateKeyPath, privateKeyBytes);
        File.WriteAllBytes(_authOptions.PublicKeyPath, publicKeyBytes);
    }
}