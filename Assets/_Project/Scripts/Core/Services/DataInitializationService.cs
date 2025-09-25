using Core.Models;
using UnityEngine;
using VContainer.Unity;

namespace Core.Services
{
    /// <summary>
    /// Сервис для инициализации данных при запуске приложения
    /// </summary>
    public class DataInitializationService : IInitializable
    {
        private readonly ApplicationDataProvider _dataProvider;

        public DataInitializationService(ApplicationDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public void Initialize()
        {
            Debug.Log("[DataInitializationService] Инициализация данных приложения...");
            
            // Проверяем, что данные загружены
            if (_dataProvider.Data != null)
            {
                Debug.Log($"[DataInitializationService] Данные успешно загружены:");
                Debug.Log($"  - Устройств: {_dataProvider.Data.Devices.Count}");
                Debug.Log($"  - Моделей устройств: {_dataProvider.Data.DeviceModels.Count}");
                Debug.Log($"  - Ролей устройств: {_dataProvider.Data.DeviceRoles.Count}");
                Debug.Log($"  - Производителей: {_dataProvider.Data.Manufacturers.Count}");
                Debug.Log($"  - Локаций: {_dataProvider.Data.Locations.Count}");
                Debug.Log($"  - Кабелей: {_dataProvider.Data.Cables.Count}");
                Debug.Log($"  - IP адресов: {_dataProvider.Data.IPAddresses.Count}");
                
                // Выводим информацию о моделях устройств
                foreach (var model in _dataProvider.Data.DeviceModels)
                {
                    Debug.Log($"  Модель: {model.Name} - {model.Description}");
                }
            }
            else
            {
                Debug.LogError("[DataInitializationService] Не удалось загрузить данные приложения!");
            }
        }
    }
}
