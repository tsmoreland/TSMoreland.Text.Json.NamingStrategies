using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using TSMoreland.Text.Json.NamingStrategies.Strategies;

namespace TSMoreland.Text.Json.NamingStrategies;

public sealed class JsonStrategizedStringEnumConverterFactory(IEnumNamingStrategy strategy) : JsonConverterFactory
{
    private readonly IEnumNamingStrategy _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) =>
        _strategy.CanConvert(typeToConvert);

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return CanConvert(typeToConvert)
            ? (JsonConverter)Activator.CreateInstance(GetEnumConverterType(typeToConvert), _strategy)!
            : null;
    }

    // based on EnumConverterFactory in System.Text.Json.Serialization.Converters
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2055:MakeGenericType",
        Justification = "'EnumConverter<T> where T : struct' implies 'T : new()', so the trimmer is warning calling MakeGenericType here because enumType's constructors are not annotated. " +
                        "But EnumConverter doesn't call new T(), so this is safe.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    private static Type GetEnumConverterType(Type enumType) =>
        typeof(JsonStrategizedStringEnumConverter<>).MakeGenericType(enumType);
}
