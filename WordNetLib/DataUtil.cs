using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordNetLib
{
    class DataUtil
    {
        public List<string> sysnset, antonymIndexes, hyponymIndexes, hypernymIndexes;
        public DataUtil(string l)
        {
            sysnset = new List<string>();
            antonymIndexes = new List<string>();
            hyponymIndexes = new List<string>();
            hypernymIndexes = new List<string>();
            string[] splitList = l.Split(' ');
            string temp = splitList[3];
            int count = Convert.ToInt32(splitList[3], 16);
            for (int i=0; i<count*2; i+=2)
            {
                sysnset.Add(splitList[4+i]);
            }
            int moveToPos = 4 + count * 2 + 1;
            for(int i= moveToPos; i <splitList.Count()-2;i++)
            {
                if (string.Equals(splitList[i], "n") && string.Equals(splitList[i+1], "0000"))
                {
                    if(string.Equals(splitList[i+2], "@") || string.Equals(splitList[i + 2], "@i"))
                    {
                        hypernymIndexes.Add(splitList[i + 3]);
                    }
                    else if (string.Equals(splitList[i + 2], "~") || string.Equals(splitList[i + 2], "~i"))
                    {
                        hyponymIndexes.Add(splitList[i + 3]);
                    }
                }
                else if (splitList[i].Equals("@") || splitList[i].Equals("@i"))
                {
                    if(splitList[i+1].Length == 8)    hypernymIndexes.Add(splitList[i + 1]);
                }
                else if (splitList[i].Equals("~") || splitList[i].Equals("~i"))
                {
                    if (splitList[i+1].Length == 8)   hyponymIndexes.Add(splitList[i + 1]);
                }
            }
        }
    }
}
