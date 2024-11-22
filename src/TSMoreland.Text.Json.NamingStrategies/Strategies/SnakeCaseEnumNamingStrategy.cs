using System.Text.Json;

namespace TSMoreland.Text.Json.NamingStrategies.Strategies;

public sealed partial class SnakeCaseEnumNamingStrategy : IEnumNamingStrategy
{
    /// <inheritdoc />
    public partial bool CanConvert(Type type);

    /// <inheritdoc />
    public partial string Convert<TEnum>(TEnum value, JsonSerializerOptions options)
        where TEnum : struct, Enum;

    /// <inheritdoc />
    public partial JsonEncodedText ConvertToEncoded<TEnum>(TEnum value, JsonSerializerOptions options)
        where TEnum : struct, Enum;

    /// <inheritdoc />
    public partial TEnum ConvertOrThrow<TEnum, TException>(ReadOnlySpan<char> value, JsonSerializerOptions options)
        where TEnum : struct, Enum
        where TException : Exception, new();

}
