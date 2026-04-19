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
    Task<ResponseExtractedContent> ExtractContentAsync(ResponseHtmlContent htmlContent, CancellationToken cancellationToken = default);

}
