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

using System.Reflection;

namespace TSMoreland.Text.Json.NamingStrategies.SourceGenerator;

internal static class StringExtensionsSourceFactory
{
    public static SourceFile Build()
    {
        StringBuilder builder = new();

        builder.Append($@"
using System;
using System.Text;

namespace TSMoreland.Text.Json.NamingStrategies
{{

    [System.CodeDom.Compiler.GeneratedCodeAttribute(""{nameof(Generator)}"", ""{Assembly.GetExecutingAssembly().GetName().Version}"")]
    public static partial class StringExtensions
    {{
");
        foreach (string methodContent in GetStringExtensionMethods())
        {
            builder.Append(methodContent);
        }

        builder.Append("\n    }\n}");

        return new SourceFile("StringExtensions.g.cs", builder.ToString());

    }

    private static IEnumerable<string> GetStringExtensionMethods()
    {
        yield return GetToSnakeCaseImplementation();
        yield return GetToKebabCaseImplementation();
    }

    private static string GetToSnakeCaseImplementation()
    {
        return GetToSeparatedCaseImplementation("ToSnakeCase", '_', '-');

    }
    private static string GetToKebabCaseImplementation()
    {
        return GetToSeparatedCaseImplementation("ToKebabCase", '-', '_');
    }

    private static string GetToSeparatedCaseImplementation(string methodName, char separator, char alternateSeparator)
    {
        return $@"
        public static partial string {methodName}(this string source)
        {{
            if (source is not {{ Length: > 0 }})
            {{
                return source;
            }}

            // worst case size
            StringBuilder builder = new(source.Length * 2);

            ReadOnlySpan<char> asSpan = source.AsSpan();

            for (int i = 0; i < asSpan.Length; i++)
            {{
                char ch = asSpan[i];
                if (char.IsWhiteSpace(ch))
                {{
                    if (builder.Length > 0)
                    {{
                        builder.Append('{separator}');
                    }}

                    continue;
                }}

                switch (ch)
                {{
                    case >= 'a' and <= 'z' or >= '0' and <= '9' or '{separator}':
                        builder.Append(ch);
                        continue;
                    case '{alternateSeparator}' or '{separator}':
                        builder.Append('{separator}');
                        continue;
                }}

                if (builder.Length == 0)
                {{
                    builder.Append(char.ToLower(ch));
                    continue;
                }}

                char previous = asSpan[i - 1];
                if (previous != '{separator}' && previous != '{alternateSeparator}' && i + 1 < asSpan.Length)
                {{
                    char next = asSpan[i + 1];
                    if (!char.IsUpper(next) && !char.IsNumber(next) && next != '{alternateSeparator}' && next != '{separator}' && next != '{alternateSeparator}')
                    {{
                        builder.Append('{separator}');
                    }}
                }}

                builder.Append(char.ToLower(ch));
            }}
            return builder.ToString().TrimEnd('{separator}');
        }}";
    }
}
