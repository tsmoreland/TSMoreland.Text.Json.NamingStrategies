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
        foreach (var methodContent in GetStringExtensionMethods())
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
