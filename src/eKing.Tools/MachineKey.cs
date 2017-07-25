using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace eKing.Tools
{
    public class MachineKey
    {
        /// <summary>
        /// 生成Webconfig的machineKey元素
        /// </summary>
        /// <param name="decryptionKeyLen"></param>
        /// <param name="validationKeyLen"></param>
        /// <returns></returns>
        /// <remarks>
        /// decryptionKey 的有效值为 8 或 24。此属性将为数据加密标准 (DES) 创建一个 16 字节密钥，或者为三重 DES 创建一个 48 字节密钥。
        /// validationKey 的有效值为 20 到 64。此属性将创建长度从 40 到 128 字节的密钥
        /// 
        /// Use the RNGCryptoServiceProvider class to generate a cryptographically strong random number.
        /// Choose an appropriate key size.The recommended key lengths are as follows: 
        /// For SHA1, set the validationKey to 64 bytes(128 hexadecimal characters).
        /// For AES, set the decryptionKey to 32 bytes(64 hexadecimal characters).
        /// For 3DES, set the decryptionKey to 24 bytes(48 hexadecimal characters).
        /// https://support.microsoft.com/zh-cn/help/312906/how-to-create-keys-by-using-visual-c-.net-for-use-in-forms-authentication
        /// </remarks>
        public static string BuildMachineKey(int decryptionKeyLen = 24, int validationKeyLen = 64)
        {
            string decryptionKey = CreateKey(decryptionKeyLen);

            string validationKey = CreateKey(validationKeyLen);

            return string.Format("<machineKey validationKey=\"{0}\" decryptionKey=\"{1}\" validation=\"{2}\"/>",
                validationKey, decryptionKey, "SHA1");
        }

        static string CreateKey(int numBytes)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[numBytes];

            rng.GetBytes(buff);

            return BytesToHexString(buff);
        }

        static string BytesToHexString(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(64);
            for (int counter = 0; counter < bytes.Length; counter++)
            {
                hexString.Append(String.Format("{0:X2}", bytes[counter]));
            }

            return hexString.ToString();
        }
    }
}
