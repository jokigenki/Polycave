using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NekoUtils;
using PolyblotPlayground;
using UnityEngine;

public class DataChecker : MonoBehaviour
{
    public DataProxy proxy;
    void Start ()
    {
#if UNITY_EDITOR
        proxy.onDataLoaded += OnDataLoaded;
#endif
    }

    public void OnDataLoaded ()
    {
        bool passed = (TestConjugations () &&
            //            TestNounsAndConjugationsAreInPBPLookupFile () &&
            TestNounsAndConjugationsArePresentInLearningSets () &&
            TestLearningSets ());
    }

    private bool TestConjugations ()
    {
        string errors = "";
        List<string> conjugationsS = proxy.sentences.SelectMany (s => s.conjugations).Distinct ().ToList ();
        List<string> conjugations = proxy.conjugations.SelectMany (s => s.conj).Distinct ().ToList ();

        string[] missing = conjugationsS.Except (conjugations).ToArray ();
        if (missing.Length > 0)
            errors += String.Join (", ", missing) + " were missing from conjugations. ";

        var listBefore = proxy.conjugations.Select (c => c.dic);
        int lb = listBefore.Count ();
        var listAfter = listBefore.Distinct ();
        int la = listAfter.Count ();

        var duplicates = proxy.conjugations.Select (c => c.dic).GroupBy (x => x)
            .Where (g => g.Count () > 1)
            .Select (y => y.Key)
            .ToArray ();
        if (duplicates.Length > 0)
            errors += String.Join (", ", duplicates) + " were duplicated in conjugations.";

        if (errors == "")
        {
            Debug.Log ("No errors in conjugations");
            return true;
        }

        Debug.Log (errors);
        return false;
    }

    private List<string> GetAllNounsFromSentenceData ()
    {
        return proxy.sentences.SelectMany (s => s.nouns).ToList ();
    }

    private List<string> GetDictionaryFormsFromConjugations ()
    {
        return proxy.conjugations.Select (c => c.dic).ToList ();
    }

    private bool TestNounsAndConjugationsAreInPBPLookupFile ()
    {
        string errors = "";
        List<string> nouns = GetAllNounsFromSentenceData ().Distinct ().ToList ();
        nouns.Sort ();
        List<string> conjs = GetDictionaryFormsFromConjugations ().Distinct ().ToList ();
        conjs.Sort ();
        List<string> allItems = nouns.Union (conjs).ToList ();
        string allItemsStr = String.Join (",", allItems.ToArray ());
        List<string> extended = new List<string> ();
        string path = Path.Combine (Application.streamingAssetsPath, "extended_data.txt");

        if (!File.Exists (path))
        {
            using (StreamWriter sw = new StreamWriter (path))
            {
                sw.Write (allItemsStr);
                errors = "Wrote extended pbp lookup file, rerun to test data.";
            }
        }
        else
        {
            using (StreamReader sr = new StreamReader (path))
            {
                string ex = sr.ReadToEnd ();
                extended = ex.Split (',').ToList ();
            }

            string[] missing = allItems.Except (extended).ToArray ();

            if (missing.Length == 0) errors = "";
            else errors = $"{string.Join (",", missing)} were missing from data. ";
        }

        if (errors == "")
        {
            Debug.Log ("No errors in pbp lookup file");
            return true;
        }

        Debug.Log (errors);
        return false;
    }

    private bool TestNounsAndConjugationsArePresentInLearningSets ()
    {
        string errors = "";
        List<string> nouns = GetAllNounsFromSentenceData ().Distinct ().ToList ();
        nouns.Sort ();
        List<string> conjs = GetDictionaryFormsFromConjugations ().Distinct ().ToList ();
        conjs.Sort ();

        List<string> allItems = nouns.Union (conjs).ToList ();

        List<string> missingItems = new List<string> ();
        foreach (string item in allItems)
        {
            LearningSetItem lsItem = proxy.extendedSet.GetItemForCompound (item);
            if (item == null) missingItems.Add (item);
        }

        if (missingItems.Count > 0)
        {
            errors = $"LearningSet was missing items for {string.Join(", ", missingItems)}";
        }

        if (errors == "")
        {
            Debug.Log ("All nouns and conjugations have learning items");
            return true;
        }

        Debug.Log (errors);
        return false;
    }

    private bool TestLearningSets ()
    {
        string errors = "";

        List<LearningSetItem> items = proxy.extendedSet.items.Select (i => i.Value).ToList ();
        foreach (LearningSetItem item in items)
        {
            try
            {
                string firstKanji = item.FirstKanji ();
                string firstReading = item.FirstReading ();
                string firstSense = item.FirstSense ();

                if (firstKanji != null)
                {
                    if (firstReading == null) errors += $"{item.sources[0]} has kanji but no reading. ";
                }
                else
                {
                    if (firstReading == null) errors += $"{item.sources[0]} has no kanji or reading. ";
                }

                if (firstSense == null) errors += $"{item.sources[0]} has no sense. ";
            }
            catch (Exception e)
            {
                errors += $"Error {e.Message} with {item.sources[0]}. ";
            }
        }

        if (errors == "")
        {
            Debug.Log ("No errors in learning sets");
            return true;
        }

        Debug.Log (errors);
        return false;
    }
}