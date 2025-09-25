# Polymer Network Infrastructure Visualizer

## Описание
Приложение для визуализации и документирования сетевой инфраструктуры, вдохновленное NetBox/

## Архитектура

### Основные компоненты:
- **JsonDataService** - Глобальный сервис для работы с JSON данными
- **ApplicationData** - Модель данных приложения
- **BootstrapInstaller** - Конфигурация dependency injection

### Структура данных:
- **NetworkDevice** - Сетевые устройства (роутеры, свитчи, серверы)
- **DeviceModel** - Модели устройств
- **DeviceRole** - Роли устройств 
- **Manufacturer** - Производители оборудования
- **Location** - Локации/здания
- **NetworkCable** - Кабельные соединения

### Структура файлов

Каждый класс модели данных находится в отдельном файле:

#### Основные модели:
- **ApplicationData.cs** - Основная модель данных приложения

## Файл данных
Данные сохраняются в: `Application.persistentDataPath/NetworkInfrastructure/network_infrastructure_data.json`

При первом запуске создается файл с базовыми шаблонами устройств и настройками.
