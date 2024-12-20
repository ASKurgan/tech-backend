using System.Security.Cryptography;

namespace SachkovTech.Core.RsaKeys;

public interface IRsaKeyProvider
{
    /// <summary>
    /// Возвращает RSA, инициализированный приватным ключом.
    /// </summary>
    /// <returns>RSA private key.</returns>
    RSA GetPrivateRsa();

    /// <summary>
    /// Возвращает RSA, инициализированный публичным ключом.
    /// </summary>
    /// <returns>RSA public key.</returns>
    RSA GetPublicRsa();
}