using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace eKing.Utils.Cryptography
{
    /// <summary>
    /// Des加解密
    /// </summary>
    public class DesCrypto
    {
        #region Des算法加密过程
        /// <summary>
        /// Des算法加密过程
        /// </summary>
        /// <param name="strEncrypt">需要加密的内容</param>
        /// <param name="strKey">密钥</param>
        /// <returns></returns>
        public static string DesEncrypt(string strEncrypt, string strKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //把字符串放到byte数组中
            byte[] inputByteArray = Encoding.Default.GetBytes(strEncrypt);
            //建立加密对象的密钥和偏移量
            //建议密码长度为8位
            strKey = matchToKey(strKey, 8);
            des.Key = ASCIIEncoding.ASCII.GetBytes(strKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(strKey);
            //内存流
            MemoryStream ms = new MemoryStream();
            //加密转换流
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            //将byte数组通过加密器写入内存流
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //得到加密后的字符串
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }
        #endregion

        #region Des算法解密过程
        /// <summary>
        /// Des算法解密过程
        /// </summary>
        /// <param name="strDecrypt">需要解密的内容</param>
        /// <param name="strKey">密钥</param>
        /// <returns></returns>
        public static string DesDecrypt(string strDecrypt, string strKey)
        {
            string strRe = "";
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //将字符串转换成byte型数组
            byte[] inputByteArray = new byte[strDecrypt.Length / 2];
            for (int x = 0; x < strDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(strDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            //建立加密对象的密钥和偏移量，此值重要，不能修改
            //转换成8位的密码
            strKey = matchToKey(strKey, 8);
            des.Key = ASCIIEncoding.ASCII.GetBytes(strKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(strKey);
            //内存流
            MemoryStream ms = new MemoryStream();
            //解密转换流
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            //将加密后的数据通解密器转化成内存流
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象
            StringBuilder ret = new StringBuilder();
            strRe = System.Text.Encoding.Default.GetString(ms.ToArray());

            return strRe;
        }
        #endregion

        #region 将字符串转换成指定位数，不足补'A',多了截取
        /// <summary>
        /// 将字符串转换成指定位数，不足补'A',多了截取
        /// </summary>
        /// <param name="strKey">初始字符串</param>
        /// <param name="iLength">要得到的字符串长度</param>
        /// <returns>转换后的结果</returns>
        private static string matchToKey(string strKey, int iLength)
        {
            if (strKey.Length < iLength)
            {
                string strTemp = "";
                for (int i = 0; i < iLength - strKey.Length; i++)
                {
                    strTemp += "A";//不足位用字母'A'补齐
                }
                strKey += strTemp;
            }
            else
            {
                strKey = strKey.Substring(0, iLength);
            }
            return strKey;
        }
        #endregion
    }
}
