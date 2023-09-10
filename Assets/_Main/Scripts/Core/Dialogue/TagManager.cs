using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class TagManager
{
    private static readonly Dictionary<string, Func<String>> tags = new Dictionary<string, Func<string>>()
    {
        {"<mainChar>",      () => "" },
        { "<time>",         () => DateTime.Now.ToString("tt hh:mm") },
        { "<playerLevel>",  () => "15" },
        { "<input>",        () => InputPanel.instance.lastInput },
        { "<tempVal1>",     () => "42" }

    };
    private static readonly Regex tagRegex = new Regex("<\\w+>");

    public static string Inject(string text)
    {
        if(tagRegex.IsMatch(text))
        {
            foreach(Match match in tagRegex.Matches(text))
            {
                if(tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    text = text.Replace(match.Value, tagValueRequest());
                }
            }
        }

        return text;
    }
}
