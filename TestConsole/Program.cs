using deploybot;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using WordNetLib;

namespace depolybot
{
    class Program
    {
        static void Main(string[] args)
        {
            string poi = "hello Mr. how is Mrdshu your St.";
            string iuy = poi.Replace("Mr.", "Mrs").Replace("St.","St");
            //CreateBufferFiles();
            HtmlWeb web = new HtmlWeb();
            int s_index = 0, e_index = 0;
            HtmlDocument doc = web.Load("https://en.wikipedia.org/wiki/Elon_Musk");
            //HtmlDocument doc = web.Load("https://en.wikipedia.org/wiki/Mahatma_Gandhi");
            HtmlNodeCollection contents = doc.DocumentNode.SelectNodes("//p");
            List<string> contentlist = new List<string>();
            List<string> searchContent = new List<string>();
            foreach (HtmlNode content in contents)
            {
                s_index = content.InnerHtml.ToString().IndexOf("<sup");
                while (s_index != -1)
                {
                    e_index = content.InnerHtml.ToString().IndexOf("</sup>");
                    content.InnerHtml = content.InnerHtml.Replace(content.InnerHtml.Substring(s_index, e_index - s_index + 6), "");
                    s_index = content.InnerHtml.ToString().IndexOf("<sup");
                }
                //Console.WriteLine(content.InnerText);
                s_index = 0;
            }

            for (int j = 0; j < contents.Count; j++)
            {
                var paraTemp = contents[j].InnerText.Replace("Mr.", "Mr").Replace("Mrs.", "Mrs").Replace("Dr.", "Dr").Replace("St.", "St");
                var temp = paraTemp.Split('.').ToList();
                searchContent.AddRange(temp);
            }
            List<DictionaryMapper> synDict = DeSerializeObject<List<DictionaryMapper>>(@"c:\users\abnan\documents\visual studio 2015\Projects\deploybot\Data\SynsetMappedList.data");
            List<DictionaryMapper> hypernymDict = DeSerializeObject<List<DictionaryMapper>>(@"c:\users\abnan\documents\visual studio 2015\Projects\deploybot\Data\HypernymMappedList.data");
            List<DictionaryMapper> hyponymDict = DeSerializeObject<List<DictionaryMapper>>(@"c:\users\abnan\documents\visual studio 2015\Projects\deploybot\Data\HyponymMappedList.data");
            Dictionary<string, List<string>> synonymDictionary = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> hypernymDictionary = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> hyponymDictionary = new Dictionary<string, List<string>>();
            foreach (var a in synDict)
            {
                synonymDictionary[a.key] = a.value;
            }
            foreach (var a in hypernymDict)
            {
                hypernymDictionary[a.key] = a.value;
            }
            foreach (var a in hyponymDict)
            {
                hyponymDictionary[a.key] = a.value;
            }
            string def = "occupation";
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<string> resSyn1 = new List<string>();
            List<string> resHypernym1 = new List<string>();
            List<string> resHyponym1 = new List<string>();
            def = StopwordTool.RemoveStopwords(def);
            foreach (string s in def.Split(' '))
            {
                List<string> temp = null;
                List<string> temp2 = null;
                List<string> temp3 = null;
                if (synonymDictionary.ContainsKey(s.ToLowerInvariant()))
                {
                    temp = synonymDictionary[s.ToLowerInvariant()];
                    List<string> _temp = new List<string>();
                    foreach (var a in temp)
                    {
                        if (a.Contains("_"))
                        {
                            _temp.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp.Add(a);
                        }
                    }
                    temp = _temp.Distinct().ToList();
                }
                if (hyponymDictionary.ContainsKey(s.ToLowerInvariant()))
                {
                    temp2 = hyponymDictionary[s.ToLowerInvariant()];
                    List<string> _temp2 = new List<string>();
                    foreach (var a in temp2)
                    {
                        if (a.Contains("_"))
                        {
                            _temp2.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp2.Add(a);
                        }
                    }
                    temp2 = _temp2.Distinct().ToList();
                }
                if (hypernymDictionary.ContainsKey(s.ToLowerInvariant()))
                {
                    temp3 = hypernymDictionary[s.ToLowerInvariant()];
                    List<string> _temp3 = new List<string>();
                    foreach (var a in temp3)
                    {
                        if (a.Contains("_"))
                        {
                            _temp3.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp3.Add(a);
                        }
                    }
                    temp3 = _temp3.Distinct().ToList();
                }
                if (temp != null) resSyn1.AddRange(temp);
                if (temp2 != null) resHyponym1.AddRange(temp2);
                if (temp3 != null) resHypernym1.AddRange(temp3);
            }
            //List<string> article = compareTo.Split('.').ToList();
            List<string> article = searchContent;
            Dictionary<string, int> dict = new Dictionary<string, int>();
            int i = 0;
            foreach (string s in article)
            {
                Console.WriteLine($" Status = {++i}/{article.Count}");
                int a = dictionaryCompare(resSyn1, resHypernym1, resHyponym1, s, synonymDictionary, hypernymDictionary, hyponymDictionary);
                dict[s] = a;
            }
            var abc = dict.Where(x => x.Value > 0).OrderByDescending(x => x.Value).Take(9);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        private static void CreateBufferFiles()
        {
            WordNetUtil init = new WordNetUtil();
            List<string> indexSplitContent = new List<string>();
            string indexFileContent = File.ReadAllText(@"E:\wn3.1.dict.tar\wn3.1.dict\dict\index.noun");
            indexSplitContent = indexFileContent.Split('\n').ToList();
            indexSplitContent.RemoveAll(x => x.StartsWith("  "));
            List<string> mylist = new List<string>();
            int k = 0;
            //init.fetchData("affidavit");
            foreach (var a in indexSplitContent)
            {
                init.fetchData(a.Split(' ')[0]);
                Console.WriteLine(k++);
                //if (k == 2000) break;
            }
            List<DictionaryMapper> synlist = new List<DictionaryMapper>();
            List<DictionaryMapper> hypernymlist = new List<DictionaryMapper>();
            List<DictionaryMapper> hyponymlist = new List<DictionaryMapper>();
            foreach (var a in init.InstanceSynList)
            {
                DictionaryMapper abtestc = new DictionaryMapper(a.Key, a.Value);
                synlist.Add(abtestc);
            }
            foreach (var a in init.InstanceHypernymList)
            {
                DictionaryMapper abtestc = new DictionaryMapper(a.Key, a.Value);
                hypernymlist.Add(abtestc);
            }
            foreach(var a in init.InstanceHyponymList)
            {
                DictionaryMapper abtestc = new DictionaryMapper(a.Key, a.Value);
                hyponymlist.Add(abtestc);
            }
            SerializeObject<List<DictionaryMapper>>(synlist, "SynsetMappedList.data");
            SerializeObject<List<DictionaryMapper>>(hypernymlist, "HypernymMappedList.data");
            SerializeObject<List<DictionaryMapper>>(hyponymlist, "HyponymMappedList.data");
        }

        public static void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static T DeSerializeObject<T>(string fileName)
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
                throw ex;
            }

            return objectOut;
        }
        private static string calcRootWord(string derivedForm)
        {
            List<string> postfixList = new List<string>(new string[] { "al" });
            foreach (var a in postfixList)
            {
                if (derivedForm.EndsWith(a))
                {
                    return derivedForm.Substring(0, derivedForm.Length - a.Length);
                }
            }
            return derivedForm;
        }
        public static int dictionaryCompare(List<string> resSyn, List<string> resHypernym, List<string> resHyponym, string compareTo, Dictionary<string, List<string>> synDict, Dictionary<string, List<string>> hypernymDict, Dictionary<string, List<string>> hyponymDict)
        {
            if(compareTo.Contains("Douglas attended"))
            {
                var a = 22;
            }
            compareTo = StopwordTool.RemoveStopwords(compareTo);
            List<string> resSyn2 = new List<string>();
            List<string> resHyponym2 = new List<string>();
            List<string> resHypernym2 = new List<string>();
            foreach (string s in compareTo.Split(' '))
            {
                List<string> temp = null;
                List<string> temp2 = null;
                List<string> temp3 = null;
                if (synDict.ContainsKey(s.ToLowerInvariant()))
                {
                    temp = synDict[s.ToLowerInvariant()];
                    List<string> _temp = new List<string>();
                    foreach (var a in temp)
                    {
                        if (a.Contains("_"))
                        {
                            _temp.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp.Add(a);
                        }
                    }
                    temp = _temp.Distinct().ToList();
                }
                if (hyponymDict.ContainsKey(s.ToLowerInvariant()))
                {
                    temp2 = hyponymDict[s.ToLowerInvariant()];
                    List<string> _temp2 = new List<string>();
                    foreach (var a in temp2)
                    {
                        if (a.Contains("_"))
                        {
                            _temp2.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp2.Add(a);
                        }
                    }
                    temp2 = _temp2.Distinct().ToList();
                }
                if (hypernymDict.ContainsKey(s.ToLowerInvariant()))
                {
                    temp3 = hypernymDict[s.ToLowerInvariant()];
                    List<string> _temp3 = new List<string>();
                    foreach (var a in temp3)
                    {
                        if (a.Contains("_"))
                        {
                            _temp3.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp3.Add(a);
                        }
                    }
                    temp3 = _temp3.Distinct().ToList();
                }
                if (temp != null) resSyn2.AddRange(temp);
                if (temp2 != null) resHyponym2.AddRange(temp2);
                if (temp3 != null) resHypernym2.AddRange(temp3);
            }

            int retVal = (resSyn.Intersect(resSyn2).Count() * 20
                + resSyn.Intersect(resHyponym2).Count() * 5
                + resHyponym.Intersect(resSyn2).Count() * 5
                + resHyponym.Intersect(resHyponym2).Count() * 40
                + resSyn.Intersect(resHypernym2).Count() * 4
                + resHypernym.Intersect(resHypernym2).Count() * 1);
            //int retVal = resSyn.Intersect(resSyn2).Count() * 10 + resSyn.Intersect(resHypernym2).Count() * 5 + resHypernym.Intersect(resHypernym2).Count();

            return retVal;
        }
    }
}
