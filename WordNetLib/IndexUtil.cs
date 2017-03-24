using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordNetLib
{
    class IndexUtil
    {
        public string lemma, pos;
        public List<string> ptr_symbol, synset_offset;
        public int synset_cnt, p_cnt, sense_cnt, tagsense_cnt;
        public IndexUtil(string l)
        {
            string[] splitList = l.Split(' ');
            ptr_symbol = new List<string>();
            synset_offset = new List<string>();
            lemma = splitList[0];
            pos = splitList[1];
            synset_cnt = Convert.ToInt32(splitList[2]);
            p_cnt = Convert.ToInt32(splitList[3]);
            for (int i=0; i<p_cnt; i++)
            {
                ptr_symbol.Add(splitList[4 + i]);
            }
            sense_cnt = Convert.ToInt32(splitList[4+p_cnt]);
            tagsense_cnt = Convert.ToInt32(splitList[5 + p_cnt]);
            for (int i = 0; i < synset_cnt; i++)
            {
                synset_offset.Add(splitList[6 + p_cnt + i]);
            }
        }
    }
}
