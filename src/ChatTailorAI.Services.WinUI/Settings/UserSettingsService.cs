﻿using System;
using Newtonsoft.Json;
using Windows.Storage;
using ChatTailorAI.Shared.Models.Settings;
using ChatTailorAI.Shared.Services.Common;
using Newtonsoft.Json.Serialization;
using ChatTailorAI.WinUI.Services.System;

namespace ChatTailorAI.Services.WinUI.Settings
{
    public class UserSettingsService : IUserSettingsService
    {
        public event EventHandler<string> SettingChanged;

        public T Get<T>(string settingKey)
        {
            object result = ApplicationData.Current.LocalSettings.Values[settingKey];
            return result is null ? (T)UserSettings.Defaults[settingKey] : (T)result;
        }

        public T Get<T>(string settingKey, T defaultOverride)
        {
            object result = ApplicationData.Current.LocalSettings.Values[settingKey];
            return result is null ? defaultOverride : (T)result;
        }

        public T GetAndDeserialize<T>(string settingKey, JsonSerializerSettings jsonSerializerSettings)
        {
            object result = ApplicationData.Current.LocalSettings.Values[settingKey];
            if (result is string serialized)
            {
                var deserializedResult = JsonConvert.DeserializeObject<T>(serialized, jsonSerializerSettings);
                if (deserializedResult is not null)
                {
                    return deserializedResult;
                }
            }

            return (T)UserSettings.Defaults[settingKey];
        }

        public void Set<T>(string settingKey, T value)
        {
            ApplicationData.Current.LocalSettings.Values[settingKey] = value;
            SettingChanged?.Invoke(this, settingKey);
        }

        public void SetAndSerialize<T>(string settingKey, T value, JsonSerializerSettings jsonSerializerSettings)
        {
            var serialized = JsonService.SafeSerialize(value);
            Set(settingKey, serialized);
        }
    }
}