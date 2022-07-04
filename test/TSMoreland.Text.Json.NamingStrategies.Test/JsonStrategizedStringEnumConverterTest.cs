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
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => _ = new JsonStrategizedStringEnumConverter<SampleValue>(null!));
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
        JsonStrategizedStringEnumConverter<SampleValue> converter = new(_mockStrategy.Object);
        bool actual = converter.CanConvert(typeof(SampleValue));
        actual.Should().Be(expected);
    }

    [Fact]
    public void Read_ThrowsJsonException_WhenReaderTokenIsNotStringOrNumber()
    {
        JsonStrategizedStringEnumConverter<SampleValue> converter = new(_mockStrategy.Object);

        JsonException ex = Assert
            .Throws<JsonException>(() =>
            {
                byte[] utf8 = Encoding.UTF8.GetBytes(@"{""exists"":false}");
                Utf8JsonReader reader = new(utf8.AsSpan(), true, new JsonReaderState());
                converter.Read(ref reader, typeof(SampleValue), new JsonSerializerOptions());
            });
        ex.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(EnumValues))]
    public void Read_ReturnsStrategyConvertOrThrowResult_WhenTokenIsString(SampleValue expected)
    {
        SampleEnumNamingStrategy strategy = new(convertedValue: expected);
        JsonStrategizedStringEnumConverter<SampleValue> converter = new(strategy);
        byte[] utf8 = Encoding.UTF8.GetBytes($"\"[{expected}]\"");
        Utf8JsonReader reader = new(utf8.AsSpan(), true, new JsonReaderState());

        reader.Read();
        SampleValue actual = converter.Read(ref reader, typeof(SampleValue), new JsonSerializerOptions());

        actual.Should().Be(expected);
        strategy.ConvertOrThrowCallCount.Should().Be(1);
    }

    [Theory]
    [MemberData(nameof(EnumValues))]
    public void Read_ReturnsExpectedValue_WhenTokenIsNumber(SampleValue expected)
    {
        SampleEnumNamingStrategy strategy = new();
        JsonStrategizedStringEnumConverter<SampleValue> converter = new(strategy);
        byte[] utf8 = Encoding.UTF8.GetBytes($"{(int)expected}");
        Utf8JsonReader reader = new(utf8.AsSpan(), true, new JsonReaderState());

        reader.Read();
        SampleValue actual = converter.Read(ref reader, typeof(SampleValue), new JsonSerializerOptions());

        actual.Should().Be(expected);
        strategy.ConvertOrThrowCallCount.Should().Be(0);
    }

    [Fact]
    public void Read_ReturnsExpectedValue_WhenTokenIsNumberAndEnumNumberTypeIsUInt32() =>
        VerifyEnumFromNumber($"{(ulong)SampleUInt32Value.Alpha}", SampleUInt32Value.Alpha);

    [Fact]
    public void Read_ReturnsExpectedValue_WhenTokenIsNumberAndEnumNumberTypeIsInt64() =>
        VerifyEnumFromNumber($"{(long)SampleInt64Value.Alpha}", SampleInt64Value.Alpha);

    [Fact]
    public void Read_ReturnsExpectedValue_WhenTokenIsNumberAndEnumNumberTypeIsUInt64() =>
        VerifyEnumFromNumber($"{(ulong)SampleUInt64Value.Alpha}", SampleUInt64Value.Alpha);

    [Fact]
    public void Read_ReturnsExpectedValue_WhenTokenIsNumberAndEnumNumberTypeIsInt16() =>
        VerifyEnumFromNumber($"{(short)SampleInt16Value.Alpha}", SampleInt16Value.Alpha);

    [Fact]
    public void Read_ReturnsExpectedValue_WhenTokenIsNumberAndEnumNumberTypeIsUInt16() =>
        VerifyEnumFromNumber($"{(ushort)SampleUInt16Value.Alpha}", SampleUInt16Value.Alpha);

    [Fact]
    public void Read_ReturnsExpectedValue_WhenTokenIsNumberAndEnumNumberTypeIsByte() =>
        VerifyEnumFromNumber($"{(byte)SampleByteValue.Alpha}", SampleByteValue.Alpha);

    [Fact]
    public void Read_ReturnsExpectedValue_WhenTokenIsNumberAndEnumNumberTypeIsSByte() =>
        VerifyEnumFromNumber($"{(sbyte)SampleSByteValue.Alpha}", SampleSByteValue.Alpha);

    [Fact]
    public void Read_ThrowsJsonException_WhenTokenIsNumberButOutOfTypeRange()
    {
        SampleEnumNamingStrategy strategy = new();
        JsonStrategizedStringEnumConverter<SampleByteValue> converter = new(strategy);

        JsonException ex = Assert
            .Throws<JsonException>(() =>
            {
                byte[] utf8 = Encoding.UTF8.GetBytes($"{(long)SampleInt64Value.Alpha}");
                Utf8JsonReader reader = new(utf8.AsSpan(), true, new JsonReaderState());
                reader.Read();

                _ = converter.Read(ref reader, typeof(SampleByteValue), new JsonSerializerOptions());
            });
        ex.Should().NotBeNull();
    }

    private static void VerifyEnumFromNumber<TEnum>(string jsonValue, TEnum expected)
        where TEnum : struct, Enum
    {
        SampleEnumNamingStrategy strategy = new();
        JsonStrategizedStringEnumConverter<TEnum> converter = new(strategy);
        byte[] utf8 = Encoding.UTF8.GetBytes(jsonValue);
        Utf8JsonReader reader = new(utf8.AsSpan(), true, new JsonReaderState());

        reader.Read();
        TEnum actual = converter.Read(ref reader, typeof(TEnum), new JsonSerializerOptions());

        actual.Should().Be(expected);
        strategy.ConvertOrThrowCallCount.Should().Be(0);
    }


    [Theory]
    [MemberData(nameof(EnumValues))]
    public void Write_ReturnsStrategyConvertToEncodedResult(SampleValue source)
    {
        string expected = $"[{source}]";
        SampleEnumNamingStrategy strategy = new(convertedText: JsonEncodedText.Encode(expected, new JsonSerializerOptions().Encoder));
        expected = $"\"{expected}\"";
        JsonStrategizedStringEnumConverter<SampleValue> converter = new(strategy);

        using MemoryStream stream = new(new byte[128], 0, 128, true);
        Utf8JsonWriter writer = new(stream);
        converter.Write(writer, source, new JsonSerializerOptions());
        writer.Flush();
        stream.Position = 0;

        using StreamReader reader = new(stream, Encoding.UTF8, true, 128, leaveOpen: true);
        string actual = reader.ReadToEnd().Trim('\0');

        actual.Should().Be(expected);
    }



    private static IEnumerable<object[]> EnumValues()
    {
        return Enum.GetValues<SampleValue>().Select(value => new object[] { value });
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
        public int ConvertCallCount { get; private set; }
        public int ConvertToEncodedCallCount { get; private set; }
        public int ConvertOrThrowCallCount { get; private set; }

        /// <inheritdoc />
        public bool CanConvert(Type type) => _canConvert;

        /// <inheritdoc />
        public string Convert<TEnum>(TEnum value, JsonSerializerOptions options)
            where TEnum : struct, Enum
        {
            ConvertCallCount++;
            return _convertedString;
        }

        /// <inheritdoc />
        public JsonEncodedText ConvertToEncoded<TEnum>(TEnum value, JsonSerializerOptions options)
            where TEnum : struct, Enum
        {
            ConvertToEncodedCallCount++;
            return _convertedText;
        }

        /// <inheritdoc />
        public TEnum ConvertOrThrow<TEnum, TException>(ReadOnlySpan<char> value, JsonSerializerOptions options)
            where TEnum : struct, Enum where TException : Exception, new()
        {
            ConvertOrThrowCallCount++;
            if (_convertedValue is null)
            {
                throw new TException();
            }

            return (TEnum)_convertedValue;
        }
    }

}
