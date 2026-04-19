using System.Net;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

namespace DeepSigma.DataAccess.WebSearch.Abstraction.Tests;

public class ResponseHtmlContentTests
{
    private static readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    [Fact]
    public void Constructor_RequiredFieldsOnly_SetsProperties()
    {
        var record = new ResponseHtmlContent(
            Url: "https://example.com",
            Html: "<html><body>Hello</body></html>",
            FetchedAt: _now,
            StatusCode: HttpStatusCode.OK);

        Assert.Equal("https://example.com", record.Url);
        Assert.Equal("<html><body>Hello</body></html>", record.Html);
        Assert.Equal(_now, record.FetchedAt);
        Assert.Equal(HttpStatusCode.OK, record.StatusCode);
    }

    [Fact]
    public void Constructor_OptionalFields_DefaultToNull()
    {
        var record = new ResponseHtmlContent(
            Url: "https://example.com",
            Html: "<html/>",
            FetchedAt: _now,
            StatusCode: HttpStatusCode.OK);

        Assert.Null(record.ContentType);
        Assert.Null(record.Title);
        Assert.Null(record.Byline);
        Assert.Null(record.Excerpt);
        Assert.Null(record.Language);
        Assert.Null(record.SourceUrlRetrival);
        Assert.Null(record.ErrorMessage);
    }

    [Fact]
    public void Constructor_ErrorFields_DefaultToFalseAndNull()
    {
        var record = new ResponseHtmlContent(
            Url: "https://example.com",
            Html: "",
            FetchedAt: _now,
            StatusCode: HttpStatusCode.OK);

        Assert.False(record.Error);
        Assert.Null(record.ErrorMessage);
    }

    [Fact]
    public void Constructor_WithError_SetsErrorFields()
    {
        var record = new ResponseHtmlContent(
            Url: "https://example.com",
            Html: "",
            FetchedAt: _now,
            StatusCode: HttpStatusCode.ServiceUnavailable,
            Error: true,
            ErrorMessage: ["Connection timed out", "Retry limit reached"]);

        Assert.True(record.Error);
        Assert.Equal(2, record.ErrorMessage!.Length);
        Assert.Equal("Connection timed out", record.ErrorMessage[0]);
        Assert.Equal("Retry limit reached", record.ErrorMessage[1]);
    }

    [Fact]
    public void Constructor_WithSourceUrlRetrival_PreservesMetadata()
    {
        var source = new ResponseUrlRetrival(
            Url: "https://example.com",
            Title: "Example Page",
            Snippet: "A snippet",
            SearchEngine: "Bing",
            RetrievedAt: _now);

        var record = new ResponseHtmlContent(
            Url: "https://example.com",
            Html: "<html/>",
            FetchedAt: _now,
            StatusCode: HttpStatusCode.OK,
            SourceUrlRetrival: source);

        Assert.Equal(source, record.SourceUrlRetrival);
        Assert.Equal("Example Page", record.SourceUrlRetrival!.Title);
    }

    [Fact]
    public void Constructor_AllOptionalFields_AreSet()
    {
        var record = new ResponseHtmlContent(
            Url: "https://example.com",
            Html: "<html/>",
            FetchedAt: _now,
            StatusCode: HttpStatusCode.OK,
            ContentType: "text/html; charset=utf-8",
            Title: "Page Title",
            Byline: "John Smith",
            Excerpt: "Short excerpt",
            Language: "en");

        Assert.Equal("text/html; charset=utf-8", record.ContentType);
        Assert.Equal("Page Title", record.Title);
        Assert.Equal("John Smith", record.Byline);
        Assert.Equal("Short excerpt", record.Excerpt);
        Assert.Equal("en", record.Language);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var a = new ResponseHtmlContent("https://example.com", "<html/>", _now, HttpStatusCode.OK);
        var b = new ResponseHtmlContent("https://example.com", "<html/>", _now, HttpStatusCode.OK);

        Assert.Equal(a, b);
    }

    [Fact]
    public void Equality_DifferentStatusCode_AreNotEqual()
    {
        var a = new ResponseHtmlContent("https://example.com", "<html/>", _now, HttpStatusCode.OK);
        var b = new ResponseHtmlContent("https://example.com", "<html/>", _now, HttpStatusCode.NotFound);

        Assert.NotEqual(a, b);
    }

    [Fact]
    public void WithOperator_CreatesNewRecordWithChangedField()
    {
        var original = new ResponseHtmlContent("https://example.com", "<html/>", _now, HttpStatusCode.OK);
        var updated = original with { Error = true, ErrorMessage = ["Something went wrong"] };

        Assert.False(original.Error);
        Assert.True(updated.Error);
        Assert.Equal(original.Url, updated.Url);
    }
}
