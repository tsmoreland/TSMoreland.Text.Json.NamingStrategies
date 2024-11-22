using System.Text.Json;

namespace TSMoreland.Text.Json.NamingStrategies.NamingPolicies;

internal sealed class JsonKebabCaseNamingPolicy : JsonNamingPolicy
{
    /// <inheritdoc />
    public override string ConvertName(string name) => name.ToKebabCase();
}
