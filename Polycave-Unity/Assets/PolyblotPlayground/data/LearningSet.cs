using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PolyblotPlayground
{
    public class LearningSet
    {
        public string name;
        public Dictionary<string, LearningSetItem> items;

        public LearningSetItem GetItemForCompound (string compound)
        {
            return items.Values.Where (i => i.HasKanji () ? i.ContainsKanjiOrReading (compound, compound, true) : i.ContainsKanjiOrReading (null, compound, true)).FirstOrDefault ();
        }

        public List<LearningSetItem> GetItemsForKanji (Kanji kanji)
        {
            List<LearningSetItem> itemsForKanji = items.Values.Where (i => i.ContainsKanjiOrReading (kanji.kanji, kanji.reading, false)).ToList ();
            if (itemsForKanji.Count > 5)
            {
                itemsForKanji = Randomer.FromList (itemsForKanji, 5);
            }
            return itemsForKanji;
        }

        public List<LearningSetItem> GetItemsInKanjiList (List<string> kanji)
        {
            List<LearningSetItem> allItems = new List<LearningSetItem> ();
            return kanji.Select (k => GetItemForCompound (k)).ToList ();
        }
    }
}