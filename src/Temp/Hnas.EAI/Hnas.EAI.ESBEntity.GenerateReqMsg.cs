using Hnas.EAI.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Hnas.EAI.ESBEntity
{
    public class GenerateReqMsg
    {
        public static string generate(MessageRequest messageRequest, int expireMinute, string privateKeyPath)
        {
            MessageHeader messageHeader = messageRequest.messageHeader;
            Parameter[] parameter = messageRequest.parameter;
            string text = Guid.NewGuid().ToString();
            string text2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string text3 = DateTime.Now.AddMinutes((double)expireMinute).ToString("yyyy-MM-dd HH:mm:ss");
            string messageType = messageHeader.messageType;
            string userId = messageHeader.userId;
            string userPwd = messageHeader.userPwd;
            Random random = new Random();
            string text4 = random.Next().ToString();
            string text5 = Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes(text4));
            if (messageType.Trim().Equals(string.Empty) || userId.Trim().Equals(string.Empty) || userPwd.Trim().Equals(string.Empty))
            {
                Exception ex = new Exception("消息请求对象MessageRequest的消息头MessageHeader所有字段都不可以为空");
                throw ex;
            }
            string text6 = "<MsgRequest><MsgHeader><MessageID>" + text + "</MessageID>";
            text6 = text6 + "<SendTime>" + text2 + "</SendTime>";
            text6 = text6 + "<MessageType>" + messageType + "</MessageType>";
            text6 = text6 + "<UserId>" + userId + "</UserId>";
            text6 = text6 + "<UserPwd>" + userPwd + "</UserPwd>";
            text6 = text6 + "<ExpireTime>" + text3 + "</ExpireTime>";
            text6 = text6 + "<Nonce>" + text5 + "</Nonce>";
            text6 += "</MsgHeader>";
            if (parameter != null && parameter.Length > 0)
            {
                text6 += "<Parameter>";
                for (int i = 0; i < parameter.Length; i++)
                {
                    text6 += parameter[i].toString();
                }
                text6 += "</Parameter>";
            }
            else
            {
                text6 += "<Parameter/>";
            }
            text6 += "</MsgRequest>";
            RSAEncrypt rSAEncrypt = new RSAEncrypt();
            try
            {
                string text7 = rSAEncrypt.SignMD5WithRSA(text6, privateKeyPath);
                text6 = text6.Replace("<UserPwd>" + userPwd + "</UserPwd>", "<UserPwd>" + text7 + "</UserPwd>");
            }
            catch (Exception ex2)
            {
                throw ex2;
            }
            return text6;
        }

        public static string generate(MessageRequest messageRequest, int expireMinute, string privateKeyPath, string esbPublicKeyPath)
        {
            string text = GenerateReqMsg.generate(messageRequest, expireMinute, privateKeyPath);
            RSAEncrypt rSAEncrypt = new RSAEncrypt();
            try
            {
                text = rSAEncrypt.EncryptPublicKey(text, esbPublicKeyPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return text;
        }

        public static string generate(MessageRequest messageRequest, string privateKeyPath)
        {
            int expireMinute = 5;
            return GenerateReqMsg.generate(messageRequest, expireMinute, privateKeyPath);
        }

        public static string generate(MessageRequest messageRequest, string privateKeyPath, string esbPublicKeyPath)
        {
            int expireMinute = 5;
            return GenerateReqMsg.generate(messageRequest, expireMinute, privateKeyPath, esbPublicKeyPath);
        }
    }
}