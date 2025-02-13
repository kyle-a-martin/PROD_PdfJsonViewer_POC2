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

        private readonly JsonSerializerOptions _options = new() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

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

        public async Task<T?> LoadJsonAsync<T>(string filePath, CancellationToken cancellationToken = default) where T : class
        {
            if (!File.Exists(filePath))
            {
                _logger.LogError("File not found: {FilePath}", filePath);
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            try
            {
                using var stream = File.OpenRead(filePath);
                return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error loading file: {FilePath}", filePath);
                throw;
            }
        }

        public async Task SaveJsonAsync(string filePath, JsonNode json, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            if (json is null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            try
            {
                await using var stream = File.Create(filePath, bufferSize: 4096);
                await JsonSerializer.SerializeAsync(stream, json, _options, cancellationToken);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error saving file: {FilePath}", filePath);
                throw;
            }
        }

        // SaveJsonStringAsync
        public async Task SaveJsonAsync(string filePath, string json, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Json string cannot be null or empty", nameof(json));
            }

            try
            {
                await using var stream = File.Create(filePath, bufferSize: 4096);
                await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(json), cancellationToken);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error saving file: {FilePath}", filePath);
                throw;
            }
        }

        // Save Object as Json
        public async Task SaveJsonAsync(string filePath, object obj, CancellationToken cancellationToken = default)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            var json = JsonSerializer.Serialize(obj, _options);
            await SaveJsonAsync(filePath, json, cancellationToken);
        }
    }
}
