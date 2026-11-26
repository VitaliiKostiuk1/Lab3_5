using Lab3_5.BLL.Entities;
using Lab3_5.DAL;

namespace Lab3_5.BLL.Services;

/// <summary>
/// Бізнес-логіка роботи зі студентами.
///
/// ВАЖЛИВО (завдання 4*): сервіс НЕ створює JsonDataProvider сам,
/// а отримує абстракцію IDataProvider&lt;Student&gt; через конструктор
/// (Constructor Injection). Тому в тестах ми передаємо заглушку і
/// тестуємо логіку без справжніх файлів на диску.
/// </summary>
public class StudentService : IStudentService
{
    private readonly IDataProvider<Student> _dataProvider;
    private readonly List<Student> _students = new();

    public StudentService(IDataProvider<Student> dataProvider)
    {
        _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
    }

    public IReadOnlyList<Student> Students => _students.AsReadOnly();

    // ---------------- Робота з файлом (через DAL) ----------------

    public void LoadFromFile(string filePath)
    {
        ValidatePath(filePath);

        // Якщо DAL повернув null — вважаємо, що студентів немає
        List<Student> loaded = _dataProvider.Load(filePath) ?? new List<Student>();

        _students.Clear();
        _students.AddRange(loaded);
    }

    public void SaveToFile(string filePath)
    {
        ValidatePath(filePath);

        // Передаємо копію, щоб DAL не міг змінити внутрішній список сервісу
        _dataProvider.Save(_students.ToList(), filePath);
    }

    public void Add(Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        if (_students.Any(s => s.StudentTicket == student.StudentTicket))
        {
            throw new InvalidOperationException(
                $"Студент із квитком {student.StudentTicket} вже існує.");
        }

        _students.Add(student);
    }

    // ---------------- Операція 1: підрахунок ----------------

    /// <summary>
    /// Критерій варіанту 13: чоловік, 5-й курс, відмінник (бал ≥ 90),
    /// ще не випущений (випускник уже не є студентом).
    /// </summary>
    private static bool IsExcellentMaleFifthYear(Student student) =>
        !student.IsGraduated
        && student.Gender == Gender.Male
        && student.Course == Student.GraduationCourse
        && student.IsExcellent;

    public int CountExcellentMaleFifthYearStudents() =>
        _students.Count(IsExcellentMaleFifthYear);

    public IReadOnlyList<Student> GetExcellentMaleFifthYearStudents() =>
        _students.Where(IsExcellentMaleFifthYear).ToList();

    // ---------------- Операція 2: отримати їх дані з файлу ----------------

    public int CountExcellentMaleFifthYearStudentsFromFile(string filePath)
    {
        LoadFromFile(filePath);
        return CountExcellentMaleFifthYearStudents();
    }

    public IReadOnlyList<Student> GetExcellentMaleFifthYearStudentsFromFile(string filePath)
    {
        LoadFromFile(filePath);
        return GetExcellentMaleFifthYearStudents();
    }

    // ---------------- Операція 3: випуск з дипломом з відзнакою ----------------

    public bool CanGraduateWithHonors(Student student) =>
        student != null
        && !student.IsGraduated
        && student.Course == Student.GraduationCourse
        && student.IsExcellent;

    public void GraduateWithHonors(Student student)
    {
        if (student == null)
        {
            throw new ArgumentNullException(nameof(student));
        }

        if (student.IsGraduated)
        {
            throw new InvalidOperationException($"Студент {student.LastName} вже випущений з ВУЗу.");
        }

        if (student.Course != Student.GraduationCourse)
        {
            throw new InvalidOperationException(
                $"Випуск можливий лише з {Student.GraduationCourse}-го курсу (у {student.LastName} — {student.Course}-й).");
        }

        if (!student.IsExcellent)
        {
            throw new InvalidOperationException(
                $"Для диплома з відзнакою потрібен середній бал ≥ {Student.ExcellentGradeThreshold} " +
                $"(у {student.LastName} — {student.AverageGrade:F2}).");
        }

        student.IsGraduated = true;
        student.Diploma = DiplomaType.WithHonors;
    }

    public int GraduateAllWithHonors()
    {
        // Спочатку відбираємо (ToList), потім змінюємо — щоб не змінювати колекцію під час перебору
        List<Student> eligible = _students.Where(CanGraduateWithHonors).ToList();

        foreach (Student student in eligible)
        {
            GraduateWithHonors(student);
        }

        return eligible.Count;
    }

    public int GraduateAllWithHonorsAndSave(string filePath)
    {
        ValidatePath(filePath);

        int graduatedCount = GraduateAllWithHonors();

        // Зберігаємо лише якщо щось змінилось (це перевіряється мок-об'єктом у тестах)
        if (graduatedCount > 0)
        {
            SaveToFile(filePath);
        }

        return graduatedCount;
    }

    private static void ValidatePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(filePath));
        }
    }
}
