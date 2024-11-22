using System.Threading;
using Microsoft.CodeAnalysis.Text;

namespace TSMoreland.Text.Json.NamingStrategies.SourceGenerator;

[Generator]
public sealed class Generator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            foreach (var (filename, content) in GetSourceFiles())
            {
                context.AddSource(filename, SourceText.From(content, Encoding.UTF8));
            }
        });
    }
    private static IEnumerable<SourceFile> GetSourceFiles()
    {
        yield return StringExtensionsSourceFactory.Build();
        yield return CaseSeparatedEnumNamingStrategyFactory.Build("SnakeCase", "ToSnakeCase");
        yield return CaseSeparatedEnumNamingStrategyFactory.Build("KebabCase", "ToKebabCase");
    }

    private class AdditionalSourceFile(string filename, string content) : AdditionalText
    {
        /// <inheritdoc />
        public override SourceText? GetText(CancellationToken cancellationToken = default) =>
            SourceText.From(content, Encoding.UTF8);

        /// <inheritdoc />
        public override string Path { get; } = filename;
    }
}
