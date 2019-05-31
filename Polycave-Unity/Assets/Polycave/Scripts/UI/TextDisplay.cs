using System.Collections.Generic;
using System.Linq;
using HiraKana;
using PolyblotPlayground;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour
{
    private TextMeshPro _japanese;
    private TextMeshPro _reading;
    private TextMeshPro _senses;
    private SpriteRenderer _spriteRenderer;

    private void Awake ()
    {
        _japanese = transform.Find ("Japanese").GetComponent<TextMeshPro> ();
        _reading = transform.Find ("Reading").GetComponent<TextMeshPro> ();
        _senses = transform.Find ("Senses").GetComponent<TextMeshPro> ();
        _spriteRenderer = GetComponent<SpriteRenderer> ();
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
        string kanji = JoinData (source.kanji, ", ");
        string reading = JoinData (source.readings, ", ");
        string romaji = new KanaTools ().HiraganaToRomaji (reading);
        string fullReading = $"{romaji}\n{reading}";
        _japanese.text = kanji != "" ? kanji : fullReading;
        _reading.text = kanji != "" ? fullReading : "";
        _senses.text = JoinData (source.senses, "\n");
    }

    private string JoinData (List<Dictionary<string, string>> source, string separator)
    {
        if (source == null) return "";
        List<string> v = source.Select (k => k.Values.First ()).ToList ();
        return string.Join (separator, v);
    }

    private void DisplayKanji (Kanji kanji)
    {
        _japanese.text = kanji.kanji;
        _reading.text = kanji.reading;
        _senses.text = kanji.meaning;
    }

    private void DisplaySentence (ExampleSentence sentence)
    {
        _japanese.text = sentence.japanese;
        _reading.text = sentence.english;
        _senses.text = "";
    }
}