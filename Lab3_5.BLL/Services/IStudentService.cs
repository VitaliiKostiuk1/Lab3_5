using Lab3_5.BLL.Entities;

namespace Lab3_5.BLL.Services;

/// <summary>Операції зі студентами (варіант 13).</summary>
public interface IStudentService
{
    IReadOnlyList<Student> Students { get; }

    void LoadFromFile(string filePath);

    void SaveToFile(string filePath);

    void Add(Student student);

    /// <summary>Кількість студентів-чоловіків відмінників 5-го курсу.</summary>
    int CountExcellentMaleFifthYearStudents();

    /// <summary>Дані студентів-чоловіків відмінників 5-го курсу.</summary>
    IReadOnlyList<Student> GetExcellentMaleFifthYearStudents();

    /// <summary>Отримати дані з файлу і порахувати потрібних студентів.</summary>
    int CountExcellentMaleFifthYearStudentsFromFile(string filePath);

    /// <summary>Отримати з файлу дані студентів-чоловіків відмінників 5-го курсу.</summary>
    IReadOnlyList<Student> GetExcellentMaleFifthYearStudentsFromFile(string filePath);

    bool CanGraduateWithHonors(Student student);

    /// <summary>Випуск з ВУЗу з отриманням диплому з відзнакою.</summary>
    void GraduateWithHonors(Student student);

    /// <summary>Випустити всіх, хто має право на диплом з відзнакою. Повертає кількість.</summary>
    int GraduateAllWithHonors();

    /// <summary>Випустити всіх відмінників 5-го курсу і зберегти результат у файл.</summary>
    int GraduateAllWithHonorsAndSave(string filePath);
}
