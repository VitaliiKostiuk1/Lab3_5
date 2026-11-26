using Lab3_5.BLL.Entities;
using Lab3_5.DAL;
using Lab3_5.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab3_5.Tests.DAL;

/// <summary>
/// Тести рівня доступу до даних. На відміну від тестів BLL, тут працюємо
/// зі справжніми файлами, але у тимчасовій папці, яка видаляється після кожного тесту
/// (TestInitialize / TestCleanup), — тести незалежні один від одного (принцип FIRST).
/// </summary>
[TestClass]
public class JsonDataProviderTests
{
    private string _tempDirectory;
    private JsonDataProvider<Student> _provider;

    [TestInitialize]
    public void SetUp()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), "Lab3_5_Tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectory);
        _provider = new JsonDataProvider<Student>();
    }

    [TestCleanup]
    public void TearDown()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }

    private string TempFile(string name) => Path.Combine(_tempDirectory, name);

    [TestMethod]
    public void SaveThenLoad_Students_DataIsPreserved()
    {
        // Arrange
        string path = TempFile("students.json");
        Student graduated = StudentFactory.Create("Дем'яненко", course: 5, gender: Gender.Female, averageGrade: 97.5);
        graduated.IsGraduated = true;
        graduated.Diploma = DiplomaType.WithHonors;
        var original = new List<Student>
        {
            StudentFactory.Create("Коваленко", course: 3, averageGrade: 81.25),
            graduated
        };

        // Act
        _provider.Save(original, path);
        List<Student> loaded = _provider.Load(path);

        // Assert
        Assert.AreEqual(original.Count, loaded.Count);
        for (int i = 0; i < original.Count; i++)
        {
            Assert.AreEqual(original[i].LastName, loaded[i].LastName);
            Assert.AreEqual(original[i].Course, loaded[i].Course);
            Assert.AreEqual(original[i].StudentTicket, loaded[i].StudentTicket);
            Assert.AreEqual(original[i].Gender, loaded[i].Gender);
            Assert.AreEqual(original[i].AverageGrade, loaded[i].AverageGrade, 1e-9);
            Assert.AreEqual(original[i].IdentificationCode, loaded[i].IdentificationCode);
            Assert.AreEqual(original[i].IsGraduated, loaded[i].IsGraduated);
            Assert.AreEqual(original[i].Diploma, loaded[i].Diploma);
        }
    }

    [TestMethod]
    public void Save_CyrillicText_IsWrittenReadable()
    {
        // Arrange
        string path = TempFile("cyr.json");

        // Act
        _provider.Save(new[] { StudentFactory.Create("Ґудзь") }, path);
        string json = File.ReadAllText(path);

        // Assert — кирилиця не екранується у \uXXXX, enum пишеться текстом
        StringAssert.Contains(json, "Ґудзь");
        StringAssert.Contains(json, "\"Male\"");
    }

    [TestMethod]
    public void Save_PathInMissingFolder_CreatesFolder()
    {
        // Arrange
        string path = Path.Combine(_tempDirectory, "sub", "dir", "students.json");

        // Act
        _provider.Save(new List<Student>(), path);

        // Assert
        Assert.IsTrue(File.Exists(path));
    }

    [TestMethod]
    public void Save_NullItems_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => _provider.Save(null, TempFile("a.json")));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("  ")]
    public void SaveAndLoad_EmptyPath_ThrowArgumentException(string path)
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentException>(() => _provider.Save(new List<Student>(), path));
        Assert.ThrowsException<ArgumentException>(() => _provider.Load(path));
    }

    [TestMethod]
    public void Load_MissingFile_ThrowsDataAccessException()
    {
        // Arrange
        string path = TempFile("not_exists.json");

        // Act & Assert
        Assert.ThrowsException<DataAccessException>(() => _provider.Load(path));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("null")]
    [DataRow("[]")]
    public void Load_EmptyContent_ReturnsEmptyList(string content)
    {
        // Arrange
        string path = TempFile("empty.json");
        File.WriteAllText(path, content);

        // Act
        List<Student> loaded = _provider.Load(path);

        // Assert
        Assert.IsNotNull(loaded);
        Assert.AreEqual(0, loaded.Count);
    }

    [TestMethod]
    public void Load_CorruptedJson_ThrowsDataAccessException()
    {
        // Arrange
        string path = TempFile("broken.json");
        File.WriteAllText(path, "[ { \"LastName\": \"Коваленко\", ");

        // Act & Assert
        Assert.ThrowsException<DataAccessException>(() => _provider.Load(path));
    }

    [TestMethod]
    public void Load_InvalidStudentData_ThrowsDataAccessExceptionWithInnerArgumentException()
    {
        // Arrange — курс 9 не пройде валідацію в сетері Student.Course
        string path = TempFile("invalid.json");
        File.WriteAllText(path, "[ { \"LastName\": \"Коваленко\", \"Course\": 9 } ]");

        // Act
        var ex = Assert.ThrowsException<DataAccessException>(() => _provider.Load(path));

        // Assert
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
    }
}
