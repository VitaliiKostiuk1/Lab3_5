using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lab3_5.DAL;

/// <summary>
/// Реалізація потоку вводу-виводу через JSON-серіалізацію (System.Text.Json).
/// </summary>
public class JsonDataProvider<T> : IDataProvider<T>
{
    // Спільні налаштування серіалізатора:
    //  - WriteIndented — «гарний» читабельний JSON;
    //  - UnsafeRelaxedJsonEscaping — кирилиця пишеться як є, а не Ш...;
    //  - JsonStringEnumConverter — enum зберігається текстом ("Male"), а не числом.
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() }
    };

    // UTF-8 без BOM
    private static readonly Encoding FileEncoding = new UTF8Encoding(false);

    public void Save(IEnumerable<T> items, string filePath)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        ValidatePath(filePath);

        try
        {
            // Якщо вказано шлях у неіснуючу папку — створюємо її
            string directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(items.ToList(), Options);
            File.WriteAllText(filePath, json, FileEncoding);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            throw new DataAccessException($"Не вдалося записати файл '{filePath}'.", ex);
        }
    }

    public List<T> Load(string filePath)
    {
        ValidatePath(filePath);

        if (!File.Exists(filePath))
        {
            throw new DataAccessException($"Файл '{filePath}' не знайдено.");
        }

        try
        {
            string json = File.ReadAllText(filePath, FileEncoding);

            // Порожній файл = порожня колекція
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<T>();
            }

            // Вміст "null" теж трактуємо як порожню колекцію
            return JsonSerializer.Deserialize<List<T>>(json, Options) ?? new List<T>();
        }
        catch (JsonException ex)
        {
            throw new DataAccessException($"Файл '{filePath}' має некоректний формат JSON.", ex);
        }
        catch (ArgumentException ex)
        {
            // Сетери сутностей валідують дані й кидають ArgumentException —
            // отже, у файлі записані недопустимі значення (наприклад, курс = 9).
            throw new DataAccessException($"Файл '{filePath}' містить некоректні дані: {ex.Message}", ex);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is ArgumentException inner)
        {
            // На деяких платформах серіалізатор викликає сетери через рефлексію,
            // і тоді виняток сетера приходить «загорнутим» у TargetInvocationException
            throw new DataAccessException($"Файл '{filePath}' містить некоректні дані: {inner.Message}", inner);
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
        {
            throw new DataAccessException($"Не вдалося прочитати файл '{filePath}'.", ex);
        }
    }

    private static void ValidatePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(filePath));
        }
    }
}
