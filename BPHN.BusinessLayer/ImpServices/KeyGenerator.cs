using BPHN.BusinessLayer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices
{
    public class KeyGenerator : IKeyGenerator
    {
        public string Decryption(string input)
        {
            byte[] IV = { 12, 21, 43, 17, 57, 35, 67, 27 };
            string encryptKey = "aXb2uy4z"; // MUST be 8 characters
            var key = Encoding.UTF8.GetBytes(encryptKey);
            byte[] byteInput = new byte[input.Length];
            byteInput = Convert.FromBase64String(input);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            using (var memStream = new MemoryStream())
            {
                ICryptoTransform transform = provider.CreateDecryptor(key, IV);
                CryptoStream cryptoStream = new CryptoStream(memStream, transform, CryptoStreamMode.Write);
                cryptoStream.Write(byteInput, 0, byteInput.Length);
                cryptoStream.FlushFinalBlock();
                Encoding encoding1 = Encoding.UTF8;
                return encoding1.GetString(memStream.ToArray());
            } 
        }

        public string Encryption(string input)
        {
            byte[] IV = { 12, 21, 43, 17, 57, 35, 67, 27 };
            string encryptKey = "aXb2uy4z";
            var key = Encoding.UTF8.GetBytes(encryptKey);
            byte[] byteInput = Encoding.UTF8.GetBytes(input);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            using (var memStream = new MemoryStream())
            {
                ICryptoTransform transform = provider.CreateEncryptor(key, IV);
                CryptoStream cryptoStream = new CryptoStream(memStream, transform, CryptoStreamMode.Write);
                cryptoStream.Write(byteInput, 0, byteInput.Length);
                cryptoStream.FlushFinalBlock();
                return Convert.ToBase64String(memStream.ToArray());
            }
        }
    }
}
