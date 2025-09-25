using System.Collections.Generic;
using Core.Models;
using VContainer.Unity;

namespace Core.Services
{
    public class DeviceRoleDataService
    {
        private readonly ApplicationDataProvider _dataProvider;
        private readonly JsonDataService _dataService;

        public DeviceRoleDataService(ApplicationDataProvider dataProvider, JsonDataService dataService)
        {
            _dataProvider = dataProvider;
            _dataService = dataService;
        }

        public DeviceRole Create(string name, string description)
        {
            var nextId = _dataProvider.Data.DeviceRoles.Count + 1;
            var newDeviceRole = new DeviceRole
            {
                Id = nextId,
                Name = name,
                Description = description,
                IconPath = string.Empty,
            };
            _dataProvider.Data.DeviceRoles.Add(newDeviceRole);
            _dataService.SaveDataToFile();
            return newDeviceRole;
        }

        public DeviceRole Get(int id)
        {
            var deviceRole = _dataProvider.Data.DeviceRoles.Find(x => x.Id == id);
            
            return deviceRole;
        }

        public List<DeviceRole> Get()
        {
            return _dataProvider.Data.DeviceRoles;
        }
    }
}