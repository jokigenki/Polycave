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

        public bool ContainsKanjiOrReading (string kanji, string reading)
        {
            if (string.IsNullOrEmpty (kanji)) return ContainsReading (reading);
            return ContainsKanji (kanji);
        }

        private bool ContainsKanji (string kanji)
        {
            if (kanji == null) return false;
            List<Dictionary<string, string>> kanjiList = data?.First ().Value.kanji;
            bool hasKanjiList = kanjiList != null && kanjiList.Count > 0;
            if (!hasKanjiList) return false;
            List<string> compounds = kanjiList.SelectMany (k => k.Values).ToList ();
            return compounds.FirstOrDefault (c => c.Contains (kanji)) != null;
        }

        public bool HasKanji ()
        {
            List<Dictionary<string, string>> kanjiList = data?.First ().Value.kanji;
            return kanjiList != null && kanjiList.Count > 0;
        }

        private bool ContainsReading (string reading)
        {
            List<Dictionary<string, string>> readingList = data?.First ().Value.readings;
            bool hasReadingList = readingList != null && readingList.Count > 0;
            if (!hasReadingList) return false;
            List<string> compounds = readingList.SelectMany (k => k.Values).ToList ();
            return compounds.FirstOrDefault (c => c.Contains (reading)) != null;
        }

        public string FirstKanji ()
        {
            List<Dictionary<string, string>> kanjiList = data?.First ().Value.kanji;
            if (kanjiList == null || kanjiList.Count == 0) return null;
            return kanjiList[0]?.First ().Value;
        }

        public string FirstReading ()
        {
            List<Dictionary<string, string>> readingList = data.First ().Value.readings;
            if (readingList == null || readingList.Count == 0) return null;
            return readingList[0].First ().Value;
        }

        public string FirstSense ()
        {
            List<Dictionary<string, string>> senseList = data.First ().Value.senses;
            if (senseList == null || senseList.Count == 0) return null;
            return senseList[0].First ().Value;
        }

        override public string ToString ()
        {
            return $"kanji:{FirstKanji()} reading:{FirstReading()} sense:{FirstSense()}";
        }
    }
}