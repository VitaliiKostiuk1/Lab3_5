# Лабораторна робота 3.5 — Дослідження модульного тестування (варіант 13)

## Структура рішення

```
Lab3_5.sln
├── Lab3_5.DAL     — доступ до даних: IDataProvider<T>, JsonDataProvider<T>, DataAccessException
├── Lab3_5.BLL     — бізнес-логіка
│   ├── Entities   — Person, Student, ChildrenEntertainer, Storyteller, Dentist, Gender, DiplomaType, Equipment
│   ├── Interfaces — ICook («Готувати»), IChildrenEntertainer («Розважати дітей…»)
│   └── Services   — IStudentService, StudentService (операції варіанту, DI через конструктор)
├── Lab3_5.PL      — консольне меню (Composition Root у Program.cs), students.json з демо-даними
└── Lab3_5.Tests   — модульні тести MSTest
    ├── Fakes      — StubStudentDataProvider (заглушка), MockStudentDataProvider (мок)
    ├── Helpers    — StudentFactory (тестові дані)
    ├── BLL        — StudentTests, StudentServiceTests, StorytellerTests, DentistTests
    └── DAL        — JsonDataProviderTests
```

## Запуск

* **Visual Studio 2022:** відкрити `Lab3_5.sln`, StartUp project — `Lab3_5.PL`, F5.
  Тести: *Test → Test Explorer → Run All*.
* **Командний рядок:**
  ```
  dotnet run --project Lab3_5.PL
  dotnet test
  ```

> Проєкти націлені на `net8.0`. Якщо встановлено лише .NET 9/10 SDK без runtime 8,
> замініть у всіх `.csproj` `<TargetFramework>net8.0</TargetFramework>` на свою версію (`net10.0`).

## Покриття коду (завдання 3)

**Visual Studio 2022 (17.8+, будь-яка редакція):** *Test → Analyze Code Coverage for All Tests*.
Відкриється вікно *Code Coverage Results* — розгорніть `lab3_5.bll.dll → Lab3_5.BLL.Entities → Student`.

**Через coverlet + ReportGenerator:**
```
dotnet tool install -g dotnet-reportgenerator-globaltool
run-coverage.cmd
```
HTML-звіт з’явиться в `CoverageReport/index.html`.

## Що перевіряють тести

| Клас тестів | Що тестується | Підміна залежностей |
|---|---|---|
| `StudentTests` | усі властивості й валідація Student/Person, IsExcellent, Cook, ToString — ціль 100% | не потрібна |
| `StudentServiceTests` | підрахунок чоловіків-відмінників 5 курсу, читання з «файлу», випуск з дипломом з відзнакою, збереження | **stub** і **mock** замість файлу (DI) |
| `StorytellerTests`, `DentistTests` | розваги дітей за наявності обладнання, ліміти груп, костюм/іграшки, Cook | не потрібна |
| `JsonDataProviderTests` | серіалізація JSON, помилки файлу | тимчасова папка |
