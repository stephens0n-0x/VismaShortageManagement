using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using VismaShortageManagement.Models;

namespace VismaShortageManagement.Services
{
    public class DataService : IDataService
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public DataService(string filePath)
        {
            _filePath = filePath;
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public List<Shortage> LoadShortages()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<Shortage>();
                }

                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<Shortage>>(json, _jsonOptions) ?? new List<Shortage>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                return new List<Shortage>();
            }
        }

        public void SaveShortages(List<Shortage> shortages)
        {
            try
            {
                var json = JsonSerializer.Serialize(shortages, _jsonOptions);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }
    }
}
