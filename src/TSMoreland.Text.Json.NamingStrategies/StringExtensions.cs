namespace TSMoreland.Text.Json.NamingStrategies;

public static partial class StringExtensions
{
    /// <summary>
    /// Converts <paramref name="source"/> to kebab-case format
    /// </summary>
    /// <param name="source">string to convert</param>
    /// <returns>string without padding in snake case format</returns>
    public static partial string ToKebabCase(this string source);

    /// <summary>
    /// Converts <paramref name="source"/> to snake_case format
    /// </summary>
    /// <param name="source">string to convert</param>
    /// <returns>string without padding in snake case format</returns>
    public static partial string ToSnakeCase(this string source);
}
