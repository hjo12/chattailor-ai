using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTailorAI.WinUI.Services.System
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json.Linq;

    public static class JsonService
    {
        // Default JSON settings with tracing and error handling
        private static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented, // Optional: Pretty print JSON
            MissingMemberHandling = MissingMemberHandling.Ignore, // Ignore missing fields
            NullValueHandling = NullValueHandling.Ignore,         // Ignore null values during serialization
            Error = (sender, args) =>
            {
                Console.WriteLine($"Error at Path: {args.ErrorContext.Path}");
                Console.WriteLine($"Error Message: {args.ErrorContext.Error.Message}");
                args.ErrorContext.Handled = true; // Prevent exceptions for this field
            },
            TraceWriter = new MemoryTraceWriter(), // Logs all serialization/deserialization actions
        };

        /// <summary>
        /// Serialize an object to a JSON string safely.
        /// </summary>
        public static string SafeSerialize<T>(T obj, JsonSerializerSettings settings = null)
        {
            try
            {
                Console.WriteLine($"Serializing {typeof(T).Name} object...");
                settings ??= DefaultSettings;
                return JsonConvert.SerializeObject(obj, settings);
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine($"Serialization error for type {typeof(T).Name}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during serialization: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deserialize a JSON string to a typed object safely.
        /// </summary>
        public static T SafeDeserialize<T>(string json, JsonSerializerSettings settings = null)
        {
            try
            {
                Console.WriteLine($"Deserializing {typeof(T).Name} object...");
                settings ??= DefaultSettings;
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine($"Serialization error for type {typeof(T).Name}: {ex.Message}");
                Console.WriteLine($"JSON Content: {json}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during deserialization: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Deserialize a JSON string to a dynamic JObject for inspection or manipulation.
        /// </summary>
        public static JObject SafeDeserializeToJObject(string json, JsonSerializerSettings settings = null)
        {
            try
            {
                Console.WriteLine("Deserializing JSON to JObject...");
                settings ??= DefaultSettings;
                return JsonConvert.DeserializeObject<JObject>(json, settings);
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine($"Deserialization error to JObject: {ex.Message}");
                Console.WriteLine($"JSON Content: {json}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during JObject deserialization: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}