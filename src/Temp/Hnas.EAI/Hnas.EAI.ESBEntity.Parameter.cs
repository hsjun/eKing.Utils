using System;

namespace Hnas.EAI.ESBEntity
{
    public class Parameter
    {
        public string paramName;

        public string paramValue;

        public Parameter()
        {
            this.paramName = string.Empty;
            this.paramValue = string.Empty;
        }

        public Parameter(string paramName, string paramValue)
        {
            this.paramName = paramName;
            this.paramValue = paramValue;
        }

        public string toString()
        {
            string result = string.Empty;
            if (this.paramValue != null && !this.paramValue.Trim().Equals(string.Empty))
            {
                result = string.Concat(new string[]
                {
                    "<",
                    this.paramName,
                    ">",
                    this.MessageReplace(this.paramValue),
                    "</",
                    this.paramName,
                    ">"
                });
            }
            else
            {
                result = "<" + this.paramName + "/>";
            }
            return result;
        }

        private string MessageReplace(string paramValue)
        {
            paramValue = paramValue.Replace("&", "&amp;");
            paramValue = paramValue.Replace("\"", "&quot;");
            paramValue = paramValue.Replace("<", "&lt;");
            paramValue = paramValue.Replace(">", "&gt;");
            paramValue = paramValue.Replace("'", "&apos;");
            return paramValue;
        }
    }
}
