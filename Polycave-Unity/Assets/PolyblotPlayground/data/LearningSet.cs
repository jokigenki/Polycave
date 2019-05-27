using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PolyblotPlayground
{
    public class LearningSet
    {
        public string name;
        public Dictionary<string, LearningSetItem> items;

        public List<LearningSetItem> GetItemsForKanji (Kanji kanji)
        {
            Debug.Log ($"Finding items for kanji: {kanji.kanji}");
            List<LearningSetItem> itemsForKanji = items.Values.Where (i => i.ContainsKanji (kanji.kanji)).ToList ();
            Debug.Log ($"items {itemsForKanji.Count}");
            return itemsForKanji;
        }

        public List<LearningSetItem> GetItemsInKanjiList (List<string> kanji)
        {
            return items.Values.Where (i => kanji.Contains (i.FirstKanji ())).ToList ();
        }
    }
}