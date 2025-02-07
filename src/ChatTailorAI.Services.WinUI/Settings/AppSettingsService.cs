using Microsoft.Windows.ApplicationModel.Resources;
using ChatTailorAI.Shared.Services.Common;

namespace ChatTailorAI.Services.WinUI.Settings
{
    public class AppSettingsService : IAppSettingsService
    {
        public AppSettingsService()
        {
            // Initialize the ResourceManager
            var resourceManager = new ResourceManager();

            // Create a ResourceContext if needed for view-specific resources
            var resourceContext = resourceManager.CreateResourceContext();

            // Load resources from the "appsettings-admin" resource file
            var resourceMap = resourceManager.MainResourceMap.GetSubtree("appsettings-admin");

            // Access individual resource strings
            OpenAiApiKey = resourceMap.TryGetValue(nameof(OpenAiApiKey))?.ValueAsString;
            SpotifyClientId = resourceMap.TryGetValue(nameof(SpotifyClientId))?.ValueAsString;
            SpotifyClientSecret = resourceMap.TryGetValue(nameof(SpotifyClientSecret))?.ValueAsString;
            SpotifyApiKey = resourceMap.TryGetValue(nameof(SpotifyApiKey))?.ValueAsString;
            ElevenLabsApiKey = resourceMap.TryGetValue(nameof(ElevenLabsApiKey))?.ValueAsString;
            GoogleAIApiKey = resourceMap.TryGetValue(nameof(GoogleAIApiKey))?.ValueAsString;
            AnthropicApiKey = resourceMap.TryGetValue(nameof(AnthropicApiKey))?.ValueAsString;
            LMStudioServerUrl = resourceMap.TryGetValue(nameof(LMStudioServerUrl))?.ValueAsString;
        }

        public string OpenAiApiKey { get; }
        public string SpotifyClientId { get; }
        public string SpotifyClientSecret { get; }
        public string SpotifyApiKey { get; }
        public string ElevenLabsApiKey { get; }
        public string GoogleAIApiKey { get; }
        public string AnthropicApiKey { get; }
        public string LMStudioServerUrl { get; }
    }
}