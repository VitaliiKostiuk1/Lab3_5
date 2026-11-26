using System.Text.RegularExpressions;

namespace Lab3_5.BLL.Entities;

/// <summary>
/// Базовий абстрактний клас для всіх людей предметної області
/// (Student, Storyteller, Dentist). Містить спільне поле — прізвище.
/// </summary>
public abstract class Person
{
    public const int MaxLastNameLength = 50;

    // Прізвище: починається з літери, далі — літери, дефіс або апостроф
    // (Сидоренко-Коваль, Дем'яненко). \p{L} — будь-яка літера Unicode (кирилиця теж).
    private static readonly Regex LastNamePattern = new(@"^\p{L}[\p{L}'’\-]*$", RegexOptions.Compiled);

    private string _lastName = string.Empty;

    protected Person()
    {
    }

    protected Person(string lastName)
    {
        LastName = lastName;
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Прізвище не може бути порожнім.", nameof(LastName));
            }

            string trimmed = value.Trim();

            if (trimmed.Length > MaxLastNameLength)
            {
                throw new ArgumentException(
                    $"Прізвище не може бути довшим за {MaxLastNameLength} символів.", nameof(LastName));
            }

            if (!LastNamePattern.IsMatch(trimmed))
            {
                throw new ArgumentException(
                    "Прізвище може містити лише літери, дефіс та апостроф.", nameof(LastName));
            }

            _lastName = trimmed;
        }
    }
}
