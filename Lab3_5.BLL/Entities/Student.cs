using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Lab3_5.BLL.Interfaces;

namespace Lab3_5.BLL.Entities;

/// <summary>
/// Студент (варіант 13): Прізвище, Курс, Студентський квиток, Стать,
/// Середній бал, Ідентифікаційний код.
/// Додаткове вміння — «Готувати» (інтерфейс ICook).
/// </summary>
public class Student : Person, ICook
{
    public const int MinCourse = 1;
    public const int MaxCourse = 6;
    public const int GraduationCourse = 5;          // випускний курс за варіантом
    public const double MinGrade = 0;
    public const double MaxGrade = 100;              // 100-бальна шкала
    public const double ExcellentGradeThreshold = 90; // «відмінно» (A) — від 90 балів

    // Студентський квиток: 2 великі українські літери + 8 цифр, напр. "КВ12345678"
    private static readonly Regex TicketPattern = new(@"^[А-ЯІЇЄҐ]{2}[0-9]{8}$", RegexOptions.Compiled);

    // Ідентифікаційний код (РНОКПП): рівно 10 цифр
    private static readonly Regex IdCodePattern = new(@"^[0-9]{10}$", RegexOptions.Compiled);

    private int _course = MinCourse;
    private string _studentTicket = string.Empty;
    private Gender _gender = Gender.Male;
    private double _averageGrade;
    private string _identificationCode = string.Empty;

    /// <summary>Конструктор без параметрів потрібен для десеріалізації JSON.</summary>
    public Student()
    {
    }

    public Student(string lastName, int course, string studentTicket, Gender gender,
                   double averageGrade, string identificationCode)
        : base(lastName)
    {
        Course = course;
        StudentTicket = studentTicket;
        Gender = gender;
        AverageGrade = averageGrade;
        IdentificationCode = identificationCode;
    }

    public int Course
    {
        get => _course;
        set
        {
            if (value < MinCourse || value > MaxCourse)
            {
                throw new ArgumentOutOfRangeException(nameof(Course), value,
                    $"Курс має бути в межах {MinCourse}–{MaxCourse}.");
            }

            _course = value;
        }
    }

    public string StudentTicket
    {
        get => _studentTicket;
        set
        {
            if (value == null || !TicketPattern.IsMatch(value.Trim()))
            {
                throw new ArgumentException(
                    "Студентський квиток має формат: 2 великі українські літери + 8 цифр (напр. КВ12345678).",
                    nameof(StudentTicket));
            }

            _studentTicket = value.Trim();
        }
    }

    public Gender Gender
    {
        get => _gender;
        set
        {
            // Захист від (Gender)42 — значення, якого немає в enum
            if (!Enum.IsDefined(value))
            {
                throw new ArgumentOutOfRangeException(nameof(Gender), value, "Невідоме значення статі.");
            }

            _gender = value;
        }
    }

    public double AverageGrade
    {
        get => _averageGrade;
        set
        {
            if (double.IsNaN(value) || value < MinGrade || value > MaxGrade)
            {
                throw new ArgumentOutOfRangeException(nameof(AverageGrade), value,
                    $"Середній бал має бути в межах {MinGrade}–{MaxGrade}.");
            }

            _averageGrade = value;
        }
    }

    public string IdentificationCode
    {
        get => _identificationCode;
        set
        {
            if (value == null || !IdCodePattern.IsMatch(value.Trim()))
            {
                throw new ArgumentException("Ідентифікаційний код має складатися з 10 цифр.",
                    nameof(IdentificationCode));
            }

            _identificationCode = value.Trim();
        }
    }

    /// <summary>Чи випущений студент з ВУЗу.</summary>
    public bool IsGraduated { get; set; }

    /// <summary>Отриманий диплом.</summary>
    public DiplomaType Diploma { get; set; } = DiplomaType.None;

    /// <summary>
    /// Відмінник — середній бал не нижче 90.
    /// [JsonIgnore] — обчислювана властивість, у файл її не пишемо.
    /// </summary>
    [JsonIgnore]
    public bool IsExcellent => AverageGrade >= ExcellentGradeThreshold;

    /// <summary>Додаткове вміння «Готувати».</summary>
    public string Cook(string dish)
    {
        if (string.IsNullOrWhiteSpace(dish))
        {
            throw new ArgumentException("Назва страви не може бути порожньою.", nameof(dish));
        }

        return $"Студент {LastName} готує страву «{dish.Trim()}» на кухні гуртожитку.";
    }

    public override string ToString()
    {
        string status = IsGraduated
            ? (Diploma == DiplomaType.WithHonors ? "випускник (диплом з відзнакою)" : "випускник")
            : $"{Course} курс";

        string gender = Gender == Gender.Male ? "ч" : "ж";

        return $"{LastName,-15} | {status,-30} | квиток {StudentTicket} | стать: {gender} | " +
               $"бал: {AverageGrade,6:F2} | код: {IdentificationCode}";
    }
}
