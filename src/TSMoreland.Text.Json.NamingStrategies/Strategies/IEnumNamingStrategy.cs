using System.Text.Json;

namespace TSMoreland.Text.Json.NamingStrategies.Strategies;

/// <summary>
/// Naming strategy used to convert enums to string and back
/// </summary>
public interface IEnumNamingStrategy
{
    /// <summary>
    /// Returns true if the naming strategy can convert <paramref name="type"/>; in most if not all cases
    /// </summary>
    /// <param name="type">the type to check</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="type"/> can be converted by this type
    /// as well as any further filters the strategy chooses to apply.
    /// </returns>
    public bool CanConvert(Type type);

    /// <summary>
    /// Converts <paramref name="value"/> to string
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="options">used to encode <paramref name="value"/></param>
    /// <returns>
    /// string representation of <paramref name="value"/> formatted using the
    /// naming strategy of this instance
    /// </returns>
    public string Convert<TEnum>(TEnum value, JsonSerializerOptions options)
        where TEnum : struct, Enum;

    /// <summary>
    /// Converts <paramref name="value"/> to an instance of <see cref="JsonEncodedText"/>
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="options">used to encode <paramref name="value"/></param>
    /// <returns>
    /// string representation of <paramref name="value"/> formatted using the
    /// naming strategy of this instance
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// if this strategy does not support conversion to JsonEncodedText
    /// </exception>
    public JsonEncodedText ConvertToEncoded<TEnum>(TEnum value, JsonSerializerOptions options)
        where TEnum : struct, Enum;

    /// <summary>
    /// Converts <paramref name="value"/> to matching value of <typeparamref name="TEnum"/>
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="options"></param>
    /// <returns>
    /// <typeparamref name="TEnum"/> converted from <paramref name="value"/>
    /// </returns>
    /// <exception cref="Exception">
    /// <see cref="Exception"/> of type <typeparamref name="TException"/>
    /// if <paramref name="value"/> does not match a value in
    /// <typeparamref name="TEnum"/> converted to string by <see cref="Convert{TEnum}(TEnum, JsonSerializerOptions)"/>
    /// </exception>
    public TEnum ConvertOrThrow<TEnum, TException>(ReadOnlySpan<char> value, JsonSerializerOptions options)
        where TEnum : struct, Enum
        where TException : Exception, new();

    /// <summary>
    /// Converts <paramref name="value"/> to matching value of <typeparamref name="TEnum"/>
    /// </summary>
    /// <param name="value">value to convert</param>
    /// <param name="options"></param>
    /// <returns>
    /// <typeparamref name="TEnum"/> converted from <paramref name="value"/>
    /// </returns>
    /// <exception cref="Exception">
    /// <see cref="Exception"/> of type <typeparamref name="TException"/>
    /// if <paramref name="value"/> is <see langword="null"/> or does not match a value in
    /// <typeparamref name="TEnum"/> converted to string by <see cref="ConvertOrThrow{TEnum, TException}(ReadOnlySpan{char}, JsonSerializerOptions)"/>
    /// </exception>
    public TEnum ConvertOrThrow<TEnum, TException>(string? value, JsonSerializerOptions options)
        where TEnum : struct, Enum
        where TException : Exception, new()
    {
        return value is not null
            ? ConvertOrThrow<TEnum, TException>(value.AsSpan(), options)
            : throw new TException();
    }
}
