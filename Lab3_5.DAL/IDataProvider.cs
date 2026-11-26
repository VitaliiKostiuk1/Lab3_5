namespace Lab3_5.DAL;

/// <summary>
/// Абстракція джерела даних (потоку вводу-виводу).
/// Саме через цей інтерфейс бізнес-логіка отримує залежність (Dependency Injection),
/// тому в модульних тестах його легко підмінити заглушкою (stub) або мок-об'єктом (mock).
/// </summary>
/// <typeparam name="T">Тип об'єктів, що зберігаються.</typeparam>
public interface IDataProvider<T>
{
    /// <summary>Зберегти колекцію об'єктів у файл.</summary>
    void Save(IEnumerable<T> items, string filePath);

    /// <summary>Прочитати колекцію об'єктів з файлу.</summary>
    List<T> Load(string filePath);
}
