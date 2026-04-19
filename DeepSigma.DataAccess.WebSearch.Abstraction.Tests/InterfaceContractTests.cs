using System.Net;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

namespace DeepSigma.DataAccess.WebSearch.Abstraction.Tests;

/// <summary>
/// Verifies that stub implementations satisfy the interface contracts.
/// These tests exist to catch compile-time regressions in the interface signatures
/// and to confirm that the contracts are straightforwardly implementable.
/// </summary>
public class InterfaceContractTests
{
    // --- Stub implementations ---

    private class StubSearchOptions
    {
        public int MaxResults { get; set; } = 10;
    }

    private class StubUrlRetriever : IUrlRetriever<StubSearchOptions>
    {
        public Task<List<ResponseUrlRetrival>> SearchAsync(
            string query,
            StubSearchOptions? searchOption = null,
            CancellationToken cancellationToken = default)
        {
            var results = new List<ResponseUrlRetrival>
            {
                new ResponseUrlRetrival(
                    Url: $"https://example.com/search?q={query}",
                    Title: "Stub Result",
                    Snippet: "A stub snippet",
                    SearchEngine: "Stub",
                    RetrievedAt: DateTimeOffset.UtcNow)
            };
            return Task.FromResult(results);
        }
    }

    private class StubHtmlRetriever : IHtmlRetriever
    {
        public Task<ResponseHtmlContent> FetchContentAsync(
            ResponseUrlRetrival responseUrl,
            CancellationToken cancellationToken = default)
        {
            var result = new ResponseHtmlContent(
                Url: responseUrl.Url,
                Html: "<html><body>Stub HTML</body></html>",
                FetchedAt: DateTimeOffset.UtcNow,
                StatusCode: HttpStatusCode.OK,
                SourceUrlRetrival: responseUrl);
            return Task.FromResult(result);
        }

        public Task<ResponseHtmlContent> FetchContentAsync(
            string url,
            CancellationToken cancellationToken = default)
        {
            var result = new ResponseHtmlContent(
                Url: url,
                Html: "<html><body>Stub HTML</body></html>",
                FetchedAt: DateTimeOffset.UtcNow,
                StatusCode: HttpStatusCode.OK);
            return Task.FromResult(result);
        }
    }

    private class StubContentExtractor : IContentExtractor
    {
        public Task<ResponseExtractedContent> ExtractContentAsync(
            ResponseHtmlContent htmlContent,
            CancellationToken cancellationToken = default)
        {
            var result = new ResponseExtractedContent(
                MainText: "Extracted text",
                Title: htmlContent.Title ?? "Untitled",
                SourceHtmlContent: htmlContent);
            return Task.FromResult(result);
        }

        public Task<ResponseExtractedContent> ExtractContentAsync(
            string html,
            string? url = null,
            CancellationToken cancellationToken = default)
        {
            var result = new ResponseExtractedContent(
                MainText: "Extracted text",
                Title: "Untitled");
            return Task.FromResult(result);
        }
    }

    // --- Tests ---

    [Fact]
    public async Task IUrlRetriever_SearchAsync_ReturnsResults()
    {
        IUrlRetriever<StubSearchOptions> retriever = new StubUrlRetriever();

        var results = await retriever.SearchAsync("test query");

        Assert.NotEmpty(results);
        Assert.Contains("test query", results[0].Url);
        Assert.Equal("Stub", results[0].SearchEngine);
    }

    [Fact]
    public async Task IUrlRetriever_SearchAsync_WithOptions_UsesOptions()
    {
        IUrlRetriever<StubSearchOptions> retriever = new StubUrlRetriever();
        var options = new StubSearchOptions { MaxResults = 5 };

        var results = await retriever.SearchAsync("test", options);

        Assert.NotEmpty(results);
    }

    [Fact]
    public async Task IUrlRetriever_SearchAsync_SupportsCancellation()
    {
        IUrlRetriever<StubSearchOptions> retriever = new StubUrlRetriever();
        using var cts = new CancellationTokenSource();

        var results = await retriever.SearchAsync("test", cancellationToken: cts.Token);

        Assert.NotEmpty(results);
    }

    [Fact]
    public async Task IHtmlRetriever_FetchContentAsync_FromUrlRetrival_PreservesSource()
    {
        IHtmlRetriever retriever = new StubHtmlRetriever();
        var source = new ResponseUrlRetrival(
            Url: "https://example.com",
            Title: "Example",
            Snippet: null,
            SearchEngine: "Stub",
            RetrievedAt: DateTimeOffset.UtcNow);

        var result = await retriever.FetchContentAsync(source);

        Assert.Equal("https://example.com", result.Url);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(source, result.SourceUrlRetrival);
    }

    [Fact]
    public async Task IHtmlRetriever_FetchContentAsync_FromString_ReturnsHtml()
    {
        IHtmlRetriever retriever = new StubHtmlRetriever();

        var result = await retriever.FetchContentAsync("https://example.com");

        Assert.Equal("https://example.com", result.Url);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.False(result.Error);
        Assert.NotEmpty(result.Html);
    }

    [Fact]
    public async Task IContentExtractor_ExtractContentAsync_FromHtmlContent_PreservesSource()
    {
        IContentExtractor extractor = new StubContentExtractor();
        var html = new ResponseHtmlContent(
            Url: "https://example.com",
            Html: "<html><body>Article body</body></html>",
            FetchedAt: DateTimeOffset.UtcNow,
            StatusCode: HttpStatusCode.OK,
            Title: "Article Title");

        var result = await extractor.ExtractContentAsync(html);

        Assert.Equal("Extracted text", result.MainText);
        Assert.Equal("Article Title", result.Title);
        Assert.Equal(html, result.SourceHtmlContent);
    }

    [Fact]
    public async Task IContentExtractor_ExtractContentAsync_FromString_ReturnsContent()
    {
        IContentExtractor extractor = new StubContentExtractor();

        var result = await extractor.ExtractContentAsync("<html><body>Hello</body></html>", "https://example.com");

        Assert.Equal("Extracted text", result.MainText);
        Assert.False(result.Error);
    }

    [Fact]
    public async Task Pipeline_SearchFetchExtract_ProducesFullChain()
    {
        IUrlRetriever<StubSearchOptions> urlRetriever = new StubUrlRetriever();
        IHtmlRetriever htmlRetriever = new StubHtmlRetriever();
        IContentExtractor contentExtractor = new StubContentExtractor();

        var urls = await urlRetriever.SearchAsync("test");
        var html = await htmlRetriever.FetchContentAsync(urls.First());
        var content = await contentExtractor.ExtractContentAsync(html);

        // Full chain of metadata is preserved
        Assert.NotNull(content.SourceHtmlContent);
        Assert.NotNull(content.SourceHtmlContent!.SourceUrlRetrival);
        Assert.Equal(urls.First().Url, content.SourceHtmlContent.Url);
    }
}
