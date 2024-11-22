using System.Text.Json;
using TSMoreland.Text.Json.NamingStrategies.NamingPolicies;

namespace TSMoreland.Text.Json.NamingStrategies;

public static class JsonStrategizedNamingPolicy
{
    /// <summary>
    /// Returns the naming policy for snake_case
    /// </summary>
    public static JsonNamingPolicy SnakeCase { get; } = new JsonSnakeCaseNamingPolicy();

    /// <summary>
    /// Returns the naming policy for kebab_case
    /// </summary>
    public static JsonNamingPolicy KebabCase { get; } = new JsonKebabCaseNamingPolicy();

}
