using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace testxml
{
    class Program
    {
        static void Main(string[] args)
        {
            string xml = @"<root rootAttrib='root to'>  
                                <StandardValues attrib1 ='Test'>    
                                        <ButtonYES a1='1' a2='2'>Ja</ButtonYES>
                                        <ButtonNO>Nei</ButtonNO>
                                </StandardValues>
                                <Page1>
                                    <Key_Head>2011 Applications</Key_Head>
                                    <Key_Title>Title from 2011</Key_Title>
                                    <Key_Param1>Parameter value</Key_Param1>
                               </Page1>
                                <Page2>
                                    <Page_Head attrib1 = 'test'>2011 Applications</Page_Head>
                                    <page_Title>Title from 2011</page_Title>
                                    <CustomParam1>Parameter value</CustomParam1>
                               </Page2>
                    </root>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);

          var nodes = doc.SelectNodes(@"/*");
          var result = Iterate(nodes, nodes[0].Attributes);

            Console.Read();
        }

        private static object Iterate(XmlNodeList nodes, XmlAttributeCollection attributeCollection)
        {
            var result = new Dictionary<string, object>();
            var attributes = new Dictionary<object, object>();

            foreach (XmlNode node in nodes)
            {
              result.Add(node.Name, node.FirstChild.HasChildNodes ? Iterate(node.ChildNodes, node.Attributes) : GetAttrib(node));
            }

            foreach (XmlAttribute attrib in attributeCollection)
            {
                result.Add(attrib.Name, attrib.Value);
            }

            return result;
        }

        private static Dictionary<object, object> GetAttrib(XmlNode e)
        {
            var result = new Dictionary<object, object>();

            if (e.Attributes is null)
            {
                result.Add("#text", e.InnerText);
                return result;
            } 

            foreach (XmlAttribute attrib in e.Attributes)
            {
               result.Add(attrib.Name, attrib.Value);
            }

            result.Add("#text", e.InnerText);

            return result;
        }

        private static object ConvertXmlToDic(XElement element)
        {
            var result = element.Elements().
                Select(
                    e => new
                    {
                        Key = e.Name,
                        Value = (e.Descendants().Count() == 0) ?
                         new { e.Value, Attributes = GetAttributes(e) } : ConvertXmlToDic(e)
                    }
                ).ToDictionary(e => e.Key, e => e.Value);
            return result;
        }

        private static Dictionary<object, object> GetAttributes(XElement e)
        {
            var a = new Dictionary<object, object>();

            foreach (var attrib in e.Attributes())
            {
                a.Add(attrib.Name, attrib.Value);
            }

            return a;
        }
    }
}
