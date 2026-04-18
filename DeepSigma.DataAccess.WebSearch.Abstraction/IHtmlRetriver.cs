using DeepSigma.DataAccess.WebSearch.Abstraction.Model;

namespace DeepSigma.DataAccess.WebSearch.Abstraction;

/// <summary>
/// Defines a contract for asynchronously retrieving HTML content from a specified URL.
/// </summary>
/// <remarks>Implementations of this interface provide methods to fetch HTML page content and related metadata
/// from remote resources. The interface is intended for use in scenarios where HTML content needs to be
/// programmatically retrieved, such as web scraping, content analysis, or automated testing. Implementations should
/// handle network errors and cancellation requests appropriately.</remarks>
public interface IHtmlRetriver
{
    /// <summary>
    /// Asynchronously retrieves the HTML content for the specified response URL.
    /// </summary>
    /// <param name="responseUrl">The response URL information used to locate and retrieve the HTML content. Cannot be null.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests. If not specified, the operation cannot be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the HTML page response content
    /// retrieved from the specified URL.</returns>
    Task<ResponseHtmlContent> FetchContentAsync(ResponseUrlRetrival responseUrl, CancellationToken? cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the content from the specified URL as a string.
    /// </summary>
    /// <param name="URL">The URL of the resource to fetch. Must be a valid, absolute URI.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. This allows the operation to be cancelled if needed, such as when a timeout occurs or when the user cancels the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ResponseHtmlContent"/> object with the fetched content and metadata.</returns>
    Task<ResponseHtmlContent> FetchContentAsync(string URL, CancellationToken? cancellationToken = default);
}
