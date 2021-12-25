using Anye.Soft.Common.Security.Encrypt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Anye.Soft.Common.Security
{
    public class EncryptHelper
    {
        public enum EncryptType
        {
            None = 0,
            RC2 = 2,
        }

        static string EnRC2(string content, string key)
        {
            RC2_ rc2 = new RC2_(key);
            return rc2.Encrypt(content);
        }
        static string EnRC2(string content, string key, string iv)
        {
            RC2_ rc2 = new RC2_(key, iv);
            return rc2.Encrypt(content);
        }
        static string DeRC2(string content, string key)
        {
            RC2_ rc2 = new RC2_(key);
            return rc2.Decrypt(content);
        }
        static string DeRC2(string content, string key, string iv)
        {
            RC2_ rc2 = new RC2_(key, iv);
            return rc2.Decrypt(content);
        }

        public static string APIEncode(string content, EncryptType etype, string key = "")
        {
            string result = string.Empty;
            switch (etype)
            {
                case EncryptType.None:
                    result = content;
                    break;
                case EncryptType.RC2:
                    result = EnRC2(content, key, key);
                    break;
                default:
                    break;
            }
            return result;
        }

        public static string APIDecode(string content, EncryptType etype, string key = "")
        {
            try
            {

                string result = string.Empty;
                switch (etype)
                {
                    case EncryptType.None:
                        result = content;
                        break;
                    case EncryptType.RC2:
                        result = DeRC2(content, key, key);
                        break;
                    default:
                        break;
                }
                return result;

            }
            catch { return null; }
        }

        public static string APIEncode(string text, string type, string key)
        {
            switch (type.ToUpper())
            {
                case "RC2":
                    return APIEncode(text, EncryptType.RC2, key);
                default:
                    return text;
            }
        }

        public static string APIDecode(string text, string type, string key, string deMD5 = "")
        {
            switch (type.ToUpper())
            {
                case "RC2":
                    return APIDecode(text, EncryptType.RC2, key);
                default:
                    return text;
            }
        }

    }
}
