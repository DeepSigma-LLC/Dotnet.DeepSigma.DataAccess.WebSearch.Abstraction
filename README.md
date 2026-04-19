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
    - [IUrlRetriever\<TSearchOptions\>](#iurlretrievertsearchoptions)
    - [IHtmlRetriever](#ihtmlretriever)
    - [IContentExtractor](#icontentextractor)
  - [Models](#models)
    - [ResponseUrlRetrival](#responseurlretrival)
    - [ResponseHtmlContent](#responsehtmlcontent)
    - [ResponseExtractedContent](#responseextractedcontent)
- [Usage Examples](#usage-examples)
  - [1. Implementing IUrlRetriever](#1-implementing-iurlretriever)
  - [2. Implementing IHtmlRetriever](#2-implementing-ihtmlretriever)
  - [3. Implementing IContentExtractor](#3-implementing-icontentextractor)
  - [4. End-to-End Pipeline](#4-end-to-end-pipeline)
  - [5. Dependency Injection Registration](#5-dependency-injection-registration)
- [Requirements](#requirements)
- [Contributing](#contributing)
- [License](#license)

---

## Features

- **`IUrlRetriever<TSearchOptions>`** — Search for URLs by query with optional, strongly-typed search options.
- **`IHtmlRetriever`** — Fetch raw HTML content from a previously retrieved search result.
- **`IContentExtractor`** — Extract structured, meaningful content (title, main text, byline, language, etc.) from HTML.
- **Rich response models** — Immutable `record` types that carry metadata such as status codes, timestamps, relevance scores, thumbnails, and error information.
- **Cancellation support** — All async methods accept a `CancellationToken` (defaults to `CancellationToken.None`).
- **Consistent error handling** — Every response model exposes `Error` and `ErrorMessage` fields for uniform error propagation across the pipeline.
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
IUrlRetriever<MySearchOptions> urlRetriever = ...;
IHtmlRetriever htmlRetriever = ...;
IContentExtractor contentExtractor = ...;

// 1. Search for URLs
List<ResponseUrlRetrival> urls = await urlRetriever.SearchAsync("latest .NET 10 features");

// 2. Fetch HTML from the first result
ResponseHtmlContent html = await htmlRetriever.FetchContentAsync(urls.First());

// 3. Extract readable content
ResponseExtractedContent content = await contentExtractor.ExtractContentAsync(html);

Console.WriteLine($"Title: {content.Title}");
Console.WriteLine($"Text:  {content.MainText}");
```

---

## API Reference

### Interfaces

#### `IUrlRetriever<TSearchOptions>`

Retrieves a list of URLs matching a search query.

| Method | Signature |
|--------|-----------|
| `SearchAsync` | `Task<List<ResponseUrlRetrival>> SearchAsync(string query, TSearchOptions? searchOption = null, CancellationToken cancellationToken = default)` |

- **`TSearchOptions`** — A class that represents provider-specific search options (e.g., region, safe-search level, result count).
- **`query`** — The search terms.
- **Returns** — A list of `ResponseUrlRetrival` records containing the URL, title, snippet, relevance score, and more.

---

#### `IHtmlRetriever`

Fetches raw HTML content from a previously retrieved search result.

| Method | Signature |
|--------|-----------|
| `FetchContentAsync` | `Task<ResponseHtmlContent> FetchContentAsync(ResponseUrlRetrival responseUrl, CancellationToken cancellationToken = default)` |

- **`responseUrl`** — A `ResponseUrlRetrival` carrying forward metadata from the search step.
- **Returns** — A `ResponseHtmlContent` record containing the raw HTML, status code, fetch timestamp, and optional metadata.

---

#### `IContentExtractor`

Extracts structured content from HTML.

| Method | Signature |
|--------|-----------|
| `ExtractContentAsync` | `Task<ResponseExtractedContent> ExtractContentAsync(ResponseHtmlContent htmlContent, CancellationToken cancellationToken = default)` |

- **`htmlContent`** — A `ResponseHtmlContent` record preserving the full fetch context.
- **Returns** — A `ResponseExtractedContent` record containing the extracted text, title, metadata, and references back to the source HTML and URL retrieval.

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
| `Error` | `bool` | Whether an error occurred. Defaults to `false`. |
| `ErrorMessage` | `string[]?` | Error details. Defaults to `null`. |
| `AdditionalData` | `Dictionary<string, object>` | Extension data for custom properties not covered by the typed fields. |

---

#### `ResponseHtmlContent`

An immutable `record` representing fetched HTML page content.

| Property | Type | Description |
|----------|------|-------------|
| `Html` | `string` | Raw HTML markup. |
| `FetchedAt` | `DateTimeOffset` | Fetch timestamp (UTC). |
| `StatusCode` | `HttpStatusCode` | HTTP response status code. |
| `ContentType` | `string?` | MIME type (e.g., `text/html`). |
| `Title` | `string?` | Page title. |
| `Excerpt` | `string?` | Page excerpt. |
| `Language` | `string?` | Language code (e.g., `en`). |
| `Error` | `bool` | Whether an error occurred. Defaults to `false`. |
| `ErrorMessage` | `string[]?` | Error details. Defaults to `null`. |

---

#### `ResponseExtractedContent`

An immutable `record` representing content extracted from HTML.

| Property | Type | Description |
|----------|------|-------------|
| `SourceUrlRetrival` | `ResponseUrlRetrival` | **(Required)** The source URL retrieval information associated with the extracted content. |
| `SourceHtmlContent` | `ResponseHtmlContent` | **(Required)** The source HTML content from which the content was extracted. |
| `MainText` | `string` | **(Required)** The primary extracted text. |
| `Title` | `string` | **(Required)** Content title. |
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
| `Error` | `bool` | Whether an error occurred. Defaults to `false`. |
| `ErrorMessage` | `string[]?` | Error details. Defaults to `null`. |
| `AdditionalData` | `Dictionary<string, object>` | Extension data for custom properties not covered by the typed fields. |

---

## Usage Examples

### 1. Implementing `IUrlRetriever`

```csharp
using DeepSigma.DataAccess.WebSearch.Abstraction;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

public class GoogleSearchOptions
{
    public string? Region { get; set; }
    public int MaxResults { get; set; } = 10;
    public bool SafeSearch { get; set; } = true;
}

public class GoogleUrlRetriever : IUrlRetriever<GoogleSearchOptions>
{
    private readonly HttpClient _httpClient;

    public GoogleUrlRetriever(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ResponseUrlRetrival>> SearchAsync(
        string query,
        GoogleSearchOptions? searchOption = null,
        CancellationToken cancellationToken = default)
    {
        var maxResults = searchOption?.MaxResults ?? 10;

        // Call your search API here...
        var apiResults = await CallSearchApiAsync(query, maxResults, cancellationToken);

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

### 2. Implementing `IHtmlRetriever`

```csharp
using System.Net;
using DeepSigma.DataAccess.WebSearch.Abstraction;
using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

public class HttpClientHtmlRetriever : IHtmlRetriever
{
    private readonly HttpClient _httpClient;

    public HttpClientHtmlRetriever(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResponseHtmlContent> FetchContentAsync(
        ResponseUrlRetrival responseUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(responseUrl.Url, cancellationToken);
            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            return new ResponseHtmlContent(
                Html: html,
                FetchedAt: DateTimeOffset.UtcNow,
                StatusCode: response.StatusCode,
                ContentType: response.Content.Headers.ContentType?.MediaType
            );
        }
        catch (Exception ex)
        {
            return new ResponseHtmlContent(
                Html: string.Empty,
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
    public Task<ResponseExtractedContent> ExtractContentAsync(
        ResponseHtmlContent htmlContent,
        CancellationToken cancellationToken = default)
    {
        // Use SmartReader or any HTML parsing library
        var reader = new SmartReader.Reader("https://example.com", htmlContent.Html);
        var article = reader.GetArticle();

        var content = new ResponseExtractedContent(
            SourceUrlRetrival: null!, // Populated by the caller or pipeline
            SourceHtmlContent: htmlContent,
            MainText: article.TextContent ?? string.Empty,
            Title: article.Title ?? string.Empty,
            Byline: article.Byline,
            Language: article.Language,
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
    private readonly IUrlRetriever<GoogleSearchOptions> _urlRetriever;
    private readonly IHtmlRetriever _htmlRetriever;
    private readonly IContentExtractor _contentExtractor;

    public WebSearchPipeline(
        IUrlRetriever<GoogleSearchOptions> urlRetriever,
        IHtmlRetriever htmlRetriever,
        IContentExtractor contentExtractor)
    {
        _urlRetriever = urlRetriever;
        _htmlRetriever = htmlRetriever;
        _contentExtractor = contentExtractor;
    }

    public async Task<List<ResponseExtractedContent>> SearchAndExtractAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        // Step 1: Search
        var urls = await _urlRetriever.SearchAsync(query, cancellationToken: cancellationToken);

        var results = new List<ResponseExtractedContent>();

        foreach (var url in urls)
        {
            // Step 2: Fetch HTML
            var html = await _htmlRetriever.FetchContentAsync(url, cancellationToken);

            if (html.Error)
            {
                results.Add(new ResponseExtractedContent(
                    SourceUrlRetrival: url,
                    SourceHtmlContent: html,
                    MainText: string.Empty,
                    Title: url.Title ?? string.Empty,
                    Error: true,
                    ErrorMessage: html.ErrorMessage
                ));
                continue;
            }

            // Step 3: Extract content
            var content = await _contentExtractor.ExtractContentAsync(html, cancellationToken);
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
services.AddHttpClient<HttpClientHtmlRetriever>();
services.AddSingleton<IUrlRetriever<GoogleSearchOptions>, GoogleUrlRetriever>();
services.AddSingleton<IHtmlRetriever, HttpClientHtmlRetriever>();
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

## License

This project is maintained by [DeepSigma LLC](https://github.com/DeepSigma-LLC). See the [LICENSE](LICENSE) file for details.
