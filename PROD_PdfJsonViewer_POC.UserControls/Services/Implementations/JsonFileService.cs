using Microsoft.Extensions.Logging;
using PROD_PdfJsonViewer_POC.UserControls.Services.Interfaces;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PROD_PdfJsonViewer_POC.UserControls.Services.Implementations
{
    public class JsonFileService : IJsonFileService
    {
        private readonly ILogger<JsonFileService> _logger;

        public JsonFileService(ILogger<JsonFileService> logger)
        {
            _logger = logger;
        }

        public async Task<JsonNode?> LoadJsonAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
            {
                _logger.LogError("File not found: {FilePath}", filePath);
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            try
            {
                using var stream = File.OpenRead(filePath);
                return await JsonNode.ParseAsync(stream, cancellationToken: cancellationToken);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error loading file: {FilePath}", filePath);
                throw;
            }
        }

        public async Task SaveJsonAsync(string filePath, JsonNode json, CancellationToken cancellationToken = default)
        {
            try
            {
                // Overwrite (or create) the file.
                using var stream = File.Create(filePath);
                var options = new JsonSerializerOptions { WriteIndented = true };
                await JsonSerializer.SerializeAsync(stream, json, options, cancellationToken);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error saving file: {FilePath}", filePath);
                throw;
            }
        }
    }
}
