
using System.Net;

namespace DeepSigma.DataAccess.WebSearch.Abstraction.Model;

/// <summary>
/// Represents the content of a web page as returned by a fetch operation, including its URL, HTML markup, and the
/// timestamp when it was retrieved.
/// </summary>
/// <param name="URL">The absolute URL of the web page from which the content was fetched. Cannot be null or empty.</param>
/// <param name="HTML">The raw HTML markup of the fetched web page. May be empty if the page has no content.</param>
/// <param name="FetchedAt">The date and time, in UTC, when the page content was retrieved.</param>
/// <param name="PublishedAt">The date and time, in UTC, when the content of the page was published, if this information can be extracted from the HTML. Defaults to the current UTC time if not available.</param>
/// <param name="Title">An optional title of the web page, if it can be extracted from the HTML. Defaults to null.</param>
/// <param name="Byline">An optional byline or author information extracted from the HTML, if available. Defaults to null.</param>
/// <param name="Excerpt">An optional excerpt or summary extracted from the HTML, if available. Defaults to null.</param>
/// <param name="Language">An optional language code (e.g., "en" for English) indicating the language of the fetched content, if it can be determined. Defaults to null.</param>
/// <param name="StatusCode">The HTTP status code returned by the server when fetching the page. This indicates whether the fetch operation was successful (e.g., 200 OK) or if there were issues (e.g., 404 Not Found, 500 Internal Server Error).</param>
/// <param name="Error">Indicates whether an error occurred during the fetch operation. Defaults to false.</param>
/// <param name="SourceUrlRetrival">An optional <see cref="ResponseUrlRetrival"/> object containing metadata about the URL retrieval operation that led to fetching this page. Defaults to null.</param>
/// <param name="ContentType">An optional string indicating the MIME type of the fetched content (e.g., "text/html"). Defaults to null.</param>
/// <param name="ErrorMessage">An optional error message providing details about any error that occurred during the fetch operation. Defaults to null.</param>
public record ResponseHtmlContent(
    string URL,
    string HTML,
    DateTimeOffset FetchedAt,
    HttpStatusCode StatusCode,
    string? ContentType = null,
    string? Title = null,
    string? Byline = null,
    string? Excerpt = null,
    string? Language = null,
    ResponseUrlRetrival? SourceUrlRetrival = null,
    bool Error = false,
    string[]? ErrorMessage = null
    );
