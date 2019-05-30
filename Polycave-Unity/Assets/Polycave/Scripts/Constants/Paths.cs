using System.IO;
using UnityEngine;

public class Paths
{
    public static string Radicals = "radicals.json";
    public static string Kanji = "kanji.json";
    public static string Sentences = "sentences.json";
    public static string Conjugations = "conjugations.json";
    public static string KanjiToRadicals = "kanji_radicals.json";
    public static string LearningSets = "learning_sets.json";
    public static string KanjiList = "kanjilist.txt";
    public static string KanaList = "kanalist.txt";
    public static string VerbList = "verblist.txt";

    public static string GetRadicalsPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, Radicals);
    }

    public static string GetKanjiPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, Kanji);
    }

    public static string GetSentencesPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, Sentences);
    }

    public static string GetConjugationsPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, Conjugations);
    }

    public static string GetKanjiToRadicalsPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, KanjiToRadicals);
    }

    public static string GetLearningSetsPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, LearningSets);
    }

    public static string GetKanjiListPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, KanjiList);
    }

    public static string GetKanaListPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, KanaList);
    }

    public static string GetVerbListPath ()
    {
        return Path.Combine (Application.streamingAssetsPath, VerbList);
    }

}