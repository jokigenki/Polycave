using System.Collections.Generic;
using System.Linq;
using PolyblotPlayground;
using TMPro;
using UnityEngine;

public class PreviewDisplay : MonoBehaviour
{
    private TextMeshPro _japanese;

    private void Awake ()
    {
        _japanese = transform.Find ("Japanese").GetComponent<TextMeshPro> ();
    }

    public void DisplayData<T> (T data)
    {
        if (data is LearningSetItem) DisplayLearningItem (data as LearningSetItem);
        else if (data is ExampleSentence) DisplaySentence (data as ExampleSentence);
        else if (data is Kanji) DisplayKanji (data as Kanji);
    }

    private void DisplayLearningItem (LearningSetItem item)
    {
        Source source = item.data.Values.First ();
        _japanese.text = JoinData (source.kanji, ", ");
    }

    private string JoinData (List<Dictionary<string, string>> source, string separator)
    {
        List<string> v = source.Select (k => k.Values.First ()).ToList ();
        return string.Join (separator, v);
    }

    private void DisplayKanji (Kanji kanji)
    {
        _japanese.text = kanji.kanji;
    }

    private void DisplaySentence (ExampleSentence sentence)
    {
        _japanese.text = sentence.japanese;
    }
}