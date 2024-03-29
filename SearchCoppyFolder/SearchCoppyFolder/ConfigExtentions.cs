﻿using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SearchCoppyFolder
{
    public static class ConfigExtensions
    {
        private static ConfigJson _configJson;
        public static async Task<ConfigJson> GetConfig()
        {
            //try
            //{
            //    if (_configJson != null) return _configJson;
            //    var pathConfig = $"{Directory.GetCurrentDirectory()}/Config.json";
            //    Console.WriteLine("=================================================CONFIG==============================================================");
            //    Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Read config {pathConfig}");
            //    var jsonString = File.ReadAllText(pathConfig);
            //    JsonSerializerOptions options = new()
            //    {
            //        NumberHandling =
            //            JsonNumberHandling.AllowReadingFromString |
            //            JsonNumberHandling.WriteAsString,
            //        ReadCommentHandling = JsonCommentHandling.Skip,
            //        AllowTrailingCommas = true,
            //        WriteIndented = true
            //    };
            //    _configJson = JsonSerializer.Deserialize<ConfigJson>(jsonString, options);
            //    Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Read success | body {JsonSerializer.Serialize(_configJson)}");
            //    Console.WriteLine("=====================================================================================================================");
            //    return _configJson;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

            try
            {
                if (_configJson != null) return _configJson;

                string pathConfig = Path.Combine(Directory.GetCurrentDirectory(), "Config.json");
                Console.WriteLine("=================================================CONFIG==============================================================");
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Read config {pathConfig}");

                string jsonString;
                using (StreamReader reader = new StreamReader(pathConfig))
                {
                    jsonString = await reader.ReadToEndAsync();
                }

                JsonSerializerOptions options = new()
                {
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                    WriteIndented = true
                };

                _configJson = JsonSerializer.Deserialize<ConfigJson>(jsonString, options);

                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Read success | body {JsonSerializer.Serialize(_configJson)}");
                Console.WriteLine("=====================================================================================================================");

                return _configJson;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Error reading config: {ex.Message}");
                throw;
            }

        }
    }
}
