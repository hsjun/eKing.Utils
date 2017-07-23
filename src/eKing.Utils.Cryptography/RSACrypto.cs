using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;


namespace eKing.Utils.Cryptography
{
    /// <summary>
    /// RSA非对称加密解密类
    /// </summary>
    /// <remarks></remarks>
    public class RSACrypto
    {
        #region 加解密
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="content">待加密字符串</param>
        /// <returns>加密后字符串</returns>
        public static string Encrypt(string xmlPublicKey, string content)
        {
            return Encrypt(xmlPublicKey, new UnicodeEncoding().GetBytes(content));


            //string result;
            //try
            //{
            //    RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            //    provider.FromXmlString(xmlPublicKey);

            //    byte[] bytes = new UnicodeEncoding().GetBytes(content);
            //    result = Convert.ToBase64String(provider.Encrypt(bytes, false));
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //return result;
        }

        public static string Encrypt(string xmlPublicKey, byte[] content)
        {
            string result;
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.FromXmlString(xmlPublicKey);

                result = Convert.ToBase64String(provider.Encrypt(content, false));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <param name="content">待解密字符串</param>
        /// <returns>解密后字符串</returns>
        public static string Decrypt(string xmlPrivateKey, string content)
        {
            return Decrypt(xmlPrivateKey, Convert.FromBase64String(content));

            //string result;
            //try
            //{
            //    RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            //    provider.FromXmlString(xmlPrivateKey);

            //    byte[] bytes = Convert.FromBase64String(content);
            //    result = new UnicodeEncoding().GetString(provider.Decrypt(bytes, false));
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            //return result;
        }

        public static string Decrypt(string xmlPrivateKey, byte[] content)
        {
            string result;
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.FromXmlString(xmlPrivateKey);

                result = new UnicodeEncoding().GetString(provider.Decrypt(content, false));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        #endregion


        /// <summary>
        /// 获取XML密钥文件
        /// </summary>
        /// <param name="keyFilePath"></param>
        /// <returns></returns>
        public static string GetKeyFile(string keyFilePath)
        {
            if (!File.Exists(keyFilePath))
            {
                keyFilePath = AppDomain.CurrentDomain.BaseDirectory + keyFilePath;
            }

            XmlDocument document = new XmlDocument();
            document.Load(keyFilePath);

            XmlNodeList elementsByTagName = document.GetElementsByTagName("RSAKeyValue");

            return document.InnerXml;
        }


        /// <summary>
        /// 
        /// </summary>
        public static void CreateKeyFile()
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            using (StreamWriter writer = new StreamWriter("PrivateKey.xml"))  //这个文件要保密...
            {
                writer.WriteLine(provider.ToXmlString(true));
            }

            using (StreamWriter writer = new StreamWriter("PublicKey.xml"))
            {
                writer.WriteLine(provider.ToXmlString(false));
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="xmlPublicKey"></param>
        public static void CreateKey(out string xmlPrivateKey, out string xmlPublicKey)
        {
            try
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();

                xmlPrivateKey = provider.ToXmlString(true);

                xmlPublicKey = provider.ToXmlString(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 签名验证
        //public string GetStrMd5(string ConvertString)
        //{
        //    string strBodyBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(ConvertString));
        //    string t2 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strBodyBase64, "MD5").ToUpper();
        //    return t2;
        //}

        /// <summary>  
        /// 对原始数据Hash加密
        /// </summary>
        /// <param name="source">待加密数据</param>
        /// <returns>加密后数据</returns> 
        public static string GetHash(string source, string encodingName = "UTF-8")
        {
            byte[] bytes = Encoding
                .GetEncoding(string.IsNullOrEmpty(encodingName) ? Encoding.Default.EncodingName : encodingName)
                .GetBytes(source);            

            //byte[] bytes = System.Text.Encoding.Default.GetBytes(source);
            //byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(source);   
            //byte[] bytes = Encoding.UTF8.GetBytes(source);
            //byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(source);
            //byte[] bytes = Encoding.Unicode.GetBytes(source);

            HashAlgorithm algorithm = HashAlgorithm.Create("MD5");
            byte[] hash = algorithm.ComputeHash(bytes);
            //byte[] hash = MD5.Create().ComputeHash(bytes);

            //SHA256Managed sha2 = new SHA256Managed();

            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// 对加密后的密文进行签名
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="hashbyteSignature">MD5加密后的密文</param>
        /// <returns></returns> 
        public static string CreateSignature(string privateKey, string hashbyteSignature)
        {
            byte[] rgbHash = Convert.FromBase64String(hashbyteSignature);

            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(privateKey);
            //或者
            //byte[] privateKeyBytes = Convert.FromBase64String(privateKey);
            //provider.ImportCspBlob(privateKeyBytes); //?? 报错提示：不正确的提供程序版本
            //publicKey用的是ASN.1 DER编码，因此不能直接用ImportCspBlob（它使用另外一种格式）。

            RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(provider);
            formatter.SetHashAlgorithm("MD5");
            //formatter.SetHashAlgorithm("SHA256");

            byte[] signature = formatter.CreateSignature(rgbHash);
            return Convert.ToBase64String(signature);
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="cypherText">密文</param>
        /// <param name="plainText">待比较明文</param>
        /// <returns></returns> 
        public static bool VerifySignature(string publicKey, string cypherText, string plainText)
        {
            try
            {
                byte[] rgbSignature = Convert.FromBase64String(cypherText);


                byte[] rgbHash = Convert.FromBase64String(GetHash(plainText));

                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.FromXmlString(publicKey);

                return provider.VerifyData(rgbHash, "MD5", rgbSignature);

                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(provider);
                deformatter.SetHashAlgorithm("MD5");
                //formatter.SetHashAlgorithm("SHA256");

                if (deformatter.VerifySignature(rgbHash, rgbSignature))
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pemFileConent"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// RSAParameters paraPub = ConvertFromPublicKey(publicKey);
        /// RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
        /// rsaProvider.ImportParameters(paraPub);
        /// </code>
        /// </example>
        private static RSAParameters ConvertFromPublicKey(string pemFileConent)
        {
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 162)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, 29, pemModulus, 0, 128);
            Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            return para;
        }

        private static RSAParameters ConvertFromPrivateKey(string pemFileConent)
        {
            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 609)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }

            int index = 11;
            byte[] pemModulus = new byte[128];
            Array.Copy(keyData, index, pemModulus, 0, 128);

            index += 128;
            index += 2;//141  
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, index, pemPublicExponent, 0, 3);

            index += 3;
            index += 4;//148  
            byte[] pemPrivateExponent = new byte[128];
            Array.Copy(keyData, index, pemPrivateExponent, 0, 128);

            index += 128;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//279  
            byte[] pemPrime1 = new byte[64];
            Array.Copy(keyData, index, pemPrime1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//346  
            byte[] pemPrime2 = new byte[64];
            Array.Copy(keyData, index, pemPrime2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//412/413  
            byte[] pemExponent1 = new byte[64];
            Array.Copy(keyData, index, pemExponent1, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//479/480  
            byte[] pemExponent2 = new byte[64];
            Array.Copy(keyData, index, pemExponent2, 0, 64);

            index += 64;
            index += ((int)keyData[index + 1] == 64 ? 2 : 3);//545/546  
            byte[] pemCoefficient = new byte[64];
            Array.Copy(keyData, index, pemCoefficient, 0, 64);

            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            para.D = pemPrivateExponent;
            para.P = pemPrime1;
            para.Q = pemPrime2;
            para.DP = pemExponent1;
            para.DQ = pemExponent2;
            para.InverseQ = pemCoefficient;
            return para;
        }
        #endregion

    }
}

