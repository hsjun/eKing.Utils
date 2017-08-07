using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace HNAS.IMP.IMPOAWebServices.Utils
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/13704752/deserialize-xml-to-object-using-dynamic"/>
    public class DynamicXmlToObject : DynamicObject
    {
        XElement _root;
        private DynamicXmlToObject(XElement root)
        {
            _root = root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// dynamic entiry = DynamicXmlToObject.Parse(source);
        /// string userAccount = entity.PropertyName;
        /// </code>
        /// </example>
        public static DynamicXmlToObject Parse(string xmlString)
        {
            return new DynamicXmlToObject(XDocument.Parse(xmlString).Root);
        }

        public static DynamicXmlToObject Load(string filename)
        {
            return new DynamicXmlToObject(XDocument.Load(filename).Root);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            var att = _root.Attribute(binder.Name);
            if (att != null)
            {
                result = att.Value;
                return true;
            }

            var nodes = _root.Elements(binder.Name);
            if (nodes.Count() > 1)
            {
                result = nodes.Select(n => n.HasElements ? (object)new DynamicXmlToObject(n) : n.Value).ToList();
                return true;
            }

            var node = _root.Element(binder.Name);
            if (node != null)
            {
                result = node.HasElements ? (object)new DynamicXmlToObject(node) : node.Value;
                return true;
            }

            return true;
        }
    }
}
