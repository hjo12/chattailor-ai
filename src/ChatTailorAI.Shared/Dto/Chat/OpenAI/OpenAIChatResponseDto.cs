﻿using Newtonsoft.Json;
using ChatTailorAI.Shared.Models.Chat.OpenAI.Responses;

namespace ChatTailorAI.Shared.Dto.Chat.OpenAI
{
    public class OpenAIChatResponseDto : ChatResponseDto
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("function_call", NullValueHandling = NullValueHandling.Ignore)]
        public FunctionCall FunctionCall { get; set; }

        // No need for JsonIgnore attributes, ViewModel-specific properties, or OnPropertyChanged
    }
}