using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Works with google sheets csv export
public class CSVReader
{

    // Credits: regexr.com/3apuc
    static string FIELD_CAPTURE_RE = "((?:\"(?:\"{2}|,|\n|[^\"]*)+\")|(?:[^,\"\n]+))";
 
    // Lines - limit of lines being read (excluding header)
    public static List<Dictionary<string, string>> Read(string file, int lines)
    {
        var list = new List<Dictionary<string, string>>();

        // Load file from resources
        TextAsset data = Resources.Load(file) as TextAsset;

        // Capture fields
        var fields = Regex.Matches(data.text, FIELD_CAPTURE_RE);

        var headers = new List<string>();

        // Detect headers
        for(int i = 0; i < fields.Count; ++i)
        {
            var match = fields[i];

            // If comma comes after the field, we're still on the
            // first line
            if(data.text[match.Index + match.Length] == ',')
            {
                headers.Add(match.Value);
                continue;
            }
            
            // If line separator comes after the field, it means
            // we've finished the first line, we have our headers
            headers.Add(match.Value);
            break;
        }

        // Iterate lines
        for(int i = headers.Count; i < Mathf.Min(fields.Count,(lines+1)*headers.Count); i+=headers.Count)
        {
            var dict = new Dictionary<string, string>();

            // Add values into dict and append it
            for(int j = i; j < i + headers.Count; j++)
            {
                var key = headers[j % headers.Count];
                var value = fields[j].Value;
                dict.Add(key, value);
            }

            list.Add(dict);
        }

        return list;
    }
}