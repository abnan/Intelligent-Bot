using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WordNetLib
{
    public class WordNetUtil
    {
        private string rootWord;
        private List<string> indexSplitContent, dataSplitContent;
        private Dictionary<string, List<string>> instanceSynList, instanceHypernymList, instanceHyponymList;
        public Dictionary<string, List<string>> InstanceSynList
        {
            get
            {
                return instanceSynList;
            }
        }
        public Dictionary<string, List<string>> InstanceHypernymList
        {
            get
            {
                return instanceHypernymList;
            }
        }
        public Dictionary<string, List<string>> InstanceHyponymList
        {
            get
            {
                return instanceHyponymList;
            }
        }
        public WordNetUtil()
        {
            instanceSynList = new Dictionary<string, List<string>>();
            instanceHypernymList = new Dictionary<string, List<string>>();
            instanceHyponymList = new Dictionary<string, List<string>>();
            string indexFileContent = File.ReadAllText(@"E:\wn3.1.dict.tar\wn3.1.dict\dict\index.noun");
            indexSplitContent = indexFileContent.Split('\n').ToList();
            indexSplitContent.RemoveAll(x => x.StartsWith("  "));

            string dataFileContent = File.ReadAllText(@"E:\wn3.1.dict.tar\wn3.1.dict\dict\data.noun");
            dataSplitContent = dataFileContent.Split('\n').ToList();
            dataSplitContent.RemoveAll(x => x.StartsWith("  "));
        }
        public void fetchData(string w)
        {
            if (instanceSynList.ContainsKey(w))
            {
                return;
            }
            rootWord = w;
            string indexFindLine = indexSplitContent.Find(x => x.StartsWith(rootWord) && x.Split(' ')[0].Equals(rootWord, StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrEmpty(indexFindLine))
            {
                instanceHypernymList[w] = null;
                instanceSynList[w] = null;
                return;
            }
            IndexUtil line = new IndexUtil(indexFindLine);

            List<string> dataFindLine = new List<string>();
            foreach (string offsets in line.synset_offset)
            {
                dataFindLine.Add(dataSplitContent.Find(x => x.StartsWith(offsets) && x.Split(' ')[0].Equals(offsets, StringComparison.InvariantCultureIgnoreCase)));
            }
            List<string> hypernymIndexes = new List<string>();
            List<string> synonyms = new List<string>();
            List<string> hyponymIndexes = new List<string>();
            foreach (string offsets in dataFindLine)
            {
                DataUtil offsetLine = new DataUtil(offsets);
                foreach (string words in offsetLine.hypernymIndexes)
                {
                    hypernymIndexes.Add(words);
                }
                foreach (string words in offsetLine.sysnset)
                {
                    synonyms.Add(words);
                }
                foreach(string words in offsetLine.hyponymIndexes)
                {
                    hyponymIndexes.Add(words);
                }
            }
            List<string> hypernyms = new List<string>();
            foreach (string index in hypernymIndexes)
            {
                hypernyms.Add(dataSplitContent.Find(x => x.StartsWith(index) && x.Split(' ')[0].Equals(index, StringComparison.InvariantCultureIgnoreCase)));
            }
            List<string> finalHypernymList = new List<string>();
            foreach (string offsets in hypernyms)
            {
                DataUtil offsetLine = new DataUtil(offsets);
                foreach (string words in offsetLine.sysnset)
                {
                    finalHypernymList.Add(words);
                }
            }
            List<string> hyponyms = new List<string>();
            foreach(string index in hyponymIndexes)
            {
                hyponyms.Add(dataSplitContent.Find(x => x.StartsWith(index) && x.Split(' ')[0].Equals(index, StringComparison.InvariantCultureIgnoreCase)));
            }
            List<string> finalHyponymList = new List<string>();
            foreach(string offsets in hyponyms)
            {
                DataUtil offsetLine = new DataUtil(offsets);
                foreach (string words in offsetLine.sysnset)
                {
                    finalHyponymList.Add(words);
                }
            }
            finalHyponymList = finalHyponymList.Distinct().ToList();
            instanceHyponymList[w] = finalHyponymList;
            finalHypernymList = finalHypernymList.Distinct().ToList();
            instanceHypernymList[w] = finalHypernymList;
            synonyms = synonyms.Distinct().ToList();
            instanceSynList[w] = synonyms;
        }
    }
}
