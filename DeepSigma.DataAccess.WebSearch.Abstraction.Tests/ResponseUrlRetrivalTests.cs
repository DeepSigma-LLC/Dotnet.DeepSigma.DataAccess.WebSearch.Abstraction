using System.Net;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

namespace DeepSigma.DataAccess.WebSearch.Abstraction.Tests;

public class ResponseUrlRetrivalTests
{
    private static readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    [Fact]
    public void Constructor_RequiredFieldsOnly_SetsProperties()
    {
        var record = new ResponseUrlRetrival(
            Url: "https://example.com",
            Title: "Example",
            Snippet: "A snippet",
            SearchEngine: "Google",
            RetrievedAt: _now);

        Assert.Equal("https://example.com", record.Url);
        Assert.Equal("Example", record.Title);
        Assert.Equal("A snippet", record.Snippet);
        Assert.Equal("Google", record.SearchEngine);
        Assert.Equal(_now, record.RetrievedAt);
    }

    [Fact]
    public void Constructor_OptionalFields_DefaultToNull()
    {
        var record = new ResponseUrlRetrival(
            Url: "https://example.com",
            Title: null,
            Snippet: null,
            SearchEngine: null,
            RetrievedAt: _now);

        Assert.Null(record.ParsedUrls);
        Assert.Null(record.Engines);
        Assert.Null(record.EngineRelevanceScore);
        Assert.Null(record.Category);
        Assert.Null(record.PrettyUrl);
        Assert.Null(record.Template);
        Assert.Null(record.Thumbnail);
        Assert.Null(record.ImageUrl);
        Assert.Null(record.Author);
        Assert.Null(record.IframeSrc);
        Assert.Null(record.PublishedDate);
        Assert.Null(record.ErrorMessage);
    }

    [Fact]
    public void Constructor_ErrorFields_DefaultToFalseAndNull()
    {
        var record = new ResponseUrlRetrival(
            Url: "https://example.com",
            Title: null,
            Snippet: null,
            SearchEngine: null,
            RetrievedAt: _now);

        Assert.False(record.Error);
        Assert.Null(record.ErrorMessage);
    }

    [Fact]
    public void Constructor_WithError_SetsErrorFields()
    {
        var record = new ResponseUrlRetrival(
            Url: "https://example.com",
            Title: null,
            Snippet: null,
            SearchEngine: null,
            RetrievedAt: _now,
            Error: true,
            ErrorMessage: ["Search quota exceeded"]);

        Assert.True(record.Error);
        Assert.Single(record.ErrorMessage!);
        Assert.Equal("Search quota exceeded", record.ErrorMessage![0]);
    }

    [Fact]
    public void Constructor_AllOptionalFields_AreSet()
    {
        var engines = new List<string> { "Google", "Bing" };
        var parsedUrls = new List<string> { "https://a.com", "https://b.com" };
        var published = DateTimeOffset.UtcNow.AddDays(-7);

        var record = new ResponseUrlRetrival(
            Url: "https://example.com",
            Title: "Title",
            Snippet: "Snippet",
            SearchEngine: "Google",
            RetrievedAt: _now,
            ParsedUrls: parsedUrls,
            Engines: engines,
            EngineRelevanceScore: 0.95,
            Category: "Technology",
            PrettyUrl: "example.com",
            Template: "article",
            Thumbnail: "https://example.com/thumb.jpg",
            ImageUrl: "https://example.com/img.jpg",
            Author: "Jane Doe",
            IframeSrc: "https://example.com/embed",
            PublishedDate: published);

        Assert.Equal(parsedUrls, record.ParsedUrls);
        Assert.Equal(engines, record.Engines);
        Assert.Equal(0.95, record.EngineRelevanceScore);
        Assert.Equal("Technology", record.Category);
        Assert.Equal("example.com", record.PrettyUrl);
        Assert.Equal("article", record.Template);
        Assert.Equal("https://example.com/thumb.jpg", record.Thumbnail);
        Assert.Equal("https://example.com/img.jpg", record.ImageUrl);
        Assert.Equal("Jane Doe", record.Author);
        Assert.Equal("https://example.com/embed", record.IframeSrc);
        Assert.Equal(published, record.PublishedDate);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var a = new ResponseUrlRetrival("https://example.com", "Title", "Snippet", "Google", _now);
        var b = new ResponseUrlRetrival("https://example.com", "Title", "Snippet", "Google", _now);

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equality_DifferentUrl_AreNotEqual()
    {
        var a = new ResponseUrlRetrival("https://a.com", "Title", null, null, _now);
        var b = new ResponseUrlRetrival("https://b.com", "Title", null, null, _now);

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void WithOperator_CreatesNewRecordWithChangedField()
    {
        var original = new ResponseUrlRetrival("https://example.com", "Old Title", null, null, _now);
        var updated = original with { Title = "New Title" };

        Assert.Equal("Old Title", original.Title);
        Assert.Equal("New Title", updated.Title);
        Assert.Equal(original.Url, updated.Url);
    }
}
