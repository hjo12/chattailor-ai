﻿namespace ChatTailorAI.Shared.Models.Chat.OpenAI
{
    public class OpenAIChatSettings : ChatSettings
    {
        public double Temperature { get; set; }
        public int MaxTokens { get; set; }
        public string TopP { get; set; }
        public double FrequencyPenalty { get; set; }
        public double PresencePenalty { get; set; }
        public string StopSequences { get; set; }
        public bool Stream { get; set; }
    }
}