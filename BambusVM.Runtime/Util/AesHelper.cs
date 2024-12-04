using System;
using System.IO;
using System.Security.Cryptography;

namespace BambusVM.Runtime.Util;

public class AesHelper
{
    //AES code by https://www.siakabaro.com/how-to-perform-aes-encryption-in-net/

    /// Encrypts the provided plain text using AES encryption and returns the encrypted data as a base64-encoded string.
    /// Additionally, outputs the encryption key and initialization vector used in the process, also as base64-encoded strings.
    /// <param name="plainText">The plain text string to be encrypted.</param>
    /// <param name="keyBase64">Outputs the encryption key as a base64-encoded string.</param>
    /// <param name="vectorBase64">Outputs the initialization vector as a base64-encoded string.</param>
    /// <returns>The encrypted data as a base64-encoded string.
    public static string EncryptDataWithAes(string plainText, out string keyBase64, out string vectorBase64)
    {
        using (var aesAlgorithm = Aes.Create())
        {
            //set the parameters with out keyword
            keyBase64 = Convert.ToBase64String(aesAlgorithm.Key);
            vectorBase64 = Convert.ToBase64String(aesAlgorithm.IV);

            // Create encryptor object
            var encryptor = aesAlgorithm.CreateEncryptor();

            byte[] encryptedData;

            //Encryption will be done in a memory stream through a CryptoStream object
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    encryptedData = ms.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedData);
        }
    }

    /// <summary>
    /// Decrypts the specified cipher text using AES decryption with the provided key and initialization vector.
    /// </summary>
    /// <param name="cipherText">The encrypted text to be decrypted, represented in base64 format.</param>
    /// <param name="keyBase64">The base64-encoded string representing the AES decryption key.</param>
    /// <param name="vectorBase64">The base64-encoded string representing the AES initialization vector.</param>
    /// <returns>The original plain text obtained from decrypting the cipher text.</returns>
    public static string DecryptDataWithAes(string cipherText, string keyBase64, string vectorBase64)
    {
        using (var aesAlgorithm = Aes.Create())
        {
            aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
            aesAlgorithm.IV = Convert.FromBase64String(vectorBase64);

            // Create decryptor object
            var decryptor = aesAlgorithm.CreateDecryptor();

            var cipher = Convert.FromBase64String(cipherText);

            //Decryption will be done in a memory stream through a CryptoStream object
            using (var ms = new MemoryStream(cipher))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}