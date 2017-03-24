using depolybot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Xml;
using System.Xml.Serialization;

namespace deploybot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            List<DictionaryMapper> synDict = DeSerializeObject<List<DictionaryMapper>>(Server.MapPath("/") + @"bin/Data/SynsetMappedList.data");
            List<DictionaryMapper> hypernymDict = DeSerializeObject<List<DictionaryMapper>>(Server.MapPath("/") + @"bin/Data/HypernymMappedList.data");
            List<DictionaryMapper> hyponymDict = DeSerializeObject<List<DictionaryMapper>>(Server.MapPath("/") + @"bin/Data/HyponymMappedList.data");
            Dictionary<string, List<string>> synonymDictionary = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> hypernymDictionary = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> hyponymDictionary = new Dictionary<string, List<string>>();
            foreach (var a in synDict)
            {
                synonymDictionary[a.key] = a.value;
            }
            foreach(var a in hypernymDict)
            {
                hypernymDictionary[a.key] = a.value;
            }
            foreach (var a in hyponymDict)
            {
                hyponymDictionary[a.key] = a.value;
            }
            Application["synDict"] = synonymDictionary;
            Application["hypernymDict"] = hypernymDictionary;
            Application["hyponymDict"] = hyponymDictionary;
        }
        public T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }

            return objectOut;
        }
    }
}
