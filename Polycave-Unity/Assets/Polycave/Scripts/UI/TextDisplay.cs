using System.Collections.Generic;
using System.Linq;
using PolyblotPlayground;
using TMPro;
using UnityEngine;

public class TextDisplay : MonoBehaviour
{
    private TextMeshPro _japanese;
    private TextMeshPro _reading;
    private TextMeshPro _senses;

    private void Awake ()
    {
        _japanese = transform.Find ("Japanese").GetComponent<TextMeshPro> ();
        _reading = transform.Find ("Reading").GetComponent<TextMeshPro> ();
        _senses = transform.Find ("Senses").GetComponent<TextMeshPro> ();
    }

    public void DisplayLearningItem (LearningSetItem item)
    {
        Source source = item.data.Values.First ();
        _japanese.text = JoinData (source.kanji, ", ");
        _reading.text = JoinData (source.reading, ", ");
        _senses.text = JoinData (source.sense, "\n");
    }

    private string JoinData (List<Dictionary<string, string>> source, string separator)
    {
        List<string> v = source.Select (k => k.Values.First ()).ToList ();
        return string.Join (separator, v);
    }

    public void DisplayKanji (string kanji)
    {
        _japanese.text = kanji;
        _reading.text = "";
        _senses.text = "";
    }

    public void DisplaySentence (ExampleSentence sentence)
    {
        _japanese.text = sentence.japanese;
        _reading.text = sentence.english;
        _senses.text = "";
    }
}