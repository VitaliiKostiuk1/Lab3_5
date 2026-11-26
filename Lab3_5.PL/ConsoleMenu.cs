using System.Globalization;
using Lab3_5.BLL.Entities;
using Lab3_5.BLL.Interfaces;
using Lab3_5.BLL.Services;
using Lab3_5.DAL;

namespace Lab3_5.PL;

/// <summary>
/// Консольний інтерфейс. Тут НЕМАЄ бізнес-логіки — лише ввід/вивід
/// і виклик методів IStudentService.
/// </summary>
internal class ConsoleMenu
{
    private const string DefaultDataFile = "students.json";

    private readonly IStudentService _service;

    public ConsoleMenu(IStudentService service)
    {
        _service = service;
    }

    public void Run()
    {
        while (true)
        {
            PrintMenu();
            string choice = ReadString("Ваш вибір: ");
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1": LoadStudents(); break;
                    case "2": ShowStudents(_service.Students, "Усі студенти"); break;
                    case "3": AddStudent(); break;
                    case "4": CountExcellentMaleFifthYear(); break;
                    case "5": GetExcellentMaleFifthYearFromFile(); break;
                    case "6": GraduateWithHonors(); break;
                    case "7": SaveStudents(); break;
                    case "8": DemonstrateEntertainers(); break;
                    case "9": DemonstrateCooking(); break;
                    case "0": return;
                    default: PrintError("Невідомий пункт меню."); break;
                }
            }
            // Помилки бізнес-логіки та доступу до даних показуємо користувачу,
            // а не «валимо» програму
            catch (ArgumentException ex) { PrintError(ex.Message); }
            catch (InvalidOperationException ex) { PrintError(ex.Message); }
            catch (DataAccessException ex) { PrintError(ex.Message); }

            Console.WriteLine();
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("========== Лабораторна робота 3.5, варіант 13 ==========");
        Console.WriteLine("1. Завантажити студентів з файлу");
        Console.WriteLine("2. Показати всіх студентів");
        Console.WriteLine("3. Додати студента");
        Console.WriteLine("4. Кількість студентів-чоловіків відмінників 5-го курсу");
        Console.WriteLine("5. Отримати їх дані з файлу");
        Console.WriteLine("6. Випуск з ВУЗу з дипломом з відзнакою (+ збереження)");
        Console.WriteLine("7. Зберегти студентів у файл");
        Console.WriteLine("8. Storyteller та Dentist: розважати дітей");
        Console.WriteLine("9. Додаткове вміння «Готувати»");
        Console.WriteLine("0. Вихід");
    }

    // ------------------------- Дії меню -------------------------

    private void LoadStudents()
    {
        string path = ReadPath();
        _service.LoadFromFile(path);
        PrintSuccess($"Завантажено студентів: {_service.Students.Count}");
    }

    private void SaveStudents()
    {
        string path = ReadPath();
        _service.SaveToFile(path);
        PrintSuccess($"Збережено студентів: {_service.Students.Count}");
    }

    private void AddStudent()
    {
        // Кожне поле вводимо окремо і одразу перевіряємо сетером через «чернетку»,
        // щоб користувач бачив помилку саме того поля, яке ввів неправильно.
        var draft = new Student();
        ReadUntilValid(() => draft.LastName = ReadString("Прізвище: "));
        ReadUntilValid(() => draft.Course = ReadInt("Курс (1-6): "));
        ReadUntilValid(() => draft.StudentTicket = ReadString("Студентський квиток (напр. КВ12345678): "));
        ReadUntilValid(() => draft.Gender = ReadGender());
        ReadUntilValid(() => draft.AverageGrade = ReadDouble("Середній бал (0-100): "));
        ReadUntilValid(() => draft.IdentificationCode = ReadString("Ідентифікаційний код (10 цифр): "));

        _service.Add(draft);
        PrintSuccess("Студента додано.");
    }

    private void CountExcellentMaleFifthYear()
    {
        int count = _service.CountExcellentMaleFifthYearStudents();
        PrintSuccess($"Студентів-чоловіків відмінників 5-го курсу: {count}");
        ShowStudents(_service.GetExcellentMaleFifthYearStudents(), "Їх дані");
    }

    private void GetExcellentMaleFifthYearFromFile()
    {
        string path = ReadPath();
        IReadOnlyList<Student> students = _service.GetExcellentMaleFifthYearStudentsFromFile(path);
        PrintSuccess($"У файлі знайдено студентів-чоловіків відмінників 5-го курсу: {students.Count}");
        ShowStudents(students, "Дані з файлу");
    }

    private void GraduateWithHonors()
    {
        string path = ReadPath();
        int graduated = _service.GraduateAllWithHonorsAndSave(path);
        PrintSuccess(graduated > 0
            ? $"Випущено з дипломом з відзнакою: {graduated}. Зміни збережено у файл."
            : "Немає студентів, які мають право на диплом з відзнакою.");
    }

    private static void DemonstrateEntertainers()
    {
        var entertainers = new List<ChildrenEntertainer>
        {
            new Storyteller("Казковський", Equipment.FairyTaleBook | Equipment.Costume),
            new Dentist("Зубенко", Equipment.ToothModel | Equipment.ToothbrushSet | Equipment.Toys)
        };

        foreach (ChildrenEntertainer entertainer in entertainers)
        {
            Console.WriteLine(entertainer);
            TryEntertain(entertainer, 4);

            // Якщо чогось бракує — «докуповуємо» і пробуємо ще раз
            if (!entertainer.CanEntertainChildren())
            {
                Console.WriteLine($"  -> Додаємо обладнання: {entertainer.MissingEquipment}");
                entertainer.AddEquipment(entertainer.MissingEquipment);
                TryEntertain(entertainer, 4);
            }

            Console.WriteLine();
        }

        // Специфіка Dentist: у кабінеті не більше 5 дітей
        TryEntertain(entertainers[1], 10);
    }

    private static void TryEntertain(IChildrenEntertainer entertainer, int children)
    {
        try
        {
            PrintSuccess("  " + entertainer.EntertainChildren(children));
        }
        catch (InvalidOperationException ex) { PrintError("  " + ex.Message); }
        catch (ArgumentOutOfRangeException ex) { PrintError("  " + ex.Message); }
    }

    private void DemonstrateCooking()
    {
        string dish = ReadString("Яку страву приготувати? ");

        // Поліморфізм через інтерфейс ICook: студент, казкар і стоматолог — різні класи,
        // але всі «вміють готувати» і викликаються однаково.
        var cooks = new List<ICook>();
        if (_service.Students.Count > 0)
        {
            cooks.Add(_service.Students[0]);
        }
        else
        {
            cooks.Add(new Student("Петренко", 1, "КВ00000001", Gender.Male, 75, "1111111111"));
        }

        cooks.Add(new Storyteller("Казковський"));
        cooks.Add(new Dentist("Зубенко"));

        foreach (ICook cook in cooks)
        {
            PrintSuccess(cook.Cook(dish));
        }
    }

    // ------------------------- Допоміжні методи -------------------------

    private static void ShowStudents(IReadOnlyList<Student> students, string title)
    {
        Console.WriteLine($"--- {title} ({students.Count}) ---");
        if (students.Count == 0)
        {
            Console.WriteLine("(список порожній)");
            return;
        }

        for (int i = 0; i < students.Count; i++)
        {
            Console.WriteLine($"{i + 1,2}. {students[i]}");
        }
    }

    private static void ReadUntilValid(Action readAndAssign)
    {
        while (true)
        {
            try
            {
                readAndAssign();
                return;
            }
            catch (ArgumentException ex)
            {
                PrintError(ex.Message);
            }
        }
    }

    private static string ReadPath()
    {
        string path = ReadString($"Шлях до файлу (Enter = {DefaultDataFile}): ");
        return string.IsNullOrWhiteSpace(path) ? DefaultDataFile : path;
    }

    private static string ReadString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static int ReadInt(string prompt)
    {
        string input = ReadString(prompt);
        if (!int.TryParse(input, out int value))
        {
            throw new ArgumentException("Потрібно ввести ціле число.");
        }

        return value;
    }

    private static double ReadDouble(string prompt)
    {
        // Приймаємо і кому, і крапку як десятковий роздільник
        string input = ReadString(prompt).Replace(',', '.');
        if (!double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
        {
            throw new ArgumentException("Потрібно ввести число.");
        }

        return value;
    }

    private static Gender ReadGender()
    {
        string input = ReadString("Стать (ч/ж): ").ToLowerInvariant();
        return input switch
        {
            "ч" or "m" => Gender.Male,
            "ж" or "f" => Gender.Female,
            _ => throw new ArgumentException("Введіть «ч» або «ж».")
        };
    }

    private static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Помилка: " + message);
        Console.ResetColor();
    }
}
