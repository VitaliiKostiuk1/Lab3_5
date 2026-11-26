using Lab3_5.BLL.Entities;

namespace Lab3_5.Tests.Helpers;

/// <summary>
/// Фабрика тестових даних: створює валідного студента, у якого можна
/// змінити лише ті поля, що важливі для конкретного тесту.
/// Це робить секцію Arrange короткою і зрозумілою.
/// </summary>
internal static class StudentFactory
{
    private static int _ticketCounter = 10000000;

    public static Student Create(
        string lastName = "Шевченко",
        int course = 5,
        Gender gender = Gender.Male,
        double averageGrade = 95,
        string ticket = null,
        string identificationCode = "1234567890")
    {
        // Унікальний номер квитка для кожного студента: КВ10000001, КВ10000002, ...
        ticket ??= "КВ" + Interlocked.Increment(ref _ticketCounter).ToString("D8");

        return new Student(lastName, course, ticket, gender, averageGrade, identificationCode);
    }
}
