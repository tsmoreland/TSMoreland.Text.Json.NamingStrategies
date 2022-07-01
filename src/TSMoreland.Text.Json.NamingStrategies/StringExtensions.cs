//
// Copyright © 2022 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Text;

namespace TSMoreland.Text.Json.SnakeCase;

public static class StringExtensions
{
    /// <summary>
    /// Converts <paramref name="source"/> to snake_case format
    /// </summary>
    /// <param name="source">string to convert</param>
    /// <returns>string without padding in snake case format</returns>
    public static string ToSnakeCase(this string source)
    {
        if (source is not { Length: > 0 })
        {
            return source;
        }


        // worst case size
        StringBuilder builder = new(source.Length * 2);

        ReadOnlySpan<char> asSpan = source.AsSpan();
        builder.Append(char.ToLower(asSpan[0]));

        for (int i = 1; i < asSpan.Length; i++)
        {
            char ch = asSpan[i];
            if (builder.Length == 0 && char.IsWhiteSpace(ch))
            {
                continue;
            }


            switch (ch)
            {
                case >= 'a' and <= 'z' or >= '0' and <= '9' or '_':
                    builder.Append(ch);
                    continue;
                case '-': // kebab-case
                    builder.Append('_');
                    break;
            }

            bool nextChangesCase = i + 1 < asSpan.Length && !char.IsUpper(asSpan[i + 1]) && asSpan[i + 1] != '_' && !char.IsNumber(asSpan[i + 1]);

            if (ch != '_' && nextChangesCase)
            {
                builder.Append('_');

            }

            builder.Append(char.ToLower(ch));
        }
        return builder.ToString();
    }

}
