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

using System.Text;
using System.Text.Json;
using TSMoreland.Text.Json.NamingStrategies.Strategies;

namespace TSMoreland.Text.Json.NamingStrategies.Test;

public sealed class JsonStrategizedStringEnumConverterTest
{
    private readonly Mock<IEnumNamingStrategy> _mockStrategy;

    public JsonStrategizedStringEnumConverterTest()
    {
        _mockStrategy = new Mock<IEnumNamingStrategy>();
    }

    [Fact]
    public void Constructor_Throws_ArgumentNullException_WhenStrategyIsNull()
    {
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => _ = new JsonStrategizedStringEnumConverter<SampleValues>(null!));
        ex.Should()
            .NotBeNull()
            .And
            .Match<ArgumentNullException>(e => e.ParamName == "strategy");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanConvert_ReturnsStrategyCanConvert(bool expected)
    {
        _mockStrategy.Setup(m => m.CanConvert(It.IsAny<Type>())).Returns(expected);
        JsonStrategizedStringEnumConverter<SampleValues> converter = new(_mockStrategy.Object);
        bool actual = converter.CanConvert(typeof(SampleValues));
        actual.Should().Be(expected);
    }

    [Fact]
    public void Read_ThrowsJsonException_WhenReaderTokenIsNotStringOrNumber()
    {
        JsonStrategizedStringEnumConverter<SampleValues> converter = new(_mockStrategy.Object);

        JsonException ex = Assert
            .Throws<JsonException>(() =>
            {
                byte[] utf8 = Encoding.UTF8.GetBytes(@"{""exists"":false}");
                Utf8JsonReader reader = new(utf8.AsSpan(), true, new JsonReaderState());
                converter.Read(ref reader, typeof(SampleValues), new JsonSerializerOptions());
            });
        ex.Should().NotBeNull();
    }


    /// <summary>
    /// Minimal implementation intended for mocking becuase Moq/NSubstite can't deal with
    /// <see cref="ReadOnlySpan{Char}"/>
    /// </summary>
    public sealed class SampleEnumNamingStrategy : IEnumNamingStrategy
    {
        private readonly bool _canConvert;
        private readonly object? _convertedValue;
        private readonly string _convertedString;
        private readonly JsonEncodedText _convertedText;

        public SampleEnumNamingStrategy(
            bool canConvert = false,
            string convertedString = "",
            object? convertedValue = null,
            JsonEncodedText convertedText = default)
        {
            _canConvert = canConvert;
            _convertedString = convertedString;
            _convertedValue = convertedValue;
            _convertedText = convertedText;
        }

        /// <inheritdoc />
        public bool CanConvert(Type type) => _canConvert;

        /// <inheritdoc />
        public string Convert<TEnum>(TEnum value, JsonSerializerOptions options)
            where TEnum : struct, Enum
        {
            return _convertedString;
        }

        /// <inheritdoc />
        public JsonEncodedText ConvertToEncoded<TEnum>(TEnum value, JsonSerializerOptions options)
            where TEnum : struct, Enum
        {
            return _convertedText;
        }

        /// <inheritdoc />
        public TEnum ConvertOrThrow<TEnum, TException>(ReadOnlySpan<char> value, JsonSerializerOptions options)
            where TEnum : struct, Enum where TException : Exception, new()
        {
            if (_convertedValue is null)
            {
                throw new TException();
            }

            return (TEnum)_convertedValue;
        }
    }

}
