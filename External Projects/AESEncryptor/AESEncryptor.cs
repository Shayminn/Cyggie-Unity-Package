using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Cyggie.Encryption
{
    /// <summary>
    /// Encryption/Decryption using AES <br/>
    /// Based on from http://www.unity3dtechguru.com/2021/03/data-encryption-decryption-unity-c.html
    /// </summary>
    public static class AESEncryptor
    {
        //
        // You can use this to generate a new set of key and iv
        // http://www.unit-conversion.info/texttools/random-string-generator/
        //

        private static string _key = "Wlo0wxabLpXE8ADwmgAA1UF2HhNVsnMu"; // set any string of 32 chars
        private static string _iv = "bw5c61NbYIX2SPIO"; // set any string of 16 chars
        
        /// <summary>
        /// Encrypt an object using <see cref="Newtonsoft.Json.JsonConvert.SerializeObject(object?)"/>
        /// </summary>
        /// <param name="obj">Any object to encrypt</param>
        /// <returns>Encrypted data</returns>
        public static string Encrypt(object obj)
        {
            return Encrypt(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// Encrypt a string of data using AES
        /// </summary>
        /// <param name="data">String data to encrypt</param>
        /// <returns>Encrypted data</returns>
        public static string Encrypt(string data)
        {
            AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();
            AEScryptoProvider.BlockSize = 128;
            AEScryptoProvider.KeySize = 256;
            AEScryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(_key);
            AEScryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(_iv);
            AEScryptoProvider.Mode = CipherMode.CBC;
            AEScryptoProvider.Padding = PaddingMode.PKCS7;

            byte[] txtByteData = ASCIIEncoding.ASCII.GetBytes(data);
            ICryptoTransform trnsfrm = AEScryptoProvider.CreateEncryptor(AEScryptoProvider.Key, AEScryptoProvider.IV);

            byte[] result = trnsfrm.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Decrypt a string of data using AES <br/>
        /// The string of data must have been previously encrypted using the encryption method, <see cref="AESEncryptor.Encrypt(string)"/>
        /// </summary>
        /// <param name="data">String data that was previously encrypted using <see cref="AESEncryptor.Decrypt(string)"/></param>
        /// <returns>Decrypted data</returns>
        public static string Decrypt(string data)
        {
            AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();
            AEScryptoProvider.BlockSize = 128;
            AEScryptoProvider.KeySize = 256;
            AEScryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(_key);
            AEScryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(_iv);
            AEScryptoProvider.Mode = CipherMode.CBC;
            AEScryptoProvider.Padding = PaddingMode.PKCS7;

            byte[] txtByteData = Convert.FromBase64String(data);
            ICryptoTransform trnsfrm = AEScryptoProvider.CreateDecryptor();

            byte[] result = trnsfrm.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
            return ASCIIEncoding.ASCII.GetString(result);
        }
    }
}
