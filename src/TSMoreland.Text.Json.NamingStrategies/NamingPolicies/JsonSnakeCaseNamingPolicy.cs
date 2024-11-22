using System.Text.Json;

namespace TSMoreland.Text.Json.NamingStrategies.NamingPolicies;

internal sealed class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
{
    /// <inheritdoc />
    public override string ConvertName(string name) => name.ToSnakeCase();
}
