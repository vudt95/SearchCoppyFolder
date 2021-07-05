using System;
using System.IO;
using System.Text.Json;

namespace SearchCoppyFolder
{
    public static class ConfigExtentions
    {
        private static ConfigJson _configJson;
        public static ConfigJson GetConfig()
        {
            try
            {
                if (_configJson != null) return _configJson;
                var pathConfig = $"{Directory.GetCurrentDirectory()}/Config.json";
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Read config {pathConfig}");
                string jsonString = File.ReadAllText(pathConfig);
                _configJson = JsonSerializer.Deserialize<ConfigJson>(jsonString);
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Read success | body {JsonSerializer.Serialize(_configJson)}");
                return _configJson;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
