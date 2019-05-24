using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NekoUtils;
using UnityEngine;

public class DataChecker : MonoBehaviour
{
    DataProxy proxy;
    void Start ()
    {
#if UNITY_EDITOR
        proxy = FindObjectOfType<DataProxy> ();
        proxy.onDataLoaded += OnDataLoaded;
#endif
    }

    public void OnDataLoaded ()
    {
        string errors = TestConjugations ();
        errors += TestData ();

        if (errors != "")
            Debug.Log (errors);
        else
            Debug.Log ("No errors found in data!");
    }

    private string TestConjugations ()
    {
        string errors = "";
        List<string> conjugationsS = proxy.sentences.SelectMany (s => s.conjugations).Distinct ().ToList ();
        List<string> conjugations = proxy.conjugations.SelectMany (s => s.conj).Distinct ().ToList ();

        string[] missing = conjugationsS.Except (conjugations).ToArray ();
        if (missing.Length > 0)
            errors += String.Join (", ", missing) + " were missing. ";

        var listBefore = proxy.conjugations.Select (c => c.dic);
        int lb = listBefore.Count ();
        var listAfter = listBefore.Distinct ();
        int la = listAfter.Count ();

        var duplicates = proxy.conjugations.Select (c => c.dic).GroupBy (x => x)
            .Where (g => g.Count () > 1)
            .Select (y => y.Key)
            .ToArray ();
        if (duplicates.Length > 0)
            errors += String.Join (", ", duplicates) + " were duplicated. ";
        return errors;
    }

    private List<string> GetAllNounsFromSentenceData ()
    {
        return proxy.sentences.SelectMany (s => s.nouns).ToList ();
    }

    private List<string> GetDictionaryFormsFromConjugations ()
    {
        return proxy.conjugations.Select (c => c.dic).ToList ();
    }

    private string TestData ()
    {
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
                return " Wrote extended file, rerun to test data.";
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

            if (missing.Length == 0)
                return "";
            return $" {string.Join (",", missing)} were missing from data";
        }
    }
}