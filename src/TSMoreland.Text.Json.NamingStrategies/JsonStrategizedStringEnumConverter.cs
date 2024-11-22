using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using TSMoreland.Text.Json.NamingStrategies.Strategies;

namespace TSMoreland.Text.Json.NamingStrategies;

/// <summary>
/// JSON enum to string converting using a strategy defined in
/// <see cref="IEnumNamingStrategy"/>
/// </summary>
/// <typeparam name="TEnum">The Enum to convert</typeparam>
/// <inheritdoc />
public class JsonStrategizedStringEnumConverter<TEnum>(IEnumNamingStrategy strategy) : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private static readonly TypeCode _sEnumTypeCode = Type.GetTypeCode(typeof(TEnum));
    private readonly IEnumNamingStrategy _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return _strategy.CanConvert(typeToConvert);
    }

    /// <inheritdoc />
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => _strategy.ConvertOrThrow<TEnum, JsonException>(reader.GetString().AsSpan(), options),
            JsonTokenType.Number => ReadNumber(ref reader),
            _ => throw new JsonException("Unsupported token type")
        };
    }

    /// <summary>
    /// Converts number to matching enum value or throws <see cref="JsonException"/> 
    /// </summary>
    /// <param name="reader">reader containing numeric value to read</param>
    /// <returns>value of <typeparamref name="TEnum"/> matching the read number</returns>
    /// <exception cref="JsonException">
    /// when unable to convert numeric value to matching enum value
    /// </exception>
    protected virtual TEnum ReadNumber(ref Utf8JsonReader reader)
    {
        switch (_sEnumTypeCode)
        {
            // Switch cases ordered by expected frequency
            case TypeCode.Int32:
                if (reader.TryGetInt32(out var int32))
                {
                    return Unsafe.As<int, TEnum>(ref int32);
                }
                break;
            case TypeCode.UInt32:
                if (reader.TryGetUInt32(out var uint32))
                {
                    return Unsafe.As<uint, TEnum>(ref uint32);
                }
                break;
            case TypeCode.UInt64:
                if (reader.TryGetUInt64(out var uint64))
                {
                    return Unsafe.As<ulong, TEnum>(ref uint64);
                }
                break;
            case TypeCode.Int64:
                if (reader.TryGetInt64(out var int64))
                {
                    return Unsafe.As<long, TEnum>(ref int64);
                }
                break;
            case TypeCode.SByte:
                if (reader.TryGetSByte(out var byte8))
                {
                    return Unsafe.As<sbyte, TEnum>(ref byte8);
                }
                break;
            case TypeCode.Byte:
                if (reader.TryGetByte(out var ubyte8))
                {
                    return Unsafe.As<byte, TEnum>(ref ubyte8);
                }
                break;
            case TypeCode.Int16:
                if (reader.TryGetInt16(out var int16))
                {
                    return Unsafe.As<short, TEnum>(ref int16);
                }
                break;
            case TypeCode.UInt16:
                if (reader.TryGetUInt16(out var uint16))
                {
                    return Unsafe.As<ushort, TEnum>(ref uint16);
                }
                break;
        }

        throw new JsonException();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(_strategy.ConvertToEncoded(value, options));
    }
}
