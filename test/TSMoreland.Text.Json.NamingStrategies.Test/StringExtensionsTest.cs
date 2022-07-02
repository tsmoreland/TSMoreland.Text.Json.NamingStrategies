namespace TSMoreland.Text.Json.NamingStrategies.Test;

public class StringExtensionsTest
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   \t   ")]
    public void ToSnakeCase_ReturnsEmpty_WhenSourceIsNullOrWhitespace(string source)
    {
        string actual = source.ToSnakeCase();
        actual.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   \t   ")]
    public void ToKebabCase_ReturnsEmpty_WhenSourceIsNullOrWhitespace(string source)
    {
        string actual = source.ToKebabCase();
        actual.Should().BeNullOrEmpty();
    }

    [Theory]
    [MemberData(nameof(SnakeCaseData))]
    public void ToSnakeCase_ReturnsSnakeCaseString_WhenSourceIsNotNullOrEmpty(string source, string expected)
    {
        string actual = source.ToSnakeCase();

        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(KebabCaseData))]
    public void ToKebabCase_ReturnsKebabCaseString_WhenSourceIsNotNullOrEmpty(string source, string expected)
    {
        string actual = source.ToKebabCase();

        actual.Should().Be(expected);
    }

    private static IEnumerable<object[]> SnakeCaseData()
    {
        yield return new object[] { "FooBar", "foo_bar" };
        yield return new object[] { "fooBar", "foo_bar" };
        yield return new object[] { "FOO_BAR", "foo_bar" };
        yield return new object[] { "foo_bar", "foo_bar" };
        yield return new object[] { "foo bar", "foo_bar" };
        yield return new object[] { "FOOBar", "foo_bar" };
        yield return new object[] { "  Alpha1_Bravo2_3Charlie  ", "alpha1_bravo2_3_charlie" };
        yield return new object[] { "  Alpha1_Bravo2_Charlie3  ", "alpha1_bravo2_charlie3" };
    }

    private static IEnumerable<object[]> KebabCaseData()
    {
        yield return new object[] { "FooBar", "foo-bar" };
        yield return new object[] { "fooBar", "foo-bar" };
        yield return new object[] { "FOO_BAR", "foo-bar" };
        yield return new object[] { "foo_bar", "foo-bar" };
        yield return new object[] { "foo bar", "foo-bar" };
        yield return new object[] { "FOOBar", "foo-bar" };
        yield return new object[] { "  Alpha1_Bravo2_3Charlie  ", "alpha1-bravo2-3-charlie" };
        yield return new object[] { "  Alpha1_Bravo2_Charlie3  ", "alpha1-bravo2-charlie3" };
    }
}
