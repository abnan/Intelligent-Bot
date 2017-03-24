using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace depolybot
{
    public class DictionaryMapper
    {
        [XmlAttribute]
        public string key;
        [XmlAttribute]
        public List<string> value;
        public DictionaryMapper()
        {
            value = new List<string>();
        }
        public DictionaryMapper(string k, List<string> v)
        {
            key = k;
            value = v;
        }
    }
}
