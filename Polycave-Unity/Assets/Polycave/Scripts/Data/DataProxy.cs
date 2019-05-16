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
    List<Radical> radicals = new List<Radical> ();
    List<KanjiToRadical> kanjiToRadicals = new List<KanjiToRadical> ();
    Dictionary<string, LearningSet> learningSets = new Dictionary<string, LearningSet> ();

    public void Start ()
    {
        StartCoroutine (LoadData ());
    }

    public IEnumerator LoadData ()
    {
        yield return LoadRadicals ();
        yield return LoadKanjiToRadicals ();
        yield return LoadLearningSets ();
    }

    public IEnumerator LoadRadicals ()
    {
        string path = Paths.GetRadicalsPath ();
        yield return DataUtils.LoadJson<List<Radical>> (path, (List<Radical> list) =>
        {
            radicals = list;
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
}