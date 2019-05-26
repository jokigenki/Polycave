using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PolyblotPlayground
{
    public class LearningSetItem
    {
        public Dictionary<string, Source> data;
        public List<string> sources;
        public List<string> tags;

        public string FirstKanji ()
        {
            List<Dictionary<string, string>> kanjiList = data?.First ().Value.kanji;
            if (kanjiList == null || kanjiList.Count == 0) return null;
            return kanjiList[0]?.First ().Value;
        }

        public string FirstReading ()
        {
            List<Dictionary<string, string>> readingList = data.First ().Value.reading;
            if (readingList.Count == 0) return null;
            return readingList[0].First ().Value;
        }
    }
}