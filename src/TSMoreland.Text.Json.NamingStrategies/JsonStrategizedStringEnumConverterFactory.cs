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

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using TSMoreland.Text.Json.NamingStrategies.Strategies;

namespace TSMoreland.Text.Json.NamingStrategies;

public /*abstract*/ class JsonStrategizedStringEnumConverterFactory : JsonConverterFactory
{
    private readonly IEnumNamingStrategy _strategy;

    public JsonStrategizedStringEnumConverterFactory(IEnumNamingStrategy strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) =>
        _strategy.CanConvert(typeToConvert);

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return CanConvert(typeToConvert)
            ? (JsonConverter)Activator.CreateInstance(GetEnumConverterType(typeToConvert), new object?[] { _strategy })!
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
