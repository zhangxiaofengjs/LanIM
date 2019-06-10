using Com.LanIM.Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Security
{
    class DES
    {
        ///私钥和公钥。都一样
        //public SecurityKeys GenerateKeys()
        //{
        //    SecureString key = new SecureString();
        //    key.AppendChar('d');
        //    key.AppendChar('e');
        //    key.AppendChar('s');
        //    key.AppendChar('h');
        //    key.AppendChar('s');
        //    key.AppendChar('0');
        //    key.AppendChar('0');
        //    key.AppendChar('8');
        //}

        //public static string DESEncrypt(string _strQ, string strKey)
        //{
        //    byte[] buffer = Encoding.UTF8.GetBytes(_strQ);
        //    MemoryStream ms = new MemoryStream();
        //    DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
        //    CryptoStream encStream = new CryptoStream(ms, tdes.CreateEncryptor(Encoding.UTF8.GetBytes(strKey), Encoding.UTF8.GetBytes(strKey)), CryptoStreamMode.Write);
        //    encStream.Write(buffer, 0, buffer.Length);
        //    encStream.FlushFinalBlock();
        //    return Convert.ToBase64String(ms.ToArray()).Replace("+", "%");
        //}

        //// DES字符串解密
        //public static string DESDecrypt(string _strQ, string strKey)
        //{
        //    _strQ = _strQ.Replace("%", "+");
        //    byte[] buffer = Convert.FromBase64String(_strQ);
        //    MemoryStream ms = new MemoryStream();
        //    DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
        //    CryptoStream encStream = new CryptoStream(ms, tdes.CreateDecryptor(Encoding.UTF8.GetBytes(strKey), Encoding.UTF8.GetBytes(strKey)), CryptoStreamMode.Write);
        //    encStream.Write(buffer, 0, buffer.Length);
        //    encStream.FlushFinalBlock();
        //    return Encoding.UTF8.GetString(ms.ToArray());
        //}
    }
}
