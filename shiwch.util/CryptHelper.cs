// -------------------------------------------------------
// Copyright (C) 施维串 版权所有。
// 创建标识：2013-11-11 10:53:36 Created by 施维串
// 功能说明：
// 注意事项：
// 
// 更新记录：
// -------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace shiwch.util
{
    /// <summary>
    /// MD5以及加解密帮助类
    /// </summary>
    public static class CryptHelper
    {
        #region 密钥
        /// <summary>
        /// 密钥
        /// </summary>
        static string Key = "!@#ASD12";
        #endregion

        #region MD5
        /// <summary>
        /// 标准MD5
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="encoding">字符串编码，默认使用Encoding.Default</param>
        /// <exception cref="ArgumentNullException">source为空</exception>
        /// <returns>该字符串的MD5</returns>
        public static string MD5(string source, Encoding encoding = null)
        {
            Check.NotNull(source, "source");
            if (encoding == null) encoding = Encoding.Default;

            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] datSource = encoding.GetBytes(source);
                byte[] newSource = md5.ComputeHash(datSource);
                return newSource.ToHex();
            }
        }

        /// <summary>
        /// 标准MD5
        /// </summary>
        /// <param name="source">源字节数组</param>
        /// <exception cref="ArgumentNullException">source为空</exception>
        /// <returns>该字节数组的MD5</returns>
        public static byte[] MD5(byte[] source)
        {
            Check.NotNull(source, "source");

            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                return md5.ComputeHash(source);
            }
        }

        /// <summary>
        /// 计算文件的MD5值并返回
        /// </summary>
        /// <param name="fileName">用于计算MD5值的输入文件</param>
        /// <returns>该文件的MD5</returns>
        public static string MD5File(string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Open))
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] retVal = md5.ComputeHash(file);
                    return retVal.ToHex();
                }
            }
        }

        #endregion

        #region  SHA1
        /// <summary>
        /// 标准SHA1
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="encoding">字符串编码，默认使用Encoding.Default</param>
        /// <exception cref="ArgumentNullException">source为空</exception>
        /// <returns>该字符串的SHA1</returns>
        public static string SHA1(string source, Encoding encoding = null)
        {
            Check.NotNull(source, "source");
            if (encoding == null) encoding = Encoding.Default;

            using (SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] datSource = encoding.GetBytes(source);
                byte[] newSource = sha1.ComputeHash(datSource);
                return newSource.ToHex();
            }
        }

        /// <summary>
        /// 标准SHA1
        /// </summary>
        /// <param name="source">源字节数组</param>
        /// <exception cref="ArgumentNullException">source为空</exception>
        /// <returns>该字节数组的SHA1</returns>
        public static byte[] SHA1(byte[] source)
        {
            Check.NotNull(source, "source");

            using (SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
            {
                return sha1.ComputeHash(source);
            }
        }

        /// <summary>
        /// 计算文件的SHA1并返回
        /// </summary>
        /// <param name="fileName">用于计算MD5值的输入文件</param>
        /// <returns>该文件的SHA1</returns>
        public static string SHA1File(string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Open))
            {
                using (SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
                {
                    byte[] retVal = sha1.ComputeHash(file);
                    return retVal.ToHex();
                }
            }
        }

        #endregion

        #region  DES 加解密

        /// <summary>
        /// 使用默认的密钥和默认的编码Encoding.Default对字符串进行DES加密
        /// </summary>
        /// <param name="source">待加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string DES_Encrypt(string source)
        {
            return DES_Encrypt(source, Key);
        }

        /// <summary>
        /// 使用指定的密钥和默认的编码Encoding.Default对字符串进行DES加密
        /// </summary>
        /// <param name="source">待加密字符</param>
        /// <param name="key">密钥</param>
        /// <exception cref="ArgumentNullException">source或者key为空</exception>
        /// <returns>加密后的字符串</returns>
        public static string DES_Encrypt(string source, string key)
        {
            return DES_Encrypt(source, key, Encoding.Default);
        }

        /// <summary>
        /// 使用默认的密钥和指定的编码对字符串进行DES加密
        /// </summary>
        /// <param name="source">待加密字符</param>
        /// <param name="encoding">编码</param>
        /// <exception cref="ArgumentNullException">source或者encoding为空</exception>
        /// <returns>加密后的字符串</returns>
        public static string DES_Encrypt(string source, Encoding encoding)
        {
            return DES_Encrypt(source, Key, encoding);
        }

        /// <summary>
        /// 使用指定的密钥和编码对字符串进行DES加密
        /// </summary>
        /// <param name="source">待加密字符</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        /// <exception cref="ArgumentNullException">source或者key或者encoding为空</exception>
        /// <returns></returns>
        public static string DES_Encrypt(string source, string key, Encoding encoding)
        {
            Check.NotNull(source, "source");
            Check.NotNull(key, "key");
            Check.NotNull(encoding, "encoding");

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = encoding.GetBytes(source);

                des.Key = Encoding.ASCII.GetBytes(key);
                des.IV = Encoding.ASCII.GetBytes(key);

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray().ToHex();
                }
            }
        }
        /// <summary>
        /// 使用指定的key和iv对字节数组进行DES加密
        /// </summary>
        /// <param name="source">待加密的字节数组</param>
        /// <param name="key">key</param>
        /// <param name="iv">iv</param>
        /// <exception cref="ArgumentNullException">source或者key或者iv为空</exception>
        /// <returns></returns>
        public static byte[] DES_Encrypt(byte[] source, byte[] key, byte[] iv)
        {
            Check.NotNull(source, "source");
            Check.NotNull(key, "key");
            Check.NotNull(iv, "iv");

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = key;
                des.IV = iv;
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(source, 0, source.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// 使用默认的密钥和默认的编码Encoding.Default对字符串进行DES解密
        /// </summary>
        /// <param name="source">待解密的字符串</param>
        /// <exception cref="ArgumentNullException">source为空</exception>
        /// <returns>解密后的字符串</returns>
        public static string DES_Decrypt(string source)
        {
            return DES_Decrypt(source, Key);
        }
        /// <summary>
        /// 使用指定的密钥和默认的编码Encoding.Default对字符串进行DES解密
        /// </summary>
        /// <param name="source">待解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <exception cref="ArgumentNullException">source或者key为空</exception>
        /// <returns>解密后的字符串</returns>
        public static string DES_Decrypt(string source, string key)
        {
            return DES_Decrypt(source, key, Encoding.Default);
        }
        /// <summary>
        /// 使用默认的密钥和指定的编码对字符串进行DES解密
        /// </summary>
        /// <param name="source">待解密的字符串</param>
        /// <param name="encoding">编码</param>
        /// <exception cref="ArgumentNullException">source或者encoding为空</exception>
        /// <returns>解密后的字符串</returns>
        public static string DES_Decrypt(string source, Encoding encoding)
        {
            return DES_Decrypt(source, Key, encoding);
        }
        /// <summary>
        /// 使用指定的密钥和编码对字符串进行DES解密
        /// </summary>
        /// <param name="source">待解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        /// <exception cref="ArgumentNullException">source或者key或者encoding为空</exception>
        /// <returns>解密后的字符串</returns>
        public static string DES_Decrypt(string source, string key, Encoding encoding)
        {
            Check.NotNull(source, "source");
            Check.NotNull(key, "key");
            Check.NotNull(encoding, "encoding");

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = source.ParseHex();

                des.Key = Encoding.ASCII.GetBytes(key);
                des.IV = Encoding.ASCII.GetBytes(key);

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    return encoding.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                }
            }
        }

        /// <summary>
        /// 使用指定的key和iv对字节数组进行DES解密
        /// </summary>
        /// <param name="source">待解密的字节数组</param>
        /// <param name="key">key</param>
        /// <param name="iv">iv</param>
        /// <exception cref="ArgumentNullException">source或者key或者iv为空</exception>
        /// <returns>解密后的字节数组</returns>
        public static byte[] DES_Decrypt(byte[] source, byte[] key, byte[] iv)
        {
            Check.NotNull(source, "source");
            Check.NotNull(key, "key");
            Check.NotNull(iv, "iv");

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = key;
                des.IV = iv;
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(source, 0, source.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }
        #endregion
    }
}
