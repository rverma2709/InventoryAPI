using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Utils
{
    public static class TripleDES
    {
        public static string Encrypt(string plainText, string key, string IV)
        {
            byte[] keyArray;
            byte[] IVArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(plainText);
            keyArray = UTF8Encoding.UTF8.GetBytes((key.Length > 24 ? key.Substring(0, 24) : key));
            IVArray = UTF8Encoding.UTF8.GetBytes((IV.Length > 8 ? IV.Substring(0, 8) : IV));

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                IV = IVArray,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherText, string key, string IV)
        {
            byte[] keyArray;
            byte[] IVArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherText);
            keyArray = UTF8Encoding.UTF8.GetBytes((key.Length > 128 ? key.Substring(0, 24) : key));
            IVArray = UTF8Encoding.UTF8.GetBytes((IV.Length > 8 ? IV.Substring(0, 8) : IV));

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                IV = IVArray,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
