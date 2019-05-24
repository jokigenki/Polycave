using System.Collections.Generic;
using System.Linq;

namespace PolyblotPlayground
{
    public class LearningSet
    {
        public string name;
        public Dictionary<string, LearningSetItem> items;

        public List<LearningSetItem> GetItemsForKanji (string kanji)
        {
            return items.Values.Where (i => i.FirstKanji () == kanji).ToList ();
        }

        public List<LearningSetItem> GetItemsInKanjiList (List<string> kanji)
        {
            return items.Values.Where (i => kanji.Contains (i.FirstKanji ())).ToList ();
        }
    }
}