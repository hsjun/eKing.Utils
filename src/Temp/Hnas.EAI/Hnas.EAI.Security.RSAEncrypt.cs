using System;
using System.Collections;
using System.Text;
using System.Xml;

namespace Hnas.EAI.Security
{
    public class RSAEncrypt
    {
        public string EncryptPublicKey(string strSource, string strPublicKeyPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(strPublicKeyPath);
            XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("RSAKeyValue");
            string strnNum = elementsByTagName.Item(0).SelectSingleNode("Modulus").InnerText.Trim();
            string strkeyNum = elementsByTagName.Item(0).SelectSingleNode("Exponent").InnerText.Trim();
            byte[] array = this.EncryptString(strSource, strkeyNum, strnNum);
            return Convert.ToBase64String(array);
        }

        public byte[] EncryptPublicKeyByte(string strSource, string strPublicKeyPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(strPublicKeyPath);
            XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("RSAKeyValue");
            string strnNum = elementsByTagName.Item(0).SelectSingleNode("Modulus").InnerText.Trim();
            string strkeyNum = elementsByTagName.Item(0).SelectSingleNode("Exponent").InnerText.Trim();
            return this.EncryptString(strSource, strkeyNum, strnNum);
        }

        public string DecryptPrivateKey(string strSource, string strPrivateKeyPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(strPrivateKeyPath);
            XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("RSAKeyValue");
            string strnNum = elementsByTagName.Item(0).SelectSingleNode("Modulus").InnerText.Trim();
            string strkeyNum = elementsByTagName.Item(0).SelectSingleNode("D").InnerText.Trim();
            byte[] dataBytes = Convert.FromBase64String(strSource);
            return this.DecryptBytes(dataBytes, strkeyNum, strnNum);
        }

        public string DecryptPrivateKeyByte(byte[] dSource, string strPrivateKeyPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(strPrivateKeyPath);
            XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("RSAKeyValue");
            string strnNum = elementsByTagName.Item(0).SelectSingleNode("Modulus").InnerText.Trim();
            string strkeyNum = elementsByTagName.Item(0).SelectSingleNode("D").InnerText.Trim();
            return this.DecryptBytes(dSource, strkeyNum, strnNum);
        }

        private byte[] EncryptString(string strData, string strkeyNum, string strnNum)
        {
            BigInteger exp = new BigInteger(strkeyNum, 16);
            BigInteger n = new BigInteger(strnNum, 16);
            byte[] bytes = Encoding.UTF8.GetBytes(strData);
            int num = bytes.Length;
            int num2;
            if (num % 120 == 0)
            {
                num2 = num / 120;
            }
            else
            {
                num2 = num / 120 + 1;
            }
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < num2; i++)
            {
                int num3;
                if (num >= 120)
                {
                    num3 = 120;
                }
                else
                {
                    num3 = num;
                }
                byte[] array = new byte[num3];
                Array.Copy(bytes, i * 120, array, 0, num3);
                Encoding.UTF8.GetString(array);
                BigInteger bigInteger = new BigInteger(array);
                BigInteger bigInteger2 = bigInteger.modPow(exp, n);
                string text = bigInteger2.ToHexString();
                if (text.Length < 256)
                {
                    while (text.Length != 256)
                    {
                        text = "0" + text;
                    }
                }
                byte[] array2 = new byte[128];
                for (int j = 0; j < array2.Length; j++)
                {
                    array2[j] = Convert.ToByte(text.Substring(j * 2, 2), 16);
                }
                arrayList.AddRange(array2);
                num -= num3;
            }
            byte[] array3 = new byte[arrayList.Count];
            for (int k = 0; k < arrayList.Count; k++)
            {
                array3[k] = (byte)arrayList[k];
            }
            return array3;
        }

        private string DecryptBytes(byte[] dataBytes, string strkeyNum, string strnNum)
        {
            BigInteger exp = new BigInteger(strkeyNum, 16);
            BigInteger n = new BigInteger(strnNum, 16);
            int num = dataBytes.Length;
            int num2;
            if (num % 128 == 0)
            {
                num2 = num / 128;
            }
            else
            {
                num2 = num / 128 + 1;
            }
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < num2; i++)
            {
                int num3;
                if (num >= 128)
                {
                    num3 = 128;
                }
                else
                {
                    num3 = num;
                }
                byte[] array = new byte[num3];
                Array.Copy(dataBytes, i * 128, array, 0, num3);
                BigInteger bigInteger = new BigInteger(array);
                BigInteger bigInteger2 = bigInteger.modPow(exp, n);
                byte[] bytes = bigInteger2.getBytes();
                arrayList.AddRange(bytes);
                num -= num3;
            }
            byte[] array2 = new byte[arrayList.Count];
            for (int j = 0; j < arrayList.Count; j++)
            {
                array2[j] = (byte)arrayList[j];
            }
            return Encoding.UTF8.GetString(array2);
        }

        public string SignMD5WithRSA(string strSource, string strPrivateKeyPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(strPrivateKeyPath);
            XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("RSAKeyValue");
            elementsByTagName.Item(0).SelectSingleNode("Modulus").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName.Item(0).SelectSingleNode("Modulus").InnerText.Trim()));
            elementsByTagName.Item(0).SelectSingleNode("Exponent").InnerText = Convert.ToBase64String(this.GetBytes(elementsByTagName.Item(0).SelectSingleNode("Exponent").InnerText.Trim()));
            elementsByTagName.Item(0).SelectSingleNode("P").InnerText=Convert.ToBase64String(this.GetBytes(elementsByTagName.Item(0).SelectSingleNode("P").InnerText.Trim()));
            elementsByTagName.Item(0).SelectSingleNode("Q").InnerText=Convert.ToBase64String(this.GetBytes(elementsByTagName.Item(0).SelectSingleNode("Q").InnerText.Trim()));
            elementsByTagName.Item(0).SelectSingleNode("DP").InnerText=Convert.ToBase64String(this.GetBytes(elementsByTagName.Item(0).SelectSingleNode("DP").InnerText.Trim()));
            elementsByTagName.Item(0).SelectSingleNode("DQ").InnerText=Convert.ToBase64String(this.GetBytes(elementsByTagName.Item(0).SelectSingleNode("DQ").InnerText.Trim()));
            elementsByTagName.Item(0).SelectSingleNode("InverseQ").InnerText=Convert.ToBase64String(this.GetBytes(elementsByTagName.Item(0).SelectSingleNode("InverseQ").InnerText.Trim()));
            elementsByTagName.Item(0).SelectSingleNode("D").InnerText=Convert.ToBase64String(this.GetBytes(elementsByTagName.Item(0).SelectSingleNode("D").InnerText.Trim()));
            string innerXml = xmlDocument.InnerXml;
            RSACryption rSACryption = new RSACryption();
            string strHashbyteSignature = "";
            rSACryption.GetHash(strSource, ref strHashbyteSignature);
            string result = "";
            rSACryption.SignatureFormatter(innerXml, strHashbyteSignature, ref result);
            return result;
        }

        public bool UnsignMD5WithRSA(string strSignData, string strSource, string strPublicKeyPath)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(strPublicKeyPath);
            XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("RSAKeyValue");
            elementsByTagName.Item(0).SelectSingleNode("Modulus").InnerText= Convert.ToBase64String(this.GetBytes(elementsByTagName.Item(0).SelectSingleNode("Modulus").InnerText.Trim()));
            elementsByTagName.Item(0).SelectSingleNode("Exponent").InnerText= Convert.ToBase64String(this.GetBytes(elementsByTagName.Item(0).SelectSingleNode("Exponent").InnerText.Trim()));
            string innerXml = xmlDocument.InnerXml;
            RSACryption rSACryption = new RSACryption();
            string p_strHashbyteDeformatter = "";
            rSACryption.GetHash(strSource, ref p_strHashbyteDeformatter);
            return rSACryption.SignatureDeformatter(innerXml, p_strHashbyteDeformatter, strSignData);
        }

        private byte[] GetBytes(string num)
        {
            BigInteger bigInteger = new BigInteger(num, 16);
            string text = bigInteger.ToString(2);
            if (text.Length % 8 > 0)
            {
                text = new string('0', 8 - text.Length % 8) + text;
            }
            byte[] array = new byte[text.Length / 8];
            for (int i = 0; i < array.Length; i++)
            {
                string text2 = text.Substring(8 * i, 8);
                array[i] = Convert.ToByte(text2, 2);
            }
            return array;
        }
    }
}
