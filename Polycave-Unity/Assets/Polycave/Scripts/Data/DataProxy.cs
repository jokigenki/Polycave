using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NekoUtils;
using Newtonsoft.Json;
using PolyblotPlayground;
using UnityEngine;

public class DataProxy : MonoBehaviour
{
    public List<Radical> radicals = new List<Radical> ();
    public List<Kanji> kanji = new List<Kanji> ();
    public List<ExampleSentence> sentences = new List<ExampleSentence> ();
    public List<Conjugation> conjugations = new List<Conjugation> ();
    public List<KanjiToRadical> kanjiToRadicals = new List<KanjiToRadical> ();
    public Dictionary<string, LearningSet> learningSets = new Dictionary<string, LearningSet> ();

    public List<string> kanjiList = new List<string> ();
    public string kanaList = "";
    public List<string> verbList = new List<string> ();
    public LearningSet extendedSet;

    private LearningSetItem _currentItem;
    private Kanji _currentKanji;
    private ExampleSentence _currentSentence;

    public void Start ()
    {
        StartCoroutine (LoadData ());
    }

    public IEnumerator LoadData ()
    {
        yield return LoadRadicals ();
        yield return LoadSentences ();
        yield return LoadKanji ();
        yield return LoadKanjiList ();
        yield return LoadKanaList ();
        yield return LoadVerbList ();
        yield return LoadConjugations ();
        yield return LoadKanjiToRadicals ();
        yield return LoadLearningSets ();

        extendedSet = learningSets.Select (kv => kv.Value).Where (s => s.name == "extended").FirstOrDefault ();

        EventBus.Instance.Raise (new DataProxyEvent (DataProxyEventType.Ready));
        EventBus.Instance.AddListener<DataProxyEvent> (DisplayFirst);

        OVRManager.HMDUnmounted += HandleHMDUnmounted;
    }

    void HandleHMDUnmounted ()
    {
        EventBus.Instance.Raise (new DataProxyEvent (DataProxyEventType.Reset));
    }

    public void DisplayFirst (DataProxyEvent e)
    {
        if (e.type != DataProxyEventType.Start) return;
        LearningSetItem startItem = extendedSet.GetItemForCompound ("一");
        SetCurrentData (startItem);
    }

    public void SetCurrentData (System.Object data)
    {
        _currentItem = null;
        _currentKanji = null;
        _currentSentence = null;
        if (data is LearningSetItem)
        {
            _currentItem = data as LearningSetItem;
            EventBus.Instance.Raise (new DataProxySelectionEvent (_currentItem, NavType.Display, GetKanjiForItem (_currentItem), GetSentencesForItem (_currentItem)));
        }
        else if (data is ExampleSentence)
        {
            _currentSentence = data as ExampleSentence;
            EventBus.Instance.Raise (new DataProxySelectionEvent (_currentSentence, NavType.Display, GetItemsForSentence (_currentSentence)));
        }
        else if (data is Kanji)
        {
            _currentKanji = data as Kanji;
            EventBus.Instance.Raise (new DataProxySelectionEvent (_currentKanji, NavType.Display, extendedSet.GetItemsForKanji (_currentKanji)));
        }
    }

    public IEnumerator LoadRadicals ()
    {
        string path = Paths.GetRadicalsPath ();
        yield return DataUtils.LoadJson<List<Radical>> (path, (List<Radical> list) =>
        {
            radicals = list;
        });
    }

    public IEnumerator LoadKanji ()
    {
        string path = Paths.GetKanjiPath ();
        yield return DataUtils.LoadJson<List<Kanji>> (path, (List<Kanji> list) =>
        {
            kanji = list;
        });
    }

    public IEnumerator LoadKanjiList ()
    {
        string path = Paths.GetKanjiListPath ();
        yield return DataUtils.FileToString (path, (string str) =>
        {
            kanjiList = str.Select (s => $"{s}").ToList ();
        });
    }

    public IEnumerator LoadKanaList ()
    {
        string path = Paths.GetKanaListPath ();
        yield return DataUtils.FileToString (path, (string str) =>
        {
            kanaList = str;
        });
    }

    public IEnumerator LoadVerbList ()
    {
        string path = Paths.GetVerbListPath ();
        yield return DataUtils.FileToString (path, (string str) =>
        {
            verbList = str.Split (',').ToList ();
        });
    }

    public IEnumerator LoadSentences ()
    {
        string path = Paths.GetSentencesPath ();
        yield return DataUtils.LoadJson<List<ExampleSentence>> (path, (List<ExampleSentence> list) =>
        {
            sentences = list;
        });
    }

    public IEnumerator LoadConjugations ()
    {
        string path = Paths.GetConjugationsPath ();
        yield return DataUtils.LoadJson<List<Conjugation>> (path, (List<Conjugation> list) =>
        {
            conjugations = list;
        });
    }

    public IEnumerator LoadKanjiToRadicals ()
    {
        string path = Paths.GetKanjiToRadicalsPath ();
        yield return DataUtils.LoadJson<List<KanjiToRadical>> (path, (List<KanjiToRadical> list) =>
        {
            kanjiToRadicals = list;
        });
    }

    public IEnumerator LoadLearningSets ()
    {
        string path = Paths.GetLearningSetsPath ();
        yield return DataUtils.LoadJson<Dictionary<string, LearningSet>> (path, (Dictionary<string, LearningSet> sets) => { learningSets = sets; });
    }

    public Radical GetRadicalByValue (string value)
    {
        return radicals.Where (r => r.radical == value).FirstOrDefault ();
    }

    public List<string> GetRadicalsForKanji (string kanji)
    {
        return kanjiToRadicals.Where (k => k.kanji == kanji).FirstOrDefault ()?.radicals;
    }

    public ExampleSentence GetSentenceForNoun (string noun)
    {
        return sentences.Where (s => s.nouns.Contains (noun)).FirstOrDefault ();
    }

    public List<string> GetConjugationsForDictionaryForm (string dictionaryForm)
    {
        return conjugations.Where (c => c.dic == dictionaryForm).FirstOrDefault ()?.conj;
    }

    public string GetDictionaryFormForConjugation (string conjugation)
    {
        return conjugations.Where (c => c.conj.Contains (conjugation)).FirstOrDefault ()?.dic;
    }

    public List<Kanji> GetKanjiForItem (LearningSetItem item)
    {
        List<Kanji> kanjiList = new List<Kanji> ();
        string firstKanji = item.FirstKanji ();
        if (firstKanji == null) return kanjiList; // if the item is kana only, this will trigger
        List<string> itemKanji = firstKanji.Select (k => $"{k}").ToList ();

        foreach (string iKanji in itemKanji)
        {
            Kanji foundKanji = kanji.Where (k => k.kanji == iKanji).FirstOrDefault ();
            if (foundKanji != null) kanjiList.Add (foundKanji);
        }

        return kanjiList;
    }

    public List<ExampleSentence> GetSentencesForItem (LearningSetItem item)
    {
        string kanji = item.FirstKanji ();
        if (kanji == null) kanji = item.FirstReading (); // in case of kana only, e.g. suru, coffee
        // sentences where the nouns array contains the kanji, or the conjugations array contains a conjugated form of the kanji
        // jumping through a hoop backwards!
        List<ExampleSentence> examples = sentences.Where (s => s.nouns.Contains (kanji) ||
            s.conjugations.Where (c => GetDictionaryFormForConjugation (c) == kanji).Count () > 0).ToList ();

        if (examples.Count > 5) examples = Randomer.FromList (examples, 5);
        return examples;
    }

    public List<LearningSetItem> GetItemsForSentence (ExampleSentence sentence)
    {
        List<string> kanji = sentence.nouns.Union (sentence.conjugations.Select (c => GetDictionaryFormForConjugation (c))).ToList ();
        List<LearningSetItem> items = extendedSet.GetItemsInKanjiList (kanji);

        if (items.Count > 5)
        {
            items = Randomer.FromList (items, 5);
        }
        return items;
    }

    public bool IsKanaOnly (string value)
    {
        foreach (char v in value)
            if (!kanaList.Contains (v)) return false;
        return true;
    }
}

public class DataProxyEvent : GameEvent
{
    public DataProxyEventType type;

    public DataProxyEvent (DataProxyEventType type)
    {
        this.type = type;
    }
}

public enum DataProxyEventType
{
    Ready,
    Start,
    Reset
}

public class DataProxyChoicesEvent : GameEvent
{

    public RuneType runeType;
    public DataProxySelectionEvent currentEvent;

    public DataProxyChoicesEvent (RuneType runeType, DataProxySelectionEvent currentEvent)
    {
        this.runeType = runeType;
        this.currentEvent = currentEvent;
    }
}

public class DataProxySelectionEvent : GameEvent
{
    public NavType navType;
    public Kanji kanji;
    public LearningSetItem compound;
    public ExampleSentence sentence;

    public List<Kanji> kanjiChoices;
    public List<LearningSetItem> compoundChoices;
    public List<ExampleSentence> sentenceChoices;

    public DataProxySelectionEvent (Kanji kanji, NavType navType, List<LearningSetItem> compoundChoices)
    {
        this.navType = navType;
        this.kanji = kanji;
        this.compoundChoices = compoundChoices;
    }

    public DataProxySelectionEvent (LearningSetItem compound, NavType navType, List<Kanji> kanjiChoices, List<ExampleSentence> sentenceChoices)
    {
        this.navType = navType;
        this.compound = compound;
        this.kanjiChoices = kanjiChoices;
        this.sentenceChoices = sentenceChoices;
    }

    public DataProxySelectionEvent (ExampleSentence sentence, NavType navType, List<LearningSetItem> itemChoices)
    {
        this.navType = navType;
        this.sentence = sentence;
        this.compoundChoices = itemChoices;
    }

    public DataProxySelectionEvent (DataProxyChoicesEvent e, NavType navType)
    {
        this.navType = navType;
        this.sentence = e.currentEvent.sentence;
        this.sentenceChoices = e.currentEvent.sentenceChoices;
        this.compound = e.currentEvent.compound;
        this.compoundChoices = e.currentEvent.compoundChoices;
        this.kanji = e.currentEvent.kanji;
        this.kanjiChoices = e.currentEvent.kanjiChoices;
    }

    public bool HasUp
    {
        get
        {
            return sentence == null &&
                ((kanji != null && compoundChoices.Count > 0) ||
                    (compound != null && sentenceChoices.Count > 0));
        }
    }

    public bool HasDown
    {
        get
        {
            return kanji == null &&
                ((compound != null && kanjiChoices.Count > 0) ||
                    (sentence != null && compoundChoices.Count > 0));
        }
    }
}