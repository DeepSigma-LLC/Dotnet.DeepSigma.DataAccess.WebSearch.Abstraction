using System.Net;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

namespace DeepSigma.DataAccess.WebSearch.Abstraction.Tests;

public class ResponseExtractedContentTests
{
    private static readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    private static ResponseUrlRetrival CreateUrlRetrival(string url = "https://example.com") =>
        new(Url: url, Title: null, Snippet: null, SearchEngine: null, RetrievedAt: DateTimeOffset.UtcNow);

    private static ResponseHtmlContent CreateHtmlContent() =>
        new(Html: "<html/>", FetchedAt: DateTimeOffset.UtcNow, StatusCode: HttpStatusCode.OK);

    [Fact]
    public void Constructor_RequiredFieldsOnly_SetsProperties()
    {
        var record = new ResponseExtractedContent(
            SourceUrlRetrival: CreateUrlRetrival(),
            SourceHtmlContent: CreateHtmlContent(),
            MainText: "Hello world",
            Title: "My Article");

        Assert.Equal("Hello world", record.MainText);
        Assert.Equal("My Article", record.Title);
    }

    [Fact]
    public void Constructor_OptionalFields_DefaultToNull()
    {
        var record = new ResponseExtractedContent(
            SourceUrlRetrival: CreateUrlRetrival(),
            SourceHtmlContent: CreateHtmlContent(),
            MainText: "Text",
            Title: "Title");

        Assert.Null(record.Language);
        Assert.Null(record.Snippet);
        Assert.Null(record.Byline);
        Assert.Null(record.Summary);
        Assert.Null(record.PublishedAt);
        Assert.Null(record.ParsedUrls);
        Assert.Null(record.Category);
        Assert.Null(record.PrettyUrl);
        Assert.Null(record.Template);
        Assert.Null(record.Thumbnail);
        Assert.Null(record.ImageUrl);
        Assert.Null(record.Author);
        Assert.Null(record.ErrorMessage);
    }

    [Fact]
    public void Constructor_ErrorFields_DefaultToFalseAndNull()
    {
        var record = new ResponseExtractedContent(
            SourceUrlRetrival: CreateUrlRetrival(),
            SourceHtmlContent: CreateHtmlContent(),
            MainText: "Text",
            Title: "Title");

        Assert.False(record.Error);
        Assert.Null(record.ErrorMessage);
    }

    [Fact]
    public void Constructor_WithError_SetsErrorFields()
    {
        var record = new ResponseExtractedContent(
            SourceUrlRetrival: CreateUrlRetrival(),
            SourceHtmlContent: CreateHtmlContent(),
            MainText: "",
            Title: "",
            Error: true,
            ErrorMessage: ["Parsing failed", "Unsupported encoding"]);

        Assert.True(record.Error);
        Assert.Equal(2, record.ErrorMessage!.Length);
        Assert.Equal("Parsing failed", record.ErrorMessage[0]);
        Assert.Equal("Unsupported encoding", record.ErrorMessage[1]);
    }

    [Fact]
    public void Constructor_WithSourceHtmlContent_PreservesChain()
    {
        var urlRetrival = CreateUrlRetrival("https://example.com");
        var html = new ResponseHtmlContent(
            Html: "<html/>",
            FetchedAt: _now,
            StatusCode: HttpStatusCode.OK);

        var record = new ResponseExtractedContent(
            SourceUrlRetrival: urlRetrival,
            SourceHtmlContent: html,
            MainText: "Extracted text",
            Title: "Article");

        Assert.Equal(html, record.SourceHtmlContent);
        Assert.Equal("https://example.com", record.SourceUrlRetrival.Url);
    }

    [Fact]
    public void Constructor_AllOptionalFields_AreSet()
    {
        var published = _now.AddDays(-3);
        var parsedUrls = new List<string> { "https://ref1.com", "https://ref2.com" };

        var record = new ResponseExtractedContent(
            SourceUrlRetrival: CreateUrlRetrival(),
            SourceHtmlContent: CreateHtmlContent(),
            MainText: "Full article text",
            Title: "Article Title",
            Language: "en",
            Snippet: "A short snippet",
            Byline: "Jane Doe",
            Summary: "A summary of the article",
            PublishedAt: published,
            ParsedUrls: parsedUrls,
            Category: "Technology",
            PrettyUrl: "example.com/article",
            Template: "article",
            Thumbnail: "https://example.com/thumb.jpg",
            ImageUrl: "https://example.com/img.jpg",
            Author: "Jane Doe");

        Assert.Equal("en", record.Language);
        Assert.Equal("A short snippet", record.Snippet);
        Assert.Equal("Jane Doe", record.Byline);
        Assert.Equal("A summary of the article", record.Summary);
        Assert.Equal(published, record.PublishedAt);
        Assert.Equal(parsedUrls, record.ParsedUrls);
        Assert.Equal("Technology", record.Category);
        Assert.Equal("example.com/article", record.PrettyUrl);
        Assert.Equal("article", record.Template);
        Assert.Equal("https://example.com/thumb.jpg", record.Thumbnail);
        Assert.Equal("https://example.com/img.jpg", record.ImageUrl);
        Assert.Equal("Jane Doe", record.Author);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var a = new ResponseExtractedContent(
            SourceUrlRetrival: CreateUrlRetrival(),
            SourceHtmlContent: CreateHtmlContent(),
            MainText: "Text",
            Title: "Title");
        var b = a with { };

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equality_DifferentMainText_AreNotEqual()
    {
        var urlRetrival = CreateUrlRetrival();
        var html = CreateHtmlContent();
        var a = new ResponseExtractedContent(SourceUrlRetrival: urlRetrival, SourceHtmlContent: html, MainText: "Text A", Title: "Title");
        var b = new ResponseExtractedContent(SourceUrlRetrival: urlRetrival, SourceHtmlContent: html, MainText: "Text B", Title: "Title");

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void WithOperator_CreatesNewRecordWithChangedField()
    {
        var original = new ResponseExtractedContent(
            SourceUrlRetrival: CreateUrlRetrival(),
            SourceHtmlContent: CreateHtmlContent(),
            MainText: "Original",
            Title: "Title");
        var updated = original with { MainText = "Updated" };

        Assert.Equal("Original", original.MainText);
        Assert.Equal("Updated", updated.MainText);
        Assert.Equal(original.Title, updated.Title);
    }
}
