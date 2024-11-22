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

using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace TSMoreland.Text.Json.NamingStrategies.Test;

public sealed class EnumModelBinderTest
{
    private readonly ILogger<EnumModelBinder> _logger;
    private readonly IOptions<JsonOptions> _options;
    private ModelBindingResult? _bindingResult;

    public EnumModelBinderTest()
    {
        _logger = new LoggerFactory().CreateLogger<EnumModelBinder>();
        _options = Options.Create(new JsonOptions());
        _options.Value.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public SampleValue EnumProperty { get; } = SampleValue.Alpha;

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => _ = new EnumModelBinder(_options, null!));
        ex.Should()
            .NotBeNull()
            .And
            .Match<ArgumentNullException>(e => e.ParamName == "logger");
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => _ = new EnumModelBinder(null!, _logger));
        ex.Should()
            .NotBeNull()
            .And
            .Match<ArgumentNullException>(e => e.ParamName == "options");
    }

    [Fact]
    public void BindModelAsync_ReturnsFailedTask_WhenBindingContextIsNull()
    {
        EnumModelBinder binder = new(_options, _logger);
        var task = binder.BindModelAsync(null!);
        task.Should().Match<Task>(t => t.IsFaulted);
    }

    /// <remarks>
    /// this method is excessively complicated and while it was good to verify once it's not worth
    /// maintaining, going forward a pseudo-integration test or asp.net core controller test would
    /// be better placed to verify this
    /// </remarks>
    [Fact]
    public async Task BindModelAsync_SetsSuccessfulBindingResult_WhenModelTypeIsEnumAndStringCanBeDeserialized()
    {
        var (context, modelState) =
            ArrangeBindModel("key", SampleValue.Bravo.ToString(), typeof(SampleValue));
        EnumModelBinder binder = new(_options, _logger);

        await binder.BindModelAsync(context.Object);

        _bindingResult.Should()
            .NotBeNull()
            .And
            .Match<ModelBindingResult>(r => r.IsModelSet)
            .And
            .Match<ModelBindingResult>(r => r.Model!.Equals(SampleValue.Bravo));
    }

    [Fact]
    public async Task BindModelAsync_SetsSuccessfulBindingResult_WhenModelTypeIsEnumAndNumberCanBeDeserialized()
    {
        var (context, modelState) =
            ArrangeBindModel("key", ((int)SampleValue.Bravo).ToString(CultureInfo.InvariantCulture), typeof(SampleValue));
        EnumModelBinder binder = new(_options, _logger);

        await binder.BindModelAsync(context.Object);

        _bindingResult.Should()
            .NotBeNull()
            .And
            .Match<ModelBindingResult>(r => r.IsModelSet)
            .And
            .Match<ModelBindingResult>(r => r.Model!.Equals(SampleValue.Bravo));
    }

    private (Mock<MockModelBindingContext> Context, ModelStateDictionary modelState) ArrangeBindModel(
        string modelName,
        string modelValue,
        Type modelType)
    {
        Mock<MockModelBindingContext> context = new();
        Mock<IValueProvider> valueProvider = new();
        ValueProviderResult providerResult = new(new StringValues(modelValue));
        context.SetupGet(m => m.ModelName).Returns(modelName);
        context.SetupGet(m => m.ValueProvider).Returns(valueProvider.Object);
        valueProvider.Setup(m => m.GetValue(modelName)).Returns(providerResult);
        ModelStateDictionary modelState = new();
        context.SetupGet(m => m.ModelState).Returns(modelState);

        Mock<ModelMetadata> metadata = new(ModelMetadataIdentity.ForType(modelType));
        context.SetupGet(m => m.ModelMetadata).Returns(metadata.Object);
        context
            .SetupSet(m => m.Result = It.IsAny<ModelBindingResult>())
            .Callback<ModelBindingResult>(result => _bindingResult = result);

        return (context, modelState);
    }

    public abstract class MockModelBindingContext : ModelBindingContext
    {
        protected MockModelBindingContext()
        {
        }
    }
}
