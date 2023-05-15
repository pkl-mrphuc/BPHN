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
            string ToReturn = "";
            string publickey = "santhosh";
            string secretkey = "engineer";
            var secretkeyByte = Encoding.UTF8.GetBytes(secretkey);
            var publickeybyte = Encoding.UTF8.GetBytes(publickey);
            byte[] inputbyteArray = Encoding.UTF8.GetBytes(input);
            using (var des = new DESCryptoServiceProvider())
            {
                using (var ms = new MemoryStream())
                {
                    var cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
            }
            return ToReturn;
        }

        public string Encryption(string input)
        {
            string ToReturn = "";
            string publickey = "santhosh";
            string privatekey = "engineer";
            var privatekeyByte = Encoding.UTF8.GetBytes(privatekey);
            var publickeybyte = Encoding.UTF8.GetBytes(publickey);
            var inputbyteArray = Convert.FromBase64String(input);
            using (var des = new DESCryptoServiceProvider())
            {
                using (var ms = new MemoryStream())
                {
                    var cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }     
            }
            return ToReturn;
        }
    }
}
