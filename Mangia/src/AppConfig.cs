using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Mangia
{
    public class AppConfig
    {
        public readonly static string CONFIG_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public string LibraryPath { get; set; } = string.Empty;

        public static void SaveConfigToFile(AppConfig config, string filePath)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(filePath, json);
        }

        public void SaveConfig()
        {
            SaveConfigToFile(this, CONFIG_PATH);
        }

        public static AppConfig LoadConfigFromFile(string filePath)
        {
            if (File.Exists(filePath) is false) return new AppConfig();

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }

        public static AppConfig LoadDefaultConfig()
        {
            var config = LoadConfigFromFile(CONFIG_PATH);
            return config;
        }

    }
}
