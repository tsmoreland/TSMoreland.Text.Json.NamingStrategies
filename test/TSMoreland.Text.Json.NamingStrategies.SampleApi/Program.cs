using System.Net.Mime;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Primitives;
using TSMoreland.Text.Json.NamingStrategies;
using TSMoreland.Text.Json.NamingStrategies.Strategies;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.Services
    .AddProblemDetails()
    .AddControllers(options =>
    {
        options.RespectBrowserAcceptHeader = true;
        options.ModelBinderProviders.Insert(0, new EnumModelBinderProvider());

        SystemTextJsonInputFormatter? jsonInputFormatter = options.InputFormatters?.OfType<SystemTextJsonInputFormatter>().FirstOrDefault();
        if (jsonInputFormatter is not null && jsonInputFormatter.SupportedMediaTypes.Contains("text/json"))
        {
            jsonInputFormatter.SupportedMediaTypes.Remove("text/json");
        }

        SystemTextJsonOutputFormatter? jsonOutputFormatter = options.OutputFormatters?.OfType<SystemTextJsonOutputFormatter>().FirstOrDefault();
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

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

WebApplication app = builder.Build();

app.Use((context, next) =>
{
    IHeaderDictionary headers = context.Response.Headers;
    headers.Add("Cache-Control", new StringValues("no-store"));
    headers.Add("X-Content-Type-Options", new StringValues("nosniff"));
    headers.Add("X-Frame-Options", new StringValues("DENY"));
    headers.Add("x-xss-protection", new StringValues("1; mode=block"));
    headers.Add("Expect-CT", new StringValues("max-age=0, enforce"));
    headers.Add("referrer-policy", new StringValues("strict-origin-when-cross-origin"));
    headers.Add("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
    return next();
});

app.UseProblemDetails();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DisplayOperationId();
    options.DisplayRequestDuration();
});
app.UseAuthorization();

app.MapControllers();

app.Run();
