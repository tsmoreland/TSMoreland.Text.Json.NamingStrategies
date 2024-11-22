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

using System.Text.Json;
using System.Text.Json.Serialization;
using TSMoreland.Text.Json.NamingStrategies.Strategies;

namespace TSMoreland.Text.Json.NamingStrategies.Test;

public sealed class JsonStrategizedStringEnumConverterFactoryTest
{
    private readonly Mock<IEnumNamingStrategy> _strategy;

    public JsonStrategizedStringEnumConverterFactoryTest()
    {
        _strategy = new Mock<IEnumNamingStrategy>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenStrategyIsNull()
    {
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => _ = new JsonStrategizedStringEnumConverterFactory(null!));
        ex.Should()
            .NotBeNull()
            .And
            .Match<ArgumentNullException>(e => e.ParamName == "strategy");
    }

    [Fact]
    public void Constructor_DoesNotThrow_WhenStrategyIsNonNull()
    {
        Exception? ex = Record.Exception(() => _ = new JsonStrategizedStringEnumConverterFactory(_strategy.Object));
        ex.Should().BeNull();
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void CreateConverter_ReturnsNonNull_WhenCanConvert_OtherwiseNull(bool canConvert, bool expectNull)
    {
        _strategy
            .Setup(m => m.CanConvert(typeof(SampleValue)))
            .Returns(canConvert);

        JsonStrategizedStringEnumConverterFactory factory = new(_strategy.Object);
        JsonConverter? converter = factory.CreateConverter(typeof(SampleValue), new JsonSerializerOptions());

        if (expectNull)
        {
            converter.Should().BeNull();
        }
        else
        {
            converter.Should().NotBeNull();
        }
    }
}
