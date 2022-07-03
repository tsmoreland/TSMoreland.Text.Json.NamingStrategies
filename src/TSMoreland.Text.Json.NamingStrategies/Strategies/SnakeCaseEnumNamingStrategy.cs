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

using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;

namespace TSMoreland.Text.Json.NamingStrategies.Strategies;

public sealed class SnakeCaseEnumNamingStrategy : IEnumNamingStrategy
{
    private readonly Hashtable _typesWithFlagsAttribute;
    private readonly Hashtable _encodedValuesByType;

    /// <summary>
    /// Initialises a new instance of the <see cref="SnakeCaseEnumNamingStrategy"/> class.
    /// </summary>
    public SnakeCaseEnumNamingStrategy()
    {
        _typesWithFlagsAttribute = Hashtable.Synchronized(new Hashtable());
        _encodedValuesByType = Hashtable.Synchronized(new Hashtable());
    }

    /// <inheritdoc />
    public bool CanConvert(Type type)
    {
        if (!type.IsEnum)
        {
            return false;
        }

        if (_typesWithFlagsAttribute[type] is not bool hasFlags)
        {
            hasFlags = type.GetCustomAttribute<FlagsAttribute>() is not null;
        }

        if (_typesWithFlagsAttribute.Count > 64)
        {
            _typesWithFlagsAttribute.Clear(); // would be nicer to remove one at a time but I don't yet see how, Remove needs a key
        }

        return !hasFlags;
    }

    /// <inheritdoc />
    public string Convert<TEnum>(TEnum value, JsonSerializerOptions options) where TEnum : struct, Enum
    {
        return value.ToString().ToSnakeCase();
    }

    /// <inheritdoc />
    public JsonEncodedText ConvertToEncoded<TEnum>(TEnum value, JsonSerializerOptions options)
        where TEnum : struct, Enum
    {
        Dictionary<JsonEncodedText, TEnum> valueByEncodedText = GetOrAddValueByEncodedText<TEnum>(options);
        return valueByEncodedText
            .Where(p => p.Value.Equals(value))
            .Select(p => p.Key)
            .First();
    }

    /// <inheritdoc />
    public TEnum ConvertOrThrow<TEnum, TException>(ReadOnlySpan<char> value, JsonSerializerOptions options)
        where TEnum : struct, Enum
        where TException : Exception, new()
    {
        JsonEncodedText text = JsonEncodedText.Encode(value, options.Encoder);
        Dictionary<JsonEncodedText, TEnum> valueByEncodedText = GetOrAddValueByEncodedText<TEnum>(options);

        return valueByEncodedText.TryGetValue(text, out TEnum enumValue)
            ? enumValue
            : throw new TException();
    }

    private Dictionary<JsonEncodedText, TEnum> GetOrAddValueByEncodedText<TEnum>(JsonSerializerOptions options)
        where TEnum : struct, Enum
    {
        Type key = typeof(TEnum);
        if (_encodedValuesByType[key] is Dictionary<JsonEncodedText, TEnum> valueByEncodedText)
        {
            return valueByEncodedText;
        }

        Dictionary<string, string?> enumMemberValueByName = key
            .GetTypeInfo()
            .DeclaredMembers
            .ToDictionary(m => m.Name, m => m.GetCustomAttribute<EnumMemberAttribute>()?.Value);

        valueByEncodedText = Enum.GetValues<TEnum>()
            .ToDictionary(
                v =>
                {
                    string name = v.ToString();
                    if (enumMemberValueByName.TryGetValue(name, out string? enumMemberValue) &&
                        enumMemberValue is { Length: > 0 })
                    {
                        return JsonEncodedText.Encode(enumMemberValue, options.Encoder);
                    }
                    else
                    {
                        return JsonEncodedText.Encode(v.ToString().ToSnakeCase(), options.Encoder);
                    }
                },
                v => v);
        _encodedValuesByType[key] = valueByEncodedText;
        return valueByEncodedText;

    }
}
