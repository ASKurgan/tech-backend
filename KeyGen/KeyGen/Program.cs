using System.Security.Cryptography;

var rsaKey = RSA.Create();
byte[] privateKey = rsaKey.ExportRSAPrivateKey();
File.WriteAllBytes("key", privateKey);