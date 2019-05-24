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
    public List<ExampleSentence> sentences = new List<ExampleSentence> ();
    public List<Conjugation> conjugations = new List<Conjugation> ();
    public List<KanjiToRadical> kanjiToRadicals = new List<KanjiToRadical> ();
    public Dictionary<string, LearningSet> learningSets = new Dictionary<string, LearningSet> ();

    public Action onDataLoaded;

    public LearningSet extendedSet;

    public string currentKanji;
    public LearningSetItem currentItem;
    public ExampleSentence currentSentence;

    public void Start ()
    {
        StartCoroutine (LoadData ());
    }

    public IEnumerator LoadData ()
    {
        yield return LoadRadicals ();
        yield return LoadSentences ();
        yield return LoadConjugations ();
        yield return LoadKanjiToRadicals ();
        yield return LoadLearningSets ();

        extendedSet = learningSets.Select (kv => kv.Value).Where (s => s.name == "extended").FirstOrDefault ();
        currentItem = GetItemForCompound ("家");

        if (onDataLoaded != null) onDataLoaded ();

        EventBus.Instance.Raise (new DataProxySelectionEvent (currentItem, GetKanjiForItem (currentItem), GetSentencesForItem (currentItem)));
    }

    public IEnumerator LoadRadicals ()
    {
        string path = Paths.GetRadicalsPath ();
        yield return DataUtils.LoadJson<List<Radical>> (path, (List<Radical> list) =>
        {
            radicals = list;
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

    public LearningSetItem GetItemForCompound (string compound)
    {
        return extendedSet.items.Select (kv => kv.Value).Where (i => i.data.Values.First ().kanji[0]["kanji"] == compound).FirstOrDefault ();
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

    public List<string> GetKanjiForItem (LearningSetItem item)
    {
        string kanji = item.FirstKanji ();
        return kanji.Select (k => $"{k}").ToList (); // TODO: we need to check whether the component is a kanji, or kana
    }

    public List<ExampleSentence> GetSentencesForItem (LearningSetItem item)
    {
        string kanji = item.FirstKanji ();
        // sentences where the nouns array contains the kanji, or the conjugations array contains a conjugated form of the kanji
        // jumping through a hoop backwards!
        return sentences.Where (s => s.nouns.Contains (kanji) ||
            s.conjugations.Where (c => GetDictionaryFormForConjugation (c) == kanji).Count () > 0).ToList ();
    }

    public List<LearningSetItem> GetItemsForSentence (ExampleSentence sentence)
    {
        List<string> kanji = sentence.nouns.Union (sentence.conjugations.Select (c => GetDictionaryFormForConjugation (c))).ToList ();
        return extendedSet.GetItemsInKanjiList (kanji);
    }
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
    public string kanji;
    public LearningSetItem item;
    public ExampleSentence sentence;

    public List<string> kanjiChoices;
    public List<LearningSetItem> itemChoices;
    public List<ExampleSentence> sentenceChoices;

    public DataProxySelectionEvent (string kanji, List<LearningSetItem> itemChoices)
    {
        this.kanji = kanji;
        this.itemChoices = itemChoices;
    }

    public DataProxySelectionEvent (LearningSetItem item, List<string> kanjiChoices, List<ExampleSentence> sentenceChoices)
    {
        this.item = item;
        this.kanjiChoices = kanjiChoices;
        this.sentenceChoices = sentenceChoices;
    }

    public DataProxySelectionEvent (ExampleSentence sentence, List<LearningSetItem> itemChoices)
    {
        this.sentence = sentence;
        this.itemChoices = itemChoices;
    }

    public bool HasUp
    {
        get
        {
            return sentence == null &&
                ((kanji != null && itemChoices.Count > 0) ||
                    (item != null && sentenceChoices.Count > 0));
        }
    }

    public bool HasDown
    {
        get
        {
            return kanji == null &&
                ((item != null && kanjiChoices.Count > 0) ||
                    (sentence != null && itemChoices.Count > 0));
        }
    }
}