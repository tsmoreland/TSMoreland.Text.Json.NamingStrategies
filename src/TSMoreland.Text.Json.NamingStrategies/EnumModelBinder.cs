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
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        _options = options.Value.JsonSerializerOptions;
    }

    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null!)
        {
            return Task.FromException(new ArgumentNullException(nameof(bindingContext)));
        }

        var modelName = bindingContext.ModelName;
        var providerResult = bindingContext.ValueProvider.GetValue(modelName);
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
            if (char.IsNumber(value[0])) // content is a number, let it be deserialized as such
            {
                bindingContext.Result = ModelBindingResult.Success(JsonSerializer.Deserialize(value, bindingContext.ModelMetadata.ModelType, _options));
                return Task.CompletedTask;
            }

            var jsonifiedValue = string.Create(value.Length + 2, value, static (output, state) =>
            {
                var input = state.AsSpan();
                output[0] = '"';
                int i;
                for (i = 0; i < input.Length; i++)
                {
                    output[i + 1] = input[i];
                }
                output[i + 1] = '"';
            });

            var deserializedValue = JsonSerializer.Deserialize(jsonifiedValue, bindingContext.ModelMetadata.ModelType, _options);
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
