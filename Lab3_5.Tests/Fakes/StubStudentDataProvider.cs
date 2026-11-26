using Lab3_5.BLL.Entities;
using Lab3_5.DAL;

namespace Lab3_5.Tests.Fakes;

/// <summary>
/// ЗАГЛУШКА (Stub) — «підставляє» заздалегідь підготовлені дані замість файлу.
/// Нічого не перевіряє і не запам'ятовує. Потрібна лише для того, щоб
/// тестований код міг працювати без реального диска.
/// </summary>
public sealed class StubStudentDataProvider : IDataProvider<Student>
{
    private readonly List<Student> _students;

    public StubStudentDataProvider(List<Student> students)
    {
        _students = students;
    }

    public List<Student> Load(string filePath) => _students;

    public void Save(IEnumerable<Student> items, string filePath)
    {
        // Заглушка нічого не зберігає
    }
}
