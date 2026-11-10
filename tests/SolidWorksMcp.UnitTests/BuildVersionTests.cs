using SolidWorksMcp.Core;
using Xunit;

namespace SolidWorksMcp.UnitTests;

public class BuildVersionTests
{
    [Theory]
    [InlineData("0.1.0-sw2023-20261110-064352+b00311be42f66b912ebd198ce4ab6453cd5e218f", "0.1.0-sw2023-20261110-064352+b00311b")]
    [InlineData("0.1.0+f38318c0295de25406f603f24e0c8dd1a4345ed3.dirty", "0.1.0+f38318c.dirty")]
    [InlineData("0.1.0+b00311b", "0.1.0+b00311b")]
    [InlineData("0.1.0", "0.1.0")]
    [InlineData("0.1.0+", "0.1.0")]
    [InlineData("0.1.0+local.build", "0.1.0+local.build")]
    [InlineData(null, "0.0.0")]
    [InlineData("", "0.0.0")]
    public void The_commit_stays_in_the_version_shortened_to_seven(string? informational, string expected) =>
        Assert.Equal(expected, BuildVersion.Format(informational));
}
