using System;

namespace Hnas.EAI.ESBEntity
{
    public class MessageRequest
    {
        public MessageHeader messageHeader;

        public Parameter[] parameter;

        public MessageRequest()
        {
            this.messageHeader = new MessageHeader();
            this.parameter = null;
        }

        public MessageRequest(MessageHeader messageHeader, Parameter[] parameter)
        {
            this.messageHeader = messageHeader;
            this.parameter = parameter;
        }
    }
}
