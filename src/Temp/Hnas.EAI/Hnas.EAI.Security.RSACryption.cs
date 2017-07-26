using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Hnas.EAI.Security
{
    public class RSACryption
    {
        public void RSAKey(out string XMLKeys, out string XMLPublicKey)
        {
            try
            {
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                XMLKeys = rSACryptoServiceProvider.ToXmlString(true);
                XMLPublicKey = rSACryptoServiceProvider.ToXmlString(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RSAEncrypt(string XMLPublicKey, string m_strEncryptString)
        {
            string result;
            try
            {
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(XMLPublicKey);
                byte[] bytes = new UnicodeEncoding().GetBytes(m_strEncryptString);
                byte[] array = rSACryptoServiceProvider.Encrypt(bytes, false);
                string text = Convert.ToBase64String(array);
                result = text;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string RSAEncrypt(string XMLPublicKey, byte[] EncryptString)
        {
            string result;
            try
            {
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(XMLPublicKey);
                byte[] array = rSACryptoServiceProvider.Encrypt(EncryptString, false);
                string text = Convert.ToBase64String(array);
                result = text;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string RSADecrypt(string XMLPrivateKey, string m_strDecryptString)
        {
            string result;
            try
            {
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(XMLPrivateKey);
                byte[] array = Convert.FromBase64String(m_strDecryptString);
                byte[] array2 = rSACryptoServiceProvider.Decrypt(array, false);
                string @string = new UnicodeEncoding().GetString(array2);
                result = @string;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string RSADecrypt(string XMLPrivateKey, byte[] DecryptString)
        {
            string result;
            try
            {
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(XMLPrivateKey);
                byte[] array = rSACryptoServiceProvider.Decrypt(DecryptString, false);
                string @string = new UnicodeEncoding().GetString(array);
                result = @string;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool GetHash(string m_strSource, ref byte[] HashData)
        {
            bool result;
            try
            {
                HashAlgorithm hashAlgorithm = HashAlgorithm.Create("MD5");
                byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(m_strSource);
                HashData = hashAlgorithm.ComputeHash(bytes);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool GetHash(string m_strSource, ref string strHashData)
        {
            bool result;
            try
            {
                HashAlgorithm hashAlgorithm = HashAlgorithm.Create("MD5");
                byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(m_strSource);
                byte[] array = hashAlgorithm.ComputeHash(bytes);
                strHashData = Convert.ToBase64String(array);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool GetHash(FileStream objFile, ref byte[] HashData)
        {
            bool result;
            try
            {
                HashAlgorithm hashAlgorithm = HashAlgorithm.Create("MD5");
                HashData = hashAlgorithm.ComputeHash(objFile);
                objFile.Close();
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool GetHash(FileStream objFile, ref string strHashData)
        {
            bool result;
            try
            {
                HashAlgorithm hashAlgorithm = HashAlgorithm.Create("MD5");
                byte[] array = hashAlgorithm.ComputeHash(objFile);
                objFile.Close();
                strHashData = Convert.ToBase64String(array);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool SignatureFormatter(string p_strKeyPrivate, byte[] HashbyteSignature, ref byte[] EncryptedSignatureData)
        {
            bool result;
            try
            {
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(p_strKeyPrivate);
                RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rSACryptoServiceProvider);
                rSAPKCS1SignatureFormatter.SetHashAlgorithm("MD5");
                EncryptedSignatureData = rSAPKCS1SignatureFormatter.CreateSignature(HashbyteSignature);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool SignatureFormatter(string p_strKeyPrivate, byte[] HashbyteSignature, ref string m_strEncryptedSignatureData)
        {
            bool result;
            try
            {
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(p_strKeyPrivate);
                RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rSACryptoServiceProvider);
                rSAPKCS1SignatureFormatter.SetHashAlgorithm("MD5");
                byte[] array = rSAPKCS1SignatureFormatter.CreateSignature(HashbyteSignature);
                m_strEncryptedSignatureData = Convert.ToBase64String(array);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref byte[] EncryptedSignatureData)
        {
            bool result;
            try
            {
                byte[] array = Convert.FromBase64String(m_strHashbyteSignature);
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(p_strKeyPrivate);
                RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rSACryptoServiceProvider);
                rSAPKCS1SignatureFormatter.SetHashAlgorithm("MD5");
                EncryptedSignatureData = rSAPKCS1SignatureFormatter.CreateSignature(array);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref string m_strEncryptedSignatureData)
        {
            bool result;
            try
            {
                byte[] array = Convert.FromBase64String(m_strHashbyteSignature);
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(p_strKeyPrivate);
                RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rSACryptoServiceProvider);
                rSAPKCS1SignatureFormatter.SetHashAlgorithm("MD5");
                byte[] array2 = rSAPKCS1SignatureFormatter.CreateSignature(array);
                m_strEncryptedSignatureData = Convert.ToBase64String(array2);
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool SignatureDeformatter(string p_strKeyPublic, byte[] HashbyteDeformatter, byte[] DeformatterData)
        {
            bool result;
            try
            {
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(p_strKeyPublic);
                RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
                rSAPKCS1SignatureDeformatter.SetHashAlgorithm("MD5");
                if (rSAPKCS1SignatureDeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, byte[] DeformatterData)
        {
            bool result;
            try
            {
                byte[] array = Convert.FromBase64String(p_strHashbyteDeformatter);
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(p_strKeyPublic);
                RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
                rSAPKCS1SignatureDeformatter.SetHashAlgorithm("MD5");
                if (rSAPKCS1SignatureDeformatter.VerifySignature(array, DeformatterData))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool SignatureDeformatter(string p_strKeyPublic, byte[] HashbyteDeformatter, string p_strDeformatterData)
        {
            bool result;
            try
            {
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(p_strKeyPublic);
                RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
                rSAPKCS1SignatureDeformatter.SetHashAlgorithm("MD5");
                byte[] array = Convert.FromBase64String(p_strDeformatterData);
                if (rSAPKCS1SignatureDeformatter.VerifySignature(HashbyteDeformatter, array))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, string p_strDeformatterData)
        {
            bool result;
            try
            {
                byte[] array = Convert.FromBase64String(p_strHashbyteDeformatter);
                CspParameters cspParameters = new CspParameters();
                cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(cspParameters);
                rSACryptoServiceProvider.FromXmlString(p_strKeyPublic);
                RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
                rSAPKCS1SignatureDeformatter.SetHashAlgorithm("MD5");
                byte[] array2 = Convert.FromBase64String(p_strDeformatterData);
                if (rSAPKCS1SignatureDeformatter.VerifySignature(array, array2))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
