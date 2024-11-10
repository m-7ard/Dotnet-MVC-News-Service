using MVC_News.MVC.Services;

namespace MVC_News.Tests.UnitTests.MVC.Services;

public class MarkupParserTests
{
    [Fact]
    public void ParseToHtml_EmptyString_ReturnsEmptyString()
    {
        // Arrange
        string input = string.Empty;

        // Act
        var result = MarkupParser.ParseToHtml(input);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ParseToHtml_NullString_ReturnsEmptyString()
    {
        // Arrange
        string? input = null;

        // Act
        var result = MarkupParser.ParseToHtml(input);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ParseToHtml_BoldTags_ReturnsHtmlBold()
    {
        // Arrange
        string input = "This is [bold]bold[/bold] text.";

        // Act
        var result = MarkupParser.ParseToHtml(input);

        // Assert
        Assert.Equal("This is <strong>bold</strong> text.", result);
    }

    [Fact]
    public void ParseToHtml_ItalicTags_ReturnsHtmlItalic()
    {
        // Arrange
        string input = "This is [italic]italic[/italic] text.";

        // Act
        var result = MarkupParser.ParseToHtml(input);

        // Assert
        Assert.Equal("This is <em>italic</em> text.", result);
    }

    [Fact]
    public void ParseToHtml_CombinedTags_ReturnsCorrectHtml()
    {
        // Arrange
        string input = "[bold]Bold text with [italic]italic[/italic] text.[/bold]";

        // Act
        var result = MarkupParser.ParseToHtml(input);

        // Assert
        Assert.Equal("<strong>Bold text with <em>italic</em> text.</strong>", result);
    }

    [Fact]
    public void ParseToHtml_ImageTag_ReturnsHtmlImage()
    {
        // Arrange
        string input = "[image url=http://example.com/image.png caption=An image]";

        // Act
        var result = MarkupParser.ParseToHtml(input);

        // Assert
        Assert.Equal("<figure><img src=\"http://example.com/image.png\" alt=\"An image\" /><figcaption>An image</figcaption></figure>", result);
    }

    [Fact]
    public void ParseToHtml_ComplexInput_ReturnsCorrectHtml()
    {
        // Arrange
        string input = "Check this [bold]bold text[/bold] and this [image url=http://example.com/image.png caption=Image Caption].";

        // Act
        var result = MarkupParser.ParseToHtml(input);

        // Assert
        var expected = "Check this <strong>bold text</strong> and this <figure><img src=\"http://example.com/image.png\" alt=\"Image Caption\" /><figcaption>Image Caption</figcaption></figure>.";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ParseToHtml_ScriptTag_ReturnsEncodedString()
    {
        // Arrange
        string input = "<script>alert('XSS');</script>";

        // Act
        var result = MarkupParser.ParseToHtml(input);

        // Assert
        Assert.Equal("&lt;script&gt;alert(&#39;XSS&#39;);&lt;/script&gt;", result);
    }
}