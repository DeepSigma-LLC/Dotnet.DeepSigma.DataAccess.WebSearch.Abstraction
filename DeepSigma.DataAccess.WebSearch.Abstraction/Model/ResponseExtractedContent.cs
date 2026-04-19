
namespace DeepSigma.DataAccess.WebSearch.Abstraction.Model;

/// <summary>
/// Represents the response from a content extraction operation, containing the URL of the extracted content, the extracted content itself, and the timestamp of when the content was extracted.
/// </summary>
/// <param name="SourceUrlRetrival">The source URL retrieval information associated with the extracted content.</param>
/// <param name="SourceHtmlContent">The source HTML content from which the content was extracted.</param>
/// <param name="MainText">The main text content extracted from the HTML.</param>
/// <param name="Title">The title of the extracted content.</param>
/// <param name="Byline">The byline or author information extracted from the HTML.</param>
/// <param name="Language">The language of the extracted content.</param>
/// <param name="Summary">A summary of the extracted content.</param>
/// <param name="PublishedAt">The date and time when the content was published, if this information can be extracted from the HTML. Defaults to null if not available.</param>
/// <param name="Snippet">An optional snippet of the extracted content. Defaults to null.</param>
/// <param name="ParsedUrls">An optional list of URLs parsed from the extracted content. Defaults to null.</param>
/// <param name="Category">An optional category or classification for the extracted content. Defaults to null.</param>
/// <param name="PrettyUrl">An optional human-readable version of the URL. Defaults to null.</param>
/// <param name="Template">An optional template or format used for the extracted content. Defaults to null.</param>
/// <param name="Thumbnail">An optional URL to a thumbnail image associated with the extracted content. Defaults to null.</param>
/// <param name="ImageUrl">An optional URL to an image associated with the extracted content. Defaults to null.</param>
/// <param name="Author">An optional author of the extracted content. Defaults to null.</param>
/// <param name="Error">Indicates whether an error occurred during content extraction. Defaults to false.</param>
/// <param name="ErrorMessage">An optional error message providing details about any error that occurred during content extraction. Defaults to null.</param>
public record ResponseExtractedContent(
    ResponseUrlRetrival SourceUrlRetrival,
    ResponseHtmlContent SourceHtmlContent,
    string MainText,
    string Title,
    string? Language = null,
    string? Snippet = null,
    string? Byline = null,
    string? Summary = null,
    DateTimeOffset? PublishedAt = null,
    IReadOnlyList<string>? ParsedUrls = null,
    string? Category = null,
    string? PrettyUrl = null,
    string? Template = null,
    string? Thumbnail = null,
    string? ImageUrl = null,
    string? Author = null,
    bool Error = false,
    string[]? ErrorMessage = null
    )
{
    /// <summary>
    /// Stores additional data associated with the object that is not defined by its strongly-typed properties.
    /// </summary>
    /// <remarks>This dictionary can be used to hold extension data or custom properties that may be present
    /// in dynamic or loosely-typed scenarios, such as deserialization of JSON objects with extra fields.</remarks>
    public Dictionary<string, object> AdditionalData = [];
}
