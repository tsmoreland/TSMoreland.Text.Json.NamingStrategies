using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using TSMoreland.Text.Json.NamingStrategies;
using TSMoreland.Text.Json.NamingStrategies.Strategies;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.Services
    .AddProblemDetails()
    .AddControllers(options =>
    {
        options.RespectBrowserAcceptHeader = true;
        options.ModelBinderProviders.Insert(0, new EnumModelBinderProvider());

        var jsonInputFormatter = options.InputFormatters?.OfType<SystemTextJsonInputFormatter>().FirstOrDefault();
        if (jsonInputFormatter is not null && jsonInputFormatter.SupportedMediaTypes.Contains("text/json"))
        {
            jsonInputFormatter.SupportedMediaTypes.Remove("text/json");
        }

        var jsonOutputFormatter = options.OutputFormatters?.OfType<SystemTextJsonOutputFormatter>().FirstOrDefault();
        if (jsonOutputFormatter is not null && jsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
        {
            jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
        }


    })
    .AddXmlSerializerFormatters()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonStrategizedNamingPolicy.SnakeCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStrategizedStringEnumConverterFactory(new SnakeCaseEnumNamingStrategy()));
    });

builder.Services.AddOpenApi();

var app = builder.Build();

app.Use((HttpContext context, RequestDelegate next) =>
{
    var headers = context.Response.Headers;
    headers.Append("Cache-Control", new StringValues("no-store"));
    headers.Append("X-Content-Type-Options", new StringValues("nosniff"));
    headers.Append("X-Frame-Options", new StringValues("DENY"));
    headers.Append("x-xss-protection", new StringValues("1; mode=block"));
    headers.Append("Expect-CT", new StringValues("max-age=0, enforce"));
    headers.Append("referrer-policy", new StringValues("strict-origin-when-cross-origin"));
    headers.Append("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
    return next(context);
});

app.UseExceptionHandler();
app.UseStatusCodePages();
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

#if !NET9_0_OR_GREATER
app.UseSwagger();
#endif
app.UseSwaggerUI(options =>
{
    options.DisplayOperationId();
    options.DisplayRequestDuration();
});

app.UseAuthorization();
app.MapControllers();
app.Run();
