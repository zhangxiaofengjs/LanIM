using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Security
{
    public class SecurityFactory
    {
        private static RSA _rsa = new RSA();
        private static MD5 _md5 = new MD5();
        
        public static SecurityKeys GenerateKeys()
        {
            return _rsa.GenerateKeys();
        }

        public static byte[] Encrypt(byte[] bytesToEncrypt, byte[] publicKey)
        {
            return _rsa.Encrypt(bytesToEncrypt, publicKey);
        }

        public static byte[] Decrypt(byte[] encryptedData, byte[] privateKey)
        {
            return _rsa.Decrypt(encryptedData, privateKey);
        }

        public string Encrypt(string str)
        {
            return _md5.Encrypt(str);
        }

        public bool Match(string str, string encryptStr)
        {
            return _md5.Match(str, encryptStr);
        }
    }
}
