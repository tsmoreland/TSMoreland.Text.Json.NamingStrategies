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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TSMoreland.Text.Json.NamingStrategies.Test;

public sealed class EnumModelBinderTest
{
    private readonly ILogger<EnumModelBinder> _logger;
    private readonly IOptions<JsonOptions> _options;

    public EnumModelBinderTest()
    {
        _logger = new LoggerFactory().CreateLogger<EnumModelBinder>();
        _options = Options.Create(new JsonOptions());
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => _ = new EnumModelBinder(_options, null!));
        ex.Should()
            .NotBeNull()
            .And
            .Match<ArgumentNullException>(e => e.ParamName == "logger");
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => _ = new EnumModelBinder(null!, _logger));
        ex.Should()
            .NotBeNull()
            .And
            .Match<ArgumentNullException>(e => e.ParamName == "options");
    }

    [Fact]
    public void BindModelAsync_ReturnsFailedTask_WhenBindingContextIsNull()
    {
        EnumModelBinder binder = new(_options, _logger);
        Task task = binder.BindModelAsync(null!);
        task.Should().Match<Task>(t => t.IsFaulted);
    }

    [Fact]
    public void BindModelAsync_SetsSuccessfulBindingResult_WhenModelTypeIsEnumAndStringCanBeDeserialized()
    {
        EnumModelBinder binder = new(_options, _logger);

    }

}
