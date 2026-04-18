using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

namespace DeepSigma.DataAccess.WebSearch.Abstraction;

/// <summary>
/// Defines an interface for extracting content from HTML. 
/// This interface provides methods for fetching content from a URL and extracting relevant information from the HTML content. 
/// Implementations of this interface can use various techniques to extract meaningful data, such as using libraries like SmartReader or custom parsing logic.
/// </summary>
public interface IContentExtractor
{
    /// <summary>
    /// Asynchronously extracts relevant content from the provided HTML content.
    /// </summary>
    /// <param name="htmlContent">The HTML content to extract information from.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ResponseExtractedContent"/> object with the extracted content and metadata.</returns>
    Task<ResponseExtractedContent> ExtractedContentAsync(ResponseHtmlContent htmlContent, CancellationToken? cancellationToken = default);


    /// <summary>
    /// Asynchronously extracts relevant content from the provided HTML string. 
    /// This method processes the HTML and returns a string containing the extracted information, such as the main text, title, byline, language, and publication date. 
    /// The extraction logic can be implemented using various techniques, including parsing libraries or custom algorithms to identify and extract meaningful content from the HTML structure.
    /// </summary>
    /// <param name="html">The HTML content to extract information from.</param>
    /// <param name="url">An optional URL associated with the HTML content, which can be used for context during extraction. Defaults to null if not provided.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ResponseExtractedContent"/> object with the extracted content and metadata.</returns>
    Task<ResponseExtractedContent> ExtractedContentAsync(string html, string? url = null, CancellationToken? cancellationToken = default);
}
