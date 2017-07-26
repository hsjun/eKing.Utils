using System;

namespace Hnas.EAI.ESBEntity
{
    public class MessageHeader
    {
        public string messageType;

        public string userId;

        public string userPwd;

        public MessageHeader()
        {
            this.messageType = string.Empty;
            this.userId = string.Empty;
            this.userPwd = string.Empty;
        }

        public MessageHeader(string messageType, string userId, string userPwd)
        {
            this.messageType = messageType;
            this.userId = userId;
            this.userPwd = userPwd;
        }
    }
}
