using System;
using System.Collections;
using System.Text;
using System.Xml;

namespace Hnas.EAI.Security
{
    public class RSAEncrypt
    {
        // Methods
        private string DecryptBytes(byte[] dataBytes, string strkeyNum, string strnNum)
        {
            BigInteger exp = new BigInteger(strkeyNum, 0x10);
            BigInteger n = new BigInteger(strnNum, 0x10);
            int length = dataBytes.Length;
            int num2 = 0;
            int num3 = 0;

            if ((length % 0x80) == 0)
            {
                num2 = length / 0x80;
            }
            else
            {
                num2 = (length / 0x80) + 1;
            }

            ArrayList list = new ArrayList();

            for (int i = 0; i < num2; i++)
            {
                if (length >= 0x80)
                {
                    num3 = 0x80;
                }
                else
                {
                    num3 = length;
                }

                byte[] destinationArray = new byte[num3];
                Array.Copy(dataBytes, i * 0x80, destinationArray, 0, num3);
                BigInteger integer3 = new BigInteger(destinationArray);
                byte[] c = integer3.modPow(exp, n).getBytes();
                list.AddRange(c);
                length -= num3;
            }
            byte[] bytes = new byte[list.Count];

            for (int j = 0; j < list.Count; j++)
            {
                bytes[j] = (byte)list[j];
            }
            return Encoding.UTF8.GetString(bytes);
        }

        public string DecryptPrivateKey(string strSource, string strPrivateKeyPath)
        {
            XmlDocument document = new XmlDocument();
            document.Load(strPrivateKeyPath);
            XmlNodeList elementsByTagName = document.GetElementsByTagName("RSAKeyValue");
            string strnNum = elementsByTagName[0].SelectSingleNode("Modulus").InnerText.Trim();
            string strkeyNum = elementsByTagName[0].SelectSingleNode("D").InnerText.Trim();
            byte[] dataBytes = Convert.FromBase64String(strSource);
            return this.DecryptBytes(dataBytes, strkeyNum, strnNum);
        }

        public string DecryptPrivateKeyByte(byte[] dSource, string strPrivateKeyPath)
        {
            XmlDocument document = new XmlDocument();
            document.Load(strPrivateKeyPath);
            XmlNodeList elementsByTagName = document.GetElementsByTagName("RSAKeyValue");
            string strnNum = elementsByTagName[0].SelectSingleNode("Modulus").InnerText.Trim();
            string strkeyNum = elementsByTagName[0].SelectSingleNode("D").InnerText.Trim();
            return this.DecryptBytes(dSource, strkeyNum, strnNum);
        }

        public string EncryptPublicKey(string strSource, string strPublicKeyPath)
        {
            XmlDocument document = new XmlDocument();
            document.Load(strPublicKeyPath);
            XmlNodeList elementsByTagName = document.GetElementsByTagName("RSAKeyValue");
            string strnNum = elementsByTagName[0].SelectSingleNode("Modulus").InnerText.Trim();
            string strkeyNum = elementsByTagName[0].SelectSingleNode("Exponent").InnerText.Trim();
            return Convert.ToBase64String(this.EncryptString(strSource, strkeyNum, strnNum));
        }

        public byte[] EncryptPublicKeyByte(string strSource, string strPublicKeyPath)
        {
            XmlDocument document = new XmlDocument();
            document.Load(strPublicKeyPath);
            XmlNodeList elementsByTagName = document.GetElementsByTagName("RSAKeyValue");
            string strnNum = elementsByTagName[0].SelectSingleNode("Modulus").InnerText.Trim();
            string strkeyNum = elementsByTagName[0].SelectSingleNode("Exponent").InnerText.Trim();
            return this.EncryptString(strSource, strkeyNum, strnNum);
        }

        private byte[] EncryptString(string strData, string strkeyNum, string strnNum)
        {
            BigInteger exp = new BigInteger(strkeyNum, 0x10);
            BigInteger n = new BigInteger(strnNum, 0x10);
            byte[] bytes = Encoding.UTF8.GetBytes(strData);
            int length = bytes.Length;
            int num2 = 0;
            int num3 = 0;

            if ((length % 120) == 0)
            {
                num2 = length / 120;
            }
            else
            {
                num2 = (length / 120) + 1;
            }

            ArrayList list = new ArrayList();

            for (int i = 0; i < num2; i++)
            {
                if (length >= 120)
                {
                    num3 = 120;
                }
                else
                {
                    num3 = length;
                }

                byte[] destinationArray = new byte[num3];
                Array.Copy(bytes, i * 120, destinationArray, 0, num3);
                Encoding.UTF8.GetString(destinationArray);
                BigInteger integer3 = new BigInteger(destinationArray);
                string str = integer3.modPow(exp, n).ToHexString();

                if (str.Length < 0x100)
                {
                    while (str.Length != 0x100)
                    {
                        str = "0" + str;
                    }
                }

                byte[] c = new byte[0x80];

                for (int k = 0; k < c.Length; k++)
                {
                    c[k] = Convert.ToByte(str.Substring(k * 2, 2), 0x10);
                }
                list.AddRange(c);
                length -= num3;
            }

            byte[] buffer4 = new byte[list.Count];

            for (int j = 0; j < list.Count; j++)
            {
                buffer4[j] = (byte)list[j];
            }
            return buffer4;
        }

        private byte[] GetBytes(string num)
        {
            string str = new BigInteger(num, 0x10).ToString(2);

            if ((str.Length % 8) > 0)
            {
                str = new string('0', 8 - (str.Length % 8)) + str;
            }

            byte[] buffer = new byte[str.Length / 8];

            for (int i = 0; i < buffer.Length; i++)
            {
                string str2 = str.Substring(8 * i, 8);
                buffer[i] = Convert.ToByte(str2, 2);
            }
            return buffer;
        }

        public string SignMD5WithRSA(string strSource, string strPrivateKeyPath)
        {
            XmlDocument document = new XmlDocument();
            document.Load(strPrivateKeyPath);
            XmlNodeList elementsByTagName = document.GetElementsByTagName("RSAKeyValue");
            elementsByTagName[0].SelectSingleNode("Modulus").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName[0].SelectSingleNode("Modulus").InnerText.Trim()));
            elementsByTagName[0].SelectSingleNode("Exponent").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName[0].SelectSingleNode("Exponent").InnerText.Trim()));
            elementsByTagName[0].SelectSingleNode("P").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName[0].SelectSingleNode("P").InnerText.Trim()));
            elementsByTagName[0].SelectSingleNode("Q").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName[0].SelectSingleNode("Q").InnerText.Trim()));
            elementsByTagName[0].SelectSingleNode("DP").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName[0].SelectSingleNode("DP").InnerText.Trim()));
            elementsByTagName[0].SelectSingleNode("DQ").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName[0].SelectSingleNode("DQ").InnerText.Trim()));
            elementsByTagName[0].SelectSingleNode("InverseQ").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName[0].SelectSingleNode("InverseQ").InnerText.Trim()));
            elementsByTagName[0].SelectSingleNode("D").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName[0].SelectSingleNode("D").InnerText.Trim()));
            string innerXml = document.InnerXml;
            RSACryption cryption = new RSACryption();
            string strHashData = "";
            cryption.GetHash(strSource, ref strHashData);
            string str3 = "";
            cryption.SignatureFormatter(innerXml, strHashData, ref str3);
            return str3;
        }

        public bool UnsignMD5WithRSA(string strSignData, string strSource, string strPublicKeyPath)
        {
            XmlDocument document = new XmlDocument();
            document.Load(strPublicKeyPath);
            XmlNodeList elementsByTagName = document.GetElementsByTagName("RSAKeyValue");
            elementsByTagName[0].SelectSingleNode("Modulus").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName[0].SelectSingleNode("Modulus").InnerText.Trim()));
            elementsByTagName[0].SelectSingleNode("Exponent").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName[0].SelectSingleNode("Exponent").InnerText.Trim()));
            string innerXml = document.InnerXml;
            RSACryption cryption = new RSACryption();
            string strHashData = "";
            cryption.GetHash(strSource, ref strHashData);
            if (!cryption.SignatureDeformatter(innerXml, strHashData, strSignData))
            {
                return false;
            }
            return true;
        }
    }
}