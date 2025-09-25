using System;
using System.IO;
using Core.Models;
using UnityEngine;
using VContainer.Unity;

namespace Core.Services
{
    /// <summary>
    /// Глобальный сервис для работы с JSON данными приложения
    /// </summary>
    public class JsonDataService : IInitializable
    {
        private const string DataFileName = "network_infrastructure_data.json";
        private string _dataFilePath;
        
        private readonly ApplicationDataProvider _dataProvider;
        
        public JsonDataService(ApplicationDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }
        
        public void Initialize()
        {
            SetupDataFilePath();
            LoadOrCreateDataFile();
        }

        private void SetupDataFilePath()
        {
            // Используем стандартный путь для данных приложения
            string dataDirectory = Path.Combine(Application.persistentDataPath, "NetworkInfrastructure");
            
            // Создаем директорию, если она не существует
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            _dataFilePath = Path.Combine(dataDirectory, DataFileName);
        }

        private void LoadOrCreateDataFile()
        {
            try
            {
                if (File.Exists(_dataFilePath))
                {
                    LoadDataFromFile();
                    Debug.Log($"[JsonDataService] Данные загружены из файла: {_dataFilePath}");
                }
                else
                {
                    CreateDefaultDataFile();
                    Debug.Log($"[JsonDataService] Создан новый файл данных: {_dataFilePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonDataService] Ошибка при работе с файлом данных: {ex.Message}");
                CreateDefaultDataFile(); // Создаем файл по умолчанию в случае ошибки
            }
        }

        private void LoadDataFromFile()
        {
            string jsonContent = File.ReadAllText(_dataFilePath);
            _dataProvider.Data = JsonUtility.FromJson<ApplicationData>(jsonContent);
            
            if (_dataProvider.Data == null)
            {
                Debug.LogWarning("[JsonDataService] Не удалось десериализовать данные, создаем новые");
                CreateDefaultDataFile();
            }
        }

        private void CreateDefaultDataFile()
        {
            _dataProvider.Data = new ApplicationData();
            SaveDataToFile();
        }

        public void SaveDataToFile()
        {
            try
            {
                var jsonContent = JsonUtility.ToJson(_dataProvider.Data, true);
                File.WriteAllText(_dataFilePath, jsonContent);
                Debug.Log($"[JsonDataService] Данные сохранены в файл: {_dataFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonDataService] Ошибка при сохранении данных: {ex.Message}");
            }
        }

        public void ReloadData()
        {
            LoadOrCreateDataFile();
        }
    }
}
