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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TSMoreland.Text.Json.NamingStrategies;

public class EnumModelBinder : IModelBinder
{
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initialises a new instance of the <see cref="EnumModelBinder"/> class.
    /// </summary>
    /// <param name="options">JSON options used to verify content can be de-serialized</param>
    /// <param name="logger">logger used to log errors</param>
    public EnumModelBinder(IOptions<JsonOptions> options, ILogger<EnumModelBinder> logger)
        : this(options, (ILogger)logger)
    {
    }


    /// <summary>
    /// Initialises a new instance of the <see cref="EnumModelBinder"/> class.
    /// </summary>
    /// <param name="options">JSON options used to verify content can be de-serialized</param>
    /// <param name="logger">logger used to log errors</param>
    protected EnumModelBinder(IOptions<JsonOptions> options, ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options.Value.JsonSerializerOptions;
    }

    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext, nameof(bindingContext));

        string modelName = bindingContext.ModelName;
        ValueProviderResult providerResult = bindingContext.ValueProvider.GetValue(modelName);
        if (providerResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, providerResult);
        if (providerResult.FirstValue is not { Length: > 0 } value)
        {
            return Task.CompletedTask;
        }

        try
        {
            string jsonifiedValue = string.Create(value.Length + 2, value, static (Span<char> output, string state) =>
            {
                ReadOnlySpan<char> input = state.AsSpan();
                output[0] = '"';
                int i;
                for (i = 0; i < input.Length; i++)
                {
                    output[i + 1] = input[i];
                }
                output[i+1] = '"';
            });

            object? deserializedValue = JsonSerializer.Deserialize(jsonifiedValue, bindingContext.ModelMetadata.ModelType, _options);
            bindingContext.Result = ModelBindingResult.Success(deserializedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse JSON into {EnumType}", bindingContext.ModelMetadata.ModelType);
            bindingContext.ModelState.TryAddModelError(modelName, "Unable to convert string to enum");
        }


        return Task.CompletedTask;
    }
}
