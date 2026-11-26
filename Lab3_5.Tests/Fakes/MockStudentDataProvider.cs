using Lab3_5.BLL.Entities;
using Lab3_5.DAL;

namespace Lab3_5.Tests.Fakes;

/// <summary>
/// МОК-ОБ'ЄКТ (Mock) — записує, як саме з ним взаємодіяв тестований код
/// (які методи, скільки разів, з якими аргументами). У тесті ми потім
/// ПЕРЕВІРЯЄМО ці записи (Assert по моку), напр. «Save викликано рівно 1 раз».
/// Написаний вручну, без бібліотеки Moq, щоб було видно, як це влаштовано.
/// </summary>
public sealed class MockStudentDataProvider : IDataProvider<Student>
{
    private readonly List<Student> _dataToReturn;

    public MockStudentDataProvider(List<Student> dataToReturn = null)
    {
        _dataToReturn = dataToReturn ?? new List<Student>();
    }

    /// <summary>Шляхи, з якими викликався Load.</summary>
    public List<string> LoadCalls { get; } = new();

    /// <summary>Виклики Save: шлях і копія переданих студентів.</summary>
    public List<(string Path, List<Student> Items)> SaveCalls { get; } = new();

    /// <summary>Якщо задано — Load кидає цей виняток (імітація збою DAL).</summary>
    public Exception ExceptionOnLoad { get; set; }

    public List<Student> Load(string filePath)
    {
        LoadCalls.Add(filePath);

        if (ExceptionOnLoad != null)
        {
            throw ExceptionOnLoad;
        }

        return _dataToReturn;
    }

    public void Save(IEnumerable<Student> items, string filePath)
    {
        SaveCalls.Add((filePath, items.ToList()));
    }
}
