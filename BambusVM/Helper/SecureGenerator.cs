using System;
using System.Security.Cryptography;

namespace BambusVM.Helper
{
    public class SecureGenerator
    {
        public static string RandomAlphabet()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            char[] password = new char[30];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                for (int i = 0; i < password.Length; i++)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    password[i] = validChars[(int)(num % (uint)validChars.Length)];
                }
            }
            return new string(password);
        }
    }
}