using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigramParsing.Models
{
    public class BigramDataViewModel
    {
        public Dictionary<string, int> BigramDictionary { get; set; }

        // returns organized list of values (bigram count)
        public StringBuilder ListBigramValues()
        {
            BigramDictionary.Values.ToArray();
            StringBuilder keys = new StringBuilder();
            foreach (var item in BigramDictionary)
            {
                keys.Append(item.Value + "\n\n\n");
            }
            return keys;
        }
        // returns organized list of keys, with a block visual representing values (bigram count)
        public StringBuilder ListBigramKeyAndVisual()
        {
            StringBuilder histoAndKeys = new StringBuilder();
    
            foreach (var item in BigramDictionary)
            {
                for (int x = 0; x < item.Value; x++)
                {
                    // rectangle block char
                    histoAndKeys.Append("\u2588");
                }
                histoAndKeys.Append("\n");
                histoAndKeys.Append(item.Key + "\n\n");
            }
            return histoAndKeys;
        }
    }
}
