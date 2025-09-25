using Core.Models;
using UnityEngine;
using VContainer.Unity;

namespace Core.Services
{
    /// <summary>
    /// Тестовый сервис для проверки работы JsonDataService
    /// </summary>
    public class DataTestService : IInitializable
    {
        private readonly ApplicationDataProvider _dataProvider;

        public DataTestService(ApplicationDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public void Initialize()
        {
            Debug.Log("=== ТЕСТ JSON DATA SERVICE ===");
            
            if (_dataProvider?.Data != null)
            {
                var data = _dataProvider.Data;
                
                Debug.Log($"✅ Данные успешно загружены!");
                Debug.Log($"📁 Путь к файлу: {Application.persistentDataPath}/NetworkInfrastructure/network_infrastructure_data.json");
                Debug.Log($"📊 Статистика данных:");
                Debug.Log($"   - Устройств: {data.Devices.Count}");
                Debug.Log($"   - Моделей устройств: {data.DeviceModels.Count}");
                Debug.Log($"   - Ролей устройств: {data.DeviceRoles.Count}");
                Debug.Log($"   - Производителей: {data.Manufacturers.Count}");
                Debug.Log($"   - Локаций: {data.Locations.Count}");
                Debug.Log($"   - Кабелей: {data.Cables.Count}");
                Debug.Log($"   - IP адресов: {data.IPAddresses.Count}");
                
                Debug.Log($"🔧 Настройки:");
                Debug.Log($"   - Размер сетки: {data.Settings.GridSize}");
                Debug.Log($"   - Показывать сетку: {data.Settings.ShowGrid}");
                Debug.Log($"   - Автосохранение: {data.Settings.AutoSave}");
                
                Debug.Log($"📋 Роли устройств:");
                foreach (var role in data.DeviceRoles)
                {
                    Debug.Log($"   - {role.Name}: {role.Description}");
                }
                
                Debug.Log($"🏭 Производители:");
                foreach (var manufacturer in data.Manufacturers)
                {
                    Debug.Log($"   - {manufacturer.Name}: {manufacturer.Description}");
                }
                
                Debug.Log($"📱 Модели устройств:");
                foreach (var model in data.DeviceModels)
                {
                    Debug.Log($"   - {model.Name}: {model.Description}");
                }
                
                Debug.Log($"🏢 Локации:");
                foreach (var location in data.Locations)
                {
                    Debug.Log($"   - {location.Name}: {location.Description}");
                }
            }
            else
            {
                Debug.LogError("❌ Не удалось загрузить данные приложения!");
            }
            
            Debug.Log("=== КОНЕЦ ТЕСТА ===");
        }
    }
}
