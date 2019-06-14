using Com.LanIM.Common.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Common.Security
{
    //待加密的字节数不能超过密钥的长度值除以 8 再减去 11（即：RSACryptoServiceProvider.KeySize / 8 - 11），
    //而加密后得到密文的字节数，正好是密钥的长度值除以 8（即：RSACryptoServiceProvider.KeySize / 8）。
    class RSA
    {
        private const int RsaKeySize = 2048;
        private const int RsaEnMaxBlockSize = RsaKeySize / 8 - 11;
        private const int RsaDeMaxBlockSize = RsaKeySize / 8;

        ///私钥和公钥。
        public SecurityKeys GenerateKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    // 获取私钥和公钥
                    SecurityKeys keys = new SecurityKeys
                    {
                        Public = rsa.ExportCspBlob(false),
                        Private = rsa.ExportCspBlob(true)
                    };
                    return keys;
                }
                catch(Exception e)
                {
                    LoggerFactory.Error("[rsa genkey error]", e);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
                return null;
            }
        }

        /// <summary>
        /// 用给定的RSA公钥文件加密纯文本
        /// </summary>
        /// <param name="plainText">要加密的文本</param>
        /// <param name="publicKey">用于加密的公钥路径</param>
        /// <returns>表示加密数据的64位编码字符串.</returns>
        public byte[] Encrypt(byte[] bytesToEncrypt, byte[] publicKey)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    rsa.ImportCspBlob(publicKey);

                    if (bytesToEncrypt.Length <= RsaEnMaxBlockSize)
                    {
                        return rsa.Encrypt(bytesToEncrypt, false);
                    }

                    //待加密的字节数不能超过密钥的长度值除以 8 再减去 11
                    using (MemoryStream plainMs = new MemoryStream(bytesToEncrypt), 
                            encryptMs = new MemoryStream())
                    {
                        Byte[] buf = new Byte[RsaEnMaxBlockSize];
                        int blockSize;

                        while ((blockSize = plainMs.Read(buf, 0, RsaEnMaxBlockSize)) > 0)
                        {
                            if (blockSize < RsaEnMaxBlockSize)
                            {
                                Byte[] buf2 = new Byte[blockSize];
                                Array.Copy(buf, 0, buf2, 0, buf2.Length);

                                byte[] encryptBuf = rsa.Encrypt(buf2, false);
                                encryptMs.Write(encryptBuf, 0, encryptBuf.Length);
                            }
                            else
                            {
                                byte[] encryptBuf = rsa.Encrypt(buf, false);
                                encryptMs.Write(encryptBuf, 0, encryptBuf.Length);
                            }
                        }
                        return encryptMs.ToArray();
                    }
                }
                catch (Exception e)
                {
                    LoggerFactory.Error("[rsa encrypt error]", e);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
                return null;
            }
        }

        /// <summary>
        /// 给定的RSA私钥文件解密文本
        /// </summary>
        /// <param name="encryptedText">加密的密文</param>
        /// <param name="privateKey">用于加密的私钥路径.</param>
        /// <returns>未加密数据的字符串</returns>
        public byte[] Decrypt(byte[] encryptedData, byte[] privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider(RsaKeySize))
            {
                try
                {
                    rsa.ImportCspBlob(privateKey);

                    if (encryptedData.Length <= RsaDeMaxBlockSize)
                    {
                        return rsa.Decrypt(encryptedData, false);
                    }

                    using (MemoryStream crypMs = new MemoryStream(encryptedData),
                        plainMs = new MemoryStream())
                    {
                        Byte[] buf = new Byte[RsaDeMaxBlockSize];
                        int blockSize;

                        while ((blockSize = crypMs.Read(buf, 0, RsaDeMaxBlockSize)) > 0)
                        {
                            if (blockSize < RsaDeMaxBlockSize)
                            {
                                Byte[] buf2 = new Byte[blockSize];
                                Array.Copy(buf, 0, buf2, 0, buf2.Length);

                                byte[] plainBuf = rsa.Decrypt(buf2, false);
                                plainMs.Write(plainBuf, 0, plainBuf.Length);
                            }
                            else
                            {
                                byte[] plainBuf = rsa.Decrypt(buf, false);
                                plainMs.Write(plainBuf, 0, plainBuf.Length);
                            }
                        }

                        return plainMs.ToArray();
                    }
                }
                catch (Exception e)
                {
                    LoggerFactory.Error("[rsa decrypt error]", e);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
                return null;
            }
        }
    }
}
