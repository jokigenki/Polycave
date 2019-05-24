using System.Collections.Generic;
using System.Linq;

namespace PolyblotPlayground
{
    public class LearningSetItem
    {
        public Dictionary<string, Source> data;
        public List<string> sources;
        public List<string> tags;

        public string FirstKanji ()
        {
            return data.First ().Value.kanji[0].First ().Value;
        }
    }
}