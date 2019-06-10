using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Security
{
    class MD5
    {
        public string Encrypt(string str)
        {
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] palindata = Encoding.UTF8.GetBytes(str);
            byte[] encryptdata = md5Provider.ComputeHash(palindata);
            return Convert.ToBase64String(encryptdata);
        }

        public bool Match(string str, string encryptStr)
        {
            return (Encrypt(str) == encryptStr);
        }
    }
}
