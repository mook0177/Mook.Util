using System;
using System.Text;

namespace Mook.Util
{
    /// <summary>
    /// 加密帮助类
    /// </summary>
    public class EncryptHelper
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="text">待加密文本字符</param>
        /// <returns></returns>
        public static string MD5(string text)
        {
            using (var myMD5 = System.Security.Cryptography.MD5.Create())
            {
                var hashValue = myMD5.ComputeHash(Encoding.UTF8.GetBytes(text));
                var builder = new StringBuilder();
                foreach (var item in hashValue)
                {
                    builder.Append(item.ToString("X2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="text">待加密文本字符</param>
        /// <returns></returns>
        public static string SHA1(string text)
        {
            using (var mySHA1 = System.Security.Cryptography.SHA1.Create())
            {
                var hashValue = mySHA1.ComputeHash(Encoding.UTF8.GetBytes(text));
                var builder = new StringBuilder();
                foreach (var item in hashValue)
                {
                    builder.Append(item.ToString("X2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// SHA256加密
        /// </summary>
        /// <param name="text">待加密文本字符</param>
        /// <returns></returns>
        public static string SHA256(string text)
        {
            using (var mySHA256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashValue = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return Convert.ToBase64String(hashValue);
            }
        }
    }
}
