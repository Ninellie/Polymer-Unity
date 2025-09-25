using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Models
{
    /// <summary>
    /// Основная модель данных приложения
    /// </summary>
    [Serializable]
    public class ApplicationData
    {
        public List<NetworkDevice> Devices = new List<NetworkDevice>();
        public List<DeviceModel> DeviceModels = new List<DeviceModel>();
        public List<DeviceRole> DeviceRoles = new List<DeviceRole>();
        public List<Manufacturer> Manufacturers = new List<Manufacturer>();
        public List<Location> Locations = new List<Location>();
        public List<NetworkCable> Cables = new List<NetworkCable>();
        public List<IPAddress> IPAddresses = new List<IPAddress>();
        public ApplicationSettings Settings = new ApplicationSettings();
        public DateTime LastModified = DateTime.Now;

        public ApplicationData()
        {
            // InitializeDefaultData();
        }
        
        private void InitializeDefaultData()
        {
            // Создаем базовые роли устройств
            DeviceRoles.Add(new DeviceRole
            {
                Id = 1,
                Name = "Router",
                Description = "Маршрутизатор для межсетевого взаимодействия",
                IconPath = "Icons/Router"
            });

            DeviceRoles.Add(new DeviceRole
            {
                Id = 2,
                Name = "Switch",
                Description = "Коммутатор для локальной сети",
                IconPath = "Icons/Switch"
            });

            DeviceRoles.Add(new DeviceRole
            {
                Id = 3,
                Name = "Server",
                Description = "Сервер для предоставления услуг",
                IconPath = "Icons/Server"
            });

            // Создаем базовых производителей
            Manufacturers.Add(new Manufacturer
            {
                Id = 1,
                Name = "Cisco",
                Description = "Ведущий производитель сетевого оборудования"
            });

            Manufacturers.Add(new Manufacturer
            {
                Id = 2,
                Name = "Generic",
                Description = "Универсальный производитель"
            });

            // Создаем базовые модели устройств
            DeviceModels.Add(new DeviceModel
            {
                Id = 1,
                Name = "ISR 4331",
                DeviceRoleId = 1, // Router
                ManufacturerId = 1, // Cisco
                Description = "Интегрированный сервисный маршрутизатор"
            });

            DeviceModels.Add(new DeviceModel
            {
                Id = 2,
                Name = "Catalyst 2960",
                DeviceRoleId = 2, // Switch
                ManufacturerId = 1, // Cisco
                Description = "Коммутатор уровня доступа"
            });

            DeviceModels.Add(new DeviceModel
            {
                Id = 3,
                Name = "Generic Server",
                DeviceRoleId = 3, // Server
                ManufacturerId = 2, // Generic
                Description = "Универсальный сервер"
            });

            // Создаем базовые локации
            Locations.Add(new Location
            {
                Id = 1,
                Name = "Главный офис",
                Description = "Основное здание компании",
                Position = Vector3.zero
            });

            // Настройки по умолчанию
            Settings.GridSize = 1.0f;
            Settings.ShowGrid = true;
            Settings.AutoSave = true;
            Settings.AutoSaveInterval = 300; // 5 минут
        }
    }
}
