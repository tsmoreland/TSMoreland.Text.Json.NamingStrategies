using System.Reflection;

namespace TSMoreland.Text.Json.NamingStrategies.SourceGenerator;

public sealed class CaseSeparatedEnumNamingStrategyFactory
{
    public static SourceFile Build(string classNamePrefix, string extensionMethodName)
    {
        StringBuilder builder = new();

        var content = $$"""
            using System;
            using System.Collections;
            using System.Reflection;
            using System.Runtime.Serialization;
            using System.Text.Json;

            #nullable enable

            namespace TSMoreland.Text.Json.NamingStrategies.Strategies
            {
            
            
                [System.CodeDom.Compiler.GeneratedCodeAttribute("{{nameof(Generator)}}", "{{Assembly.GetExecutingAssembly().GetName().Version}}")]
                public sealed partial class {{classNamePrefix}}EnumNamingStrategy 
                {
                    private readonly Hashtable _typesWithFlagsAttribute;
                    private readonly Hashtable _encodedValuesByType;
            
                    /// <summary>
                    /// Initialises a new instance of the <see cref="{{classNamePrefix}}EnumNamingStrategy"/> class.
                    /// </summary>
                    public {{classNamePrefix}}EnumNamingStrategy()
                    {
                        {
                            _typesWithFlagsAttribute = Hashtable.Synchronized(new Hashtable());
                            _encodedValuesByType = Hashtable.Synchronized(new Hashtable());
                        }
                    }
            
                    /// <inheritdoc />
                    public partial bool CanConvert(Type type)
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
                    public partial string Convert<TEnum>(TEnum value, JsonSerializerOptions options) where TEnum : struct, Enum
                    {
                        return value.ToString().{{extensionMethodName}}();
                    }
            
                    /// <inheritdoc />
                    public partial JsonEncodedText ConvertToEncoded<TEnum>(TEnum value, JsonSerializerOptions options)
                        where TEnum : struct, Enum
                    {
                        Dictionary<JsonEncodedText, TEnum> valueByEncodedText = GetOrAddValueByEncodedText<TEnum>(options);
                        return valueByEncodedText
                            .Where(p => p.Value.Equals(value))
                            .Select(p => p.Key)
                            .First();
                    }
            
                    /// <inheritdoc />
                    public partial TEnum ConvertOrThrow<TEnum, TException>(ReadOnlySpan<char> value, JsonSerializerOptions options)
                        where TEnum : struct, Enum
                        where TException : Exception, new()
                    {
                        JsonEncodedText text = JsonEncodedText.Encode(value, options.Encoder);
                        Dictionary<JsonEncodedText, TEnum> valueByEncodedText = GetOrAddValueByEncodedText<TEnum>(options);
            
                        if (valueByEncodedText.TryGetValue(text, out TEnum enumValue))
                        {
                            return enumValue;
                        }
            
                        // fallback to parsing without naming strategy since this will apply to multiple enums which may not
                        if (Enum.TryParse(value, true, out enumValue))
                        {
                            return enumValue;
                        }
            
                        throw new TException();
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
                                        return JsonEncodedText.Encode(v.ToString().{{extensionMethodName}}(), options.Encoder);
                                    }
                                },
                                v => v);
                        _encodedValuesByType[key] = valueByEncodedText;
                        return valueByEncodedText;
                    }
                }
            }
            """;
        return new SourceFile($"{classNamePrefix}EnumNamingStrategy.g.cs", content);
    }

}
