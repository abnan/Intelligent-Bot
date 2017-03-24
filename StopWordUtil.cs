﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deploybot
{
    static class StopwordTool
    {
        /// <summary>
        /// Words we want to remove.
        /// </summary>
        static Dictionary<string, bool> _stops = new Dictionary<string, bool>
        {
            { "a", true },
            { "about", true },
            { "above", true },
            { "across", true },
            { "after", true },
            { "afterwards", true },
            { "again", true },
            { "against", true },
            { "all", true },
            { "almost", true },
            { "alone", true },
            { "along", true },
            { "already", true },
            { "also", true },
            { "although", true },
            { "always", true },
            { "am", true },
            { "among", true },
            { "amongst", true },
            { "amount", true },
            { "an", true },
            { "and", true },
            { "another", true },
            { "any", true },
            { "anyhow", true },
            { "anyone", true },
            { "anything", true },
            { "anyway", true },
            { "anywhere", true },
            { "are", true },
            { "around", true },
            { "as", true },
            { "at", true },
            { "back", true },
            { "be", true },
            { "became", true },
            { "because", true },
            { "become", true },
            { "becomes", true },
            { "becoming", true },
            { "been", true },
            { "before", true },
            { "beforehand", true },
            { "behind", true },
            { "being", true },
            { "below", true },
            { "beside", true },
            { "besides", true },
            { "between", true },
            { "beyond", true },
            { "bill", true },
            { "both", true },
            { "bottom", true },
            { "but", true },
            { "by", true },
            { "call", true },
            { "can", true },
            { "cannot", true },
            { "cant", true },
            { "co", true },
            { "computer", true },
            { "con", true },
            { "could", true },
            { "couldnt", true },
            { "cry", true },
            { "de", true },
            { "describe", true },
            { "detail", true },
            { "do", true },
            { "done", true },
            { "down", true },
            { "due", true },
            { "during", true },
            { "each", true },
            { "eg", true },
            { "eight", true },
            { "either", true },
            { "eleven", true },
            { "else", true },
            { "elsewhere", true },
            { "empty", true },
            { "enough", true },
            { "etc", true },
            { "even", true },
            { "ever", true },
            { "every", true },
            { "everyone", true },
            { "everything", true },
            { "everywhere", true },
            { "except", true },
            { "few", true },
            { "fifteen", true },
            { "fify", true },
            { "fill", true },
            { "find", true },
            { "fire", true },
            { "first", true },
            { "five", true },
            { "for", true },
            { "former", true },
            { "formerly", true },
            { "forty", true },
            { "found", true },
            { "four", true },
            { "from", true },
            { "front", true },
            { "full", true },
            { "further", true },
            { "get", true },
            { "give", true },
            { "go", true },
            { "had", true },
            { "has", true },
            { "have", true },
            { "he", true },
            { "hence", true },
            { "her", true },
            { "here", true },
            { "hereafter", true },
            { "hereby", true },
            { "herein", true },
            { "hereupon", true },
            { "hers", true },
            { "herself", true },
            { "him", true },
            { "himself", true },
            { "his", true },
            { "how", true },
            { "however", true },
            { "hundred", true },
            { "i", true },
            { "ie", true },
            { "if", true },
            { "in", true },
            { "inc", true },
            { "indeed", true },
            { "interest", true },
            { "into", true },
            { "is", true },
            { "it", true },
            { "its", true },
            { "itself", true },
            { "keep", true },
            { "last", true },
            { "latter", true },
            { "latterly", true },
            { "least", true },
            { "less", true },
            { "ltd", true },
            { "made", true },
            { "many", true },
            { "may", true },
            { "me", true },
            { "meanwhile", true },
            { "might", true },
            { "mill", true },
            { "mine", true },
            { "more", true },
            { "moreover", true },
            { "most", true },
            { "mostly", true },
            { "move", true },
            { "much", true },
            { "must", true },
            { "my", true },
            { "myself", true },
            { "name", true },
            { "namely", true },
            { "neither", true },
            { "never", true },
            { "nevertheless", true },
            { "next", true },
            { "nine", true },
            { "no", true },
            { "nobody", true },
            { "none", true },
            { "nor", true },
            { "not", true },
            { "nothing", true },
            { "now", true },
            { "nowhere", true },
            { "of", true },
            { "off", true },
            { "often", true },
            { "on", true },
            { "once", true },
            { "one", true },
            { "only", true },
            { "onto", true },
            { "or", true },
            { "other", true },
            { "others", true },
            { "otherwise", true },
            { "our", true },
            { "ours", true },
            { "ourselves", true },
            { "out", true },
            { "over", true },
            { "own", true },
            { "part", true },
            { "per", true },
            { "perhaps", true },
            { "please", true },
            { "put", true },
            { "rather", true },
            { "re", true },
            { "same", true },
            { "see", true },
            { "seem", true },
            { "seemed", true },
            { "seeming", true },
            { "seems", true },
            { "serious", true },
            { "several", true },
            { "she", true },
            { "should", true },
            { "show", true },
            { "side", true },
            { "since", true },
            { "sincere", true },
            { "six", true },
            { "sixty", true },
            { "so", true },
            { "some", true },
            { "somehow", true },
            { "someone", true },
            { "something", true },
            { "sometime", true },
            { "sometimes", true },
            { "somewhere", true },
            { "still", true },
            { "such", true },
            { "system", true },
            { "take", true },
            { "ten", true },
            { "than", true },
            { "that", true },
            { "the", true },
            { "their", true },
            { "them", true },
            { "themselves", true },
            { "then", true },
            { "thence", true },
            { "there", true },
            { "thereafter", true },
            { "thereby", true },
            { "therefore", true },
            { "therein", true },
            { "thereupon", true },
            { "these", true },
            { "they", true },
            { "thick", true },
            { "thin", true },
            { "third", true },
            { "this", true },
            { "those", true },
            { "though", true },
            { "three", true },
            { "through", true },
            { "throughout", true },
            { "thru", true },
            { "thus", true },
            { "to", true },
            { "together", true },
            { "too", true },
            { "top", true },
            { "toward", true },
            { "towards", true },
            { "twelve", true },
            { "twenty", true },
            { "two", true },
            { "un", true },
            { "under", true },
            { "until", true },
            { "up", true },
            { "upon", true },
            { "us", true },
            { "very", true },
            { "via", true },
            { "was", true },
            { "we", true },
            { "well", true },
            { "were", true },
            { "what", true },
            { "whatever", true },
            { "when", true },
            { "whence", true },
            { "whenever", true },
            { "where", true },
            { "whereafter", true },
            { "whereas", true },
            { "whereby", true },
            { "wherein", true },
            { "whereupon", true },
            { "wherever", true },
            { "whether", true },
            { "which", true },
            { "while", true },
            { "whither", true },
            { "who", true },
            { "whoever", true },
            { "whole", true },
            { "whom", true },
            { "whose", true },
            { "why", true },
            { "will", true },
            { "with", true },
            { "within", true },
            { "without", true },
            { "would", true },
            { "yet", true },
            { "you", true },
            { "your", true },
            { "yours", true },
            { "yourself", true },
            { "yourselves", true }
        };

        /// <summary>
        /// Chars that separate words.
        /// </summary>
        static char[] _delimiters = new char[]
        {
        ' ',
        ',',
        ';',
        '.'
        };

        /// <summary>
        /// Remove stopwords from string.
        /// </summary>
        public static string RemoveStopwords(string input)
        {
            // 1
            // Split parameter into words
            var words = input.Split(_delimiters,
                StringSplitOptions.RemoveEmptyEntries);
            // 2
            // Allocate new dictionary to store found words
            var found = new Dictionary<string, bool>();
            // 3
            // Store results in this StringBuilder
            StringBuilder builder = new StringBuilder();
            // 4
            // Loop through all words
            foreach (string currentWord in words)
            {
                // 5
                // Convert to lowercase
                string lowerWord = currentWord.ToLower();
                // 6
                // If this is a usable word, add it
                if (!_stops.ContainsKey(lowerWord) &&
                    !found.ContainsKey(lowerWord))
                {
                    builder.Append(currentWord).Append(' ');
                    found.Add(lowerWord, true);
                }
            }
            // 7
            // Return string with words removed
            return builder.ToString().Trim();
        }
    }
}
