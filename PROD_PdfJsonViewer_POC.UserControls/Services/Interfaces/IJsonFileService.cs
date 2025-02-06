using System.Text.Json.Nodes;

namespace PROD_PdfJsonViewer_POC.UserControls.Services.Interfaces
{
    public interface IJsonFileService
    {
        /// <summary>
        /// Loads JSON content from the file asynchronously.
        /// </summary>
        /// <param name="filePath">The file path to load from.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A JsonNode representing the JSON content (or null if empty).</returns>
        Task<JsonNode?> LoadJsonAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves JSON content to the file asynchronously.
        /// </summary>
        /// <param name="filePath">The file path to save to.</param>
        /// <param name="json">The JSON content to save.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        Task SaveJsonAsync(string filePath, JsonNode json, CancellationToken cancellationToken = default);
    }
}
