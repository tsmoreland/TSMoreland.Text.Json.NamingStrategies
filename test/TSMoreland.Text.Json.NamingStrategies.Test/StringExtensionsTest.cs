namespace TSMoreland.Text.Json.NamingStrategies.Test;

public class StringExtensionsTest
{
    [Theory]
    [MemberData(nameof(CaseData))]
    public void ToSnakeCase_ReturnsSnakeCaseString_WhenSourceIsNotNullOrEmpty(string source, string expected)
    {
        string actual = source.ToSnakeCase();

        actual.Should().Be(expected);
    }

    private static IEnumerable<object[]> CaseData()
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
}
