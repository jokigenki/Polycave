using System.IO;
using UnityEngine;

public class Paths
{
    public static string Radicals = "radicals.json";
    public static string Sentences = "sentences.json";
    public static string KanjiToRadicals = "kanji_radicals.json";
    public static string LearningSets = "learning_sets.json";

    public static string GetRadicalsPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, Radicals);
    }

    public static string GetSentencesPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, Sentences);
    }

    public static string GetKanjiToRadicalsPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, KanjiToRadicals);
    }

    public static string GetLearningSetsPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, LearningSets);
    }
}