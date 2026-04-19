# DeepSigma.DataAccess.WebSearch.Abstraction

[![NuGet](https://img.shields.io/nuget/v/DeepSigma.DataAccess.WebSearch.Abstraction)](https://www.nuget.org/packages/DeepSigma.DataAccess.WebSearch.Abstraction)
[![License](https://img.shields.io/github/license/DeepSigma-LLC/Dotnet.DeepSigma.DataAccess.WebSearch.Abstraction)](LICENSE)

A .NET abstraction library that defines contracts (interfaces) and shared models for web search, HTML retrieval, and content extraction. Build provider-specific implementations against these interfaces to keep your application decoupled from any particular search engine, HTTP client, or content-parsing strategy.

---

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [API Reference](#api-reference)
  - [Interfaces](#interfaces)
    - [IUrlRetriver\<TSearchOptions\>](#iurlretrievertsearchoptions)
    - [IHtmlRetriver](#ihtmlretriver)
    - [IContentExtractor](#icontentextractor)
  - [Models](#models)
    - [ResponseUrlRetrival](#responseurlretrival)
    - [ResponseHtmlContent](#responsehtmlcontent)
    - [ResponseExtractedContent](#responseextractedcontent)
- [Usage Examples](#usage-examples)
  - [1. Implementing IUrlRetriver](#1-implementing-iurlretriver)
  - [2. Implementing IHtmlRetriver](#2-implementing-ihtmlretriver)
  - [3. Implementing IContentExtractor](#3-implementing-icontentextractor)
  - [4. End-to-End Pipeline](#4-end-to-end-pipeline)
  - [5. Dependency Injection Registration](#5-dependency-injection-registration)
- [Requirements](#requirements)
- [Contributing](#contributing)
- [License](#license)

---

## Features

- **`IUrlRetriver<TSearchOptions>`** — Search for URLs by query with optional, strongly-typed search options.
- **`IHtmlRetriver`** — Fetch raw HTML content from a URL or a previously retrieved search result.
- **`IContentExtractor`** — Extract structured, meaningful content (title, main text, byline, language, etc.) from HTML.
- **Rich response models** — Immutable `record` types that carry metadata such as status codes, timestamps, relevance scores, thumbnails, and error information.
- **Cancellation support** — All async methods accept an optional `CancellationToken`.
- **Provider-agnostic** — Implement the interfaces with any search engine SDK, HTTP client, or HTML parser.

---

## Installation

Install from NuGet:

```bash
dotnet add package DeepSigma.DataAccess.WebSearch.Abstraction
```

Or via the Package Manager Console:

```powershell
Install-Package DeepSigma.DataAccess.WebSearch.Abstraction
```

---

## Quick Start

```csharp
using DeepSigma.DataAccess.WebSearch.Abstraction;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

// Assume implementations are injected via DI
IUrlRetriver<MySearchOptions> urlRetriver = ...;
IHtmlRetriver htmlRetriver = ...;
IContentExtractor contentExtractor = ...;

// 1. Search for URLs
List<ResponseUrlRetrival> urls = await urlRetriver.SearchAsync("latest .NET 10 features");

// 2. Fetch HTML from the first result
ResponseHtmlContent html = await htmlRetriver.FetchContentAsync(urls.First());

// 3. Extract readable content
ResponseExtractedContent content = await contentExtractor.ExtractedContentAsync(html);

Console.WriteLine($"Title: {content.Title}");
Console.WriteLine($"Text:  {content.MainText}");
```

---

## API Reference

### Interfaces

#### `IUrlRetriver<TSearchOptions>`

Retrieves a list of URLs matching a search query.

| Method | Signature |
|--------|-----------|
| `SearchAsync` | `Task<List<ResponseUrlRetrival>> SearchAsync(string query, TSearchOptions? searchOption = null, CancellationToken? cancellationToken = default)` |

- **`TSearchOptions`** — A class that represents provider-specific search options (e.g., region, safe-search level, result count).
- **`query`** — The search terms.
- **Returns** — A list of `ResponseUrlRetrival` records containing the URL, title, snippet, relevance score, and more.

---

#### `IHtmlRetriver`

Fetches raw HTML content from a URL.

| Method | Signature |
|--------|-----------|
| `FetchContentAsync` | `Task<ResponseHtmlContent> FetchContentAsync(ResponseUrlRetrival responseUrl, CancellationToken? cancellationToken = default)` |
| `FetchContentAsync` | `Task<ResponseHtmlContent> FetchContentAsync(string URL, CancellationToken? cancellationToken = default)` |

Two overloads are provided:
1. Accept a `ResponseUrlRetrival` to carry forward metadata from the search step.
2. Accept a raw URL string for standalone use.

---

#### `IContentExtractor`

Extracts structured content from HTML.

| Method | Signature |
|--------|-----------|
| `ExtractedContentAsync` | `Task<ResponseExtractedContent> ExtractedContentAsync(ResponseHtmlContent htmlContent, CancellationToken? cancellationToken = default)` |
| `ExtractedContentAsync` | `Task<ResponseExtractedContent> ExtractedContentAsync(string html, string? url = null, CancellationToken? cancellationToken = default)` |

Two overloads are provided:
1. Accept a `ResponseHtmlContent` to preserve the full fetch context.
2. Accept a raw HTML string (with an optional URL for context).

---

### Models

#### `ResponseUrlRetrival`

An immutable `record` representing a single search result.

| Property | Type | Description |
|----------|------|-------------|
| `Url` | `string` | The result URL. |
| `Title` | `string?` | Page title. |
| `Snippet` | `string?` | Short description / snippet. |
| `SearchEngine` | `string?` | Source search engine identifier. |
| `RetrievedAt` | `DateTimeOffset` | When the result was retrieved. |
| `ParsedUrls` | `IReadOnlyList<string>?` | Additional URLs parsed from the result. |
| `Engines` | `IReadOnlyList<string>?` | Engines that contributed to this result. |
| `EngineRelevanceScore` | `double?` | Relevance score. |
| `Category` | `string?` | Content category. |
| `PrettyUrl` | `string?` | Human-readable URL. |
| `Template` | `string?` | Template identifier. |
| `Thumbnail` | `string?` | Thumbnail image URL. |
| `ImageUrl` | `string?` | Associated image URL. |
| `Author` | `string?` | Content author. |
| `IframeSrc` | `string?` | Iframe source URL. |
| `PublishedDate` | `DateTimeOffset?` | Content publication date. |

---

#### `ResponseHtmlContent`

An immutable `record` representing fetched HTML page content.

| Property | Type | Description |
|----------|------|-------------|
| `URL` | `string` | The page URL. |
| `HTML` | `string` | Raw HTML markup. |
| `FetchedAt` | `DateTimeOffset` | Fetch timestamp (UTC). |
| `StatusCode` | `HttpStatusCode` | HTTP response status code. |
| `ContentType` | `string?` | MIME type (e.g., `text/html`). |
| `Title` | `string?` | Page title. |
| `Byline` | `string?` | Author / byline. |
| `Excerpt` | `string?` | Page excerpt. |
| `Language` | `string?` | Language code (e.g., `en`). |
| `SourceUrlRetrival` | `ResponseUrlRetrival?` | Original search result metadata. |
| `Error` | `bool` | Whether an error occurred. |
| `ErrorMessage` | `string[]?` | Error details. |

---

#### `ResponseExtractedContent`

An immutable `record` representing content extracted from HTML.

| Property | Type | Description |
|----------|------|-------------|
| `MainText` | `string` | The primary extracted text. |
| `Title` | `string` | Content title. |
| `Language` | `string?` | Language code. |
| `Snippet` | `string?` | Short snippet. |
| `Byline` | `string?` | Author / byline. |
| `Summary` | `string?` | Content summary. |
| `PublishedAt` | `DateTimeOffset?` | Publication date. |
| `ParsedUrls` | `IReadOnlyList<string>?` | URLs found in the content. |
| `Category` | `string?` | Content category. |
| `PrettyUrl` | `string?` | Human-readable URL. |
| `Template` | `string?` | Template identifier. |
| `Thumbnail` | `string?` | Thumbnail image URL. |
| `ImageUrl` | `string?` | Associated image URL. |
| `Author` | `string?` | Author name. |
| `SourceHtmlContent` | `ResponseHtmlContent?` | Original HTML content. |
| `Error` | `bool` | Whether an error occurred. |
| `ErrorMessage` | `string[]?` | Error details. |

---

## Usage Examples

### 1. Implementing `IUrlRetriver`

```csharp
using DeepSigma.DataAccess.WebSearch.Abstraction;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

public class GoogleSearchOptions
{
    public string? Region { get; set; }
    public int MaxResults { get; set; } = 10;
    public bool SafeSearch { get; set; } = true;
}

public class GoogleUrlRetriver : IUrlRetriver<GoogleSearchOptions>
{
    private readonly HttpClient _httpClient;

    public GoogleUrlRetriver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ResponseUrlRetrival>> SearchAsync(
        string query,
        GoogleSearchOptions? searchOption = null,
        CancellationToken? cancellationToken = default)
    {
        var ct = cancellationToken ?? CancellationToken.None;
        var maxResults = searchOption?.MaxResults ?? 10;

        // Call your search API here...
        var apiResults = await CallSearchApiAsync(query, maxResults, ct);

        return apiResults.Select(r => new ResponseUrlRetrival(
            Url: r.Link,
            Title: r.Title,
            Snippet: r.Snippet,
            SearchEngine: "Google",
            RetrievedAt: DateTimeOffset.UtcNow
        )).ToList();
    }

    private Task<List<ApiResult>> CallSearchApiAsync(string query, int max, CancellationToken ct)
    {
        // Your Google Custom Search API logic
        throw new NotImplementedException();
    }
}
```

---

### 2. Implementing `IHtmlRetriver`

```csharp
using System.Net;
using DeepSigma.DataAccess.WebSearch.Abstraction;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

public class HttpClientHtmlRetriver : IHtmlRetriver
{
    private readonly HttpClient _httpClient;

    public HttpClientHtmlRetriver(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResponseHtmlContent> FetchContentAsync(
        ResponseUrlRetrival responseUrl,
        CancellationToken? cancellationToken = default)
    {
        var result = await FetchContentAsync(responseUrl.Url, cancellationToken);
        // Carry forward the source metadata
        return result with { SourceUrlRetrival = responseUrl };
    }

    public async Task<ResponseHtmlContent> FetchContentAsync(
        string URL,
        CancellationToken? cancellationToken = default)
    {
        var ct = cancellationToken ?? CancellationToken.None;

        try
        {
            var response = await _httpClient.GetAsync(URL, ct);
            var html = await response.Content.ReadAsStringAsync(ct);

            return new ResponseHtmlContent(
                URL: URL,
                HTML: html,
                FetchedAt: DateTimeOffset.UtcNow,
                StatusCode: response.StatusCode,
                ContentType: response.Content.Headers.ContentType?.MediaType
            );
        }
        catch (Exception ex)
        {
            return new ResponseHtmlContent(
                URL: URL,
                HTML: string.Empty,
                FetchedAt: DateTimeOffset.UtcNow,
                StatusCode: HttpStatusCode.InternalServerError,
                Error: true,
                ErrorMessage: [ex.Message]
            );
        }
    }
}
```

---

### 3. Implementing `IContentExtractor`

```csharp
using DeepSigma.DataAccess.WebSearch.Abstraction;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

public class SmartReaderContentExtractor : IContentExtractor
{
    public async Task<ResponseExtractedContent> ExtractedContentAsync(
        ResponseHtmlContent htmlContent,
        CancellationToken? cancellationToken = default)
    {
        var result = await ExtractedContentAsync(htmlContent.HTML, htmlContent.URL, cancellationToken);
        return result with { SourceHtmlContent = htmlContent };
    }

    public Task<ResponseExtractedContent> ExtractedContentAsync(
        string html,
        string? url = null,
        CancellationToken? cancellationToken = default)
    {
        // Use SmartReader or any HTML parsing library
        var reader = new SmartReader.Reader(url ?? "https://example.com", html);
        var article = reader.GetArticle();

        var content = new ResponseExtractedContent(
            MainText: article.TextContent ?? string.Empty,
            Title: article.Title ?? string.Empty,
            Byline: article.Byline,
            Language: article.Language,
            Excerpt: article.Excerpt,
            PublishedAt: article.PublicationDate
        );

        return Task.FromResult(content);
    }
}
```

---

### 4. End-to-End Pipeline

```csharp
public class WebSearchPipeline
{
    private readonly IUrlRetriver<GoogleSearchOptions> _urlRetriver;
    private readonly IHtmlRetriver _htmlRetriver;
    private readonly IContentExtractor _contentExtractor;

    public WebSearchPipeline(
        IUrlRetriver<GoogleSearchOptions> urlRetriver,
        IHtmlRetriver htmlRetriver,
        IContentExtractor contentExtractor)
    {
        _urlRetriver = urlRetriver;
        _htmlRetriver = htmlRetriver;
        _contentExtractor = contentExtractor;
    }

    public async Task<List<ResponseExtractedContent>> SearchAndExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        // Step 1: Search
        var urls = await _urlRetriver.SearchAsync(query, cancellationToken: cancellationToken);

        var results = new List<ResponseExtractedContent>();

        foreach (var url in urls)
        {
            // Step 2: Fetch HTML
            var html = await _htmlRetriver.FetchContentAsync(url, cancellationToken);

            if (html.Error)
            {
                results.Add(new ResponseExtractedContent(
                    MainText: string.Empty,
                    Title: url.Title ?? string.Empty,
                    Error: true,
                    ErrorMessage: html.ErrorMessage
                ));
                continue;
            }

            // Step 3: Extract content
            var content = await _contentExtractor.ExtractedContentAsync(html, cancellationToken);
            results.Add(content);
        }

        return results;
    }
}
```

---

### 5. Dependency Injection Registration

```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Register your implementations
services.AddHttpClient<HttpClientHtmlRetriver>();
services.AddSingleton<IUrlRetriver<GoogleSearchOptions>, GoogleUrlRetriver>();
services.AddSingleton<IHtmlRetriver, HttpClientHtmlRetriver>();
services.AddSingleton<IContentExtractor, SmartReaderContentExtractor>();

// Register the pipeline
services.AddTransient<WebSearchPipeline>();

var provider = services.BuildServiceProvider();

// Resolve and use
var pipeline = provider.GetRequiredService<WebSearchPipeline>();
var results = await pipeline.SearchAndExtractAsync("C# web scraping best practices");

foreach (var result in results)
{
    Console.WriteLine($"## {result.Title}");
    Console.WriteLine(result.MainText[..Math.Min(200, result.MainText.Length)] + "...");
    Console.WriteLine();
}
```

---

## Requirements

- **.NET 10** or later
- No external dependencies — this is a pure abstraction library

---

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on [GitHub](https://github.com/DeepSigma-LLC/Dotnet.DeepSigma.DataAccess.WebSearch.Abstraction).

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-feature`)
3. Commit your changes (`git commit -am 'Add my feature'`)
4. Push to the branch (`git push origin feature/my-feature`)
5. Open a Pull Request

---

## License

This project is maintained by [DeepSigma LLC](https://github.com/DeepSigma-LLC). See the [LICENSE](LICENSE) file for details.
