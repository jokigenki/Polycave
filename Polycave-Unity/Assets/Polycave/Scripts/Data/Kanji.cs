using System.Collections.Generic;

public class Kanji
{
    public string kanji;
    public string reading;
    public string meaning;

    override public string ToString ()
    {
        return $"kanji:{kanji} reading:{reading} meaning:{meaning}";
    }
}