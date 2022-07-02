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

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using TSMoreland.Text.Json.NamingStrategies.Strategies;

namespace TSMoreland.Text.Json.NamingStrategies;

/// <summary>
/// JSON enum to string converting using a strategy defined in
/// <see cref="IEnumNamingStrategy{TEnum}"/>
/// </summary>
/// <typeparam name="TEnum">The Enum to convert</typeparam>
public abstract class JsonStrategizedStringEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private static readonly TypeCode s_enumTypeCode = Type.GetTypeCode(typeof(TEnum));
    private readonly IEnumNamingStrategy<TEnum> _strategy;

    /// <inheritdoc />
    private protected JsonStrategizedStringEnumConverter(IEnumNamingStrategy<TEnum> strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }


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
            JsonTokenType.String => _strategy.ConvertOrThrow<JsonException>(reader.GetString().AsSpan()),
            JsonTokenType.Number => ReadNumber(ref reader),
            _ => throw new JsonException("Unsupported token type")
        };
    }

    /// <summary>
    /// Converts number to matching enum value or throws <see cref="JsonException"/> 
    /// </summary>
    /// <param name="reader">reader containing numeric value to read</param>
    /// <returns>value of <typeparamref name="TEnum"/> matching the read number</returns>
    /// <exception cref="JsonException"></exception>
    protected virtual TEnum ReadNumber(ref Utf8JsonReader reader)
    {
        switch (s_enumTypeCode)
        {
            // Switch cases ordered by expected frequency
            case TypeCode.Int32:
                if (reader.TryGetInt32(out int int32))
                {
                    return Unsafe.As<int, TEnum>(ref int32);
                }
                break;
            case TypeCode.UInt32:
                if (reader.TryGetUInt32(out uint uint32))
                {
                    return Unsafe.As<uint, TEnum>(ref uint32);
                }
                break;
            case TypeCode.UInt64:
                if (reader.TryGetUInt64(out ulong uint64))
                {
                    return Unsafe.As<ulong, TEnum>(ref uint64);
                }
                break;
            case TypeCode.Int64:
                if (reader.TryGetInt64(out long int64))
                {
                    return Unsafe.As<long, TEnum>(ref int64);
                }
                break;
            case TypeCode.SByte:
                if (reader.TryGetSByte(out sbyte byte8))
                {
                    return Unsafe.As<sbyte, TEnum>(ref byte8);
                }
                break;
            case TypeCode.Byte:
                if (reader.TryGetByte(out byte ubyte8))
                {
                    return Unsafe.As<byte, TEnum>(ref ubyte8);
                }
                break;
            case TypeCode.Int16:
                if (reader.TryGetInt16(out short int16))
                {
                    return Unsafe.As<short, TEnum>(ref int16);
                }
                break;
            case TypeCode.UInt16:
                if (reader.TryGetUInt16(out ushort uint16))
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
        if (_strategy.SupportsConversionToJsonEncodedText)
        {
            writer.WriteStringValue(_strategy.ConvertToEncoded(value));
        }
        else
        {
            writer.WriteStringValue(JsonEncodedText.Encode(_strategy.Convert(value), options.Encoder));
        }
    }
}
