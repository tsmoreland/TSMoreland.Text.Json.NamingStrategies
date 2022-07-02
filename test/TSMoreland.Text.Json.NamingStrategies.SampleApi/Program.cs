using Hellang.Middleware.ProblemDetails;
using TSMoreland.Text.Json.NamingStrategies;
using TSMoreland.Text.Json.NamingStrategies.Strategies;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddProblemDetails()
    .AddScoped<IEnumNamingStrategy, SnakeCaseEnumNamingStrategy>()
    .AddControllers(options =>
    {
        options.RespectBrowserAcceptHeader = true;
        options.ModelBinderProviders.Insert(0, new EnumModelBinderProvider());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStrategizedStringEnumConverterFactory(new SnakeCaseEnumNamingStrategy()));
    });

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

WebApplication app = builder.Build();

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
