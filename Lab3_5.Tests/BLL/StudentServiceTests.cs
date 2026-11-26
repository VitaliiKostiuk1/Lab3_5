using Lab3_5.BLL.Entities;
using Lab3_5.BLL.Services;
using Lab3_5.DAL;
using Lab3_5.Tests.Fakes;
using Lab3_5.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab3_5.Tests.BLL;

/// <summary>
/// Тести операцій варіанту 13 у StudentService.
/// Завдання 4*: залежність від файлу (DAL) підміняється через DI:
///   StubStudentDataProvider — заглушка, що просто віддає підготовлені дані;
///   MockStudentDataProvider — мок, у якого ми перевіряємо факт і параметри викликів.
/// Жоден тест тут не торкається файлової системи.
/// </summary>
[TestClass]
public class StudentServiceTests
{
    private const string FilePath = "students.json";

    /// <summary>
    /// Набір, де відомо, скільки «правильних» студентів:
    /// лише двоє — чоловіки, 5 курс, бал ≥ 90, не випущені.
    /// </summary>
    private static List<Student> CreateMixedStudents() => new()
    {
        StudentFactory.Create("Коваленко", course: 5, gender: Gender.Male, averageGrade: 95),   // +
        StudentFactory.Create("Шевчук", course: 5, gender: Gender.Male, averageGrade: 90),      // + (рівно межа)
        StudentFactory.Create("Мельник", course: 5, gender: Gender.Female, averageGrade: 99),   // жінка
        StudentFactory.Create("Бондар", course: 4, gender: Gender.Male, averageGrade: 97),      // 4 курс
        StudentFactory.Create("Ткачук", course: 5, gender: Gender.Male, averageGrade: 89.99),   // не відмінник
        CreateGraduated("Олійник")                                                               // вже випущений
    };

    private static Student CreateGraduated(string lastName)
    {
        Student student = StudentFactory.Create(lastName, course: 5, gender: Gender.Male, averageGrade: 98);
        student.IsGraduated = true;
        student.Diploma = DiplomaType.WithHonors;
        return student;
    }

    private static StudentService CreateServiceWith(List<Student> students)
    {
        var service = new StudentService(new StubStudentDataProvider(students));
        service.LoadFromFile(FilePath);
        return service;
    }

    // =============================== Конструктор (DI) ===============================

    [TestMethod]
    public void Constructor_NullDataProvider_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => new StudentService(null));
    }

    [TestMethod]
    public void Constructor_ValidDataProvider_StartsWithEmptyList()
    {
        // Arrange & Act
        var service = new StudentService(new StubStudentDataProvider(new List<Student>()));

        // Assert
        Assert.AreEqual(0, service.Students.Count);
    }

    // =============================== Завантаження / збереження ===============================

    [TestMethod]
    public void LoadFromFile_ValidPath_CallsProviderOnceWithSamePath()
    {
        // Arrange
        var mock = new MockStudentDataProvider(CreateMixedStudents());
        var service = new StudentService(mock);

        // Act
        service.LoadFromFile("data/group.json");

        // Assert — перевіряємо ВЗАЄМОДІЮ з моком
        Assert.AreEqual(1, mock.LoadCalls.Count);
        Assert.AreEqual("data/group.json", mock.LoadCalls[0]);
        Assert.AreEqual(6, service.Students.Count);
    }

    [TestMethod]
    public void LoadFromFile_CalledTwice_ReplacesPreviousData()
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(CreateMixedStudents()));
        service.Add(StudentFactory.Create("Зайвий"));

        // Act
        service.LoadFromFile(FilePath);

        // Assert
        Assert.AreEqual(6, service.Students.Count);
        Assert.IsFalse(service.Students.Any(s => s.LastName == "Зайвий"));
    }

    [TestMethod]
    public void LoadFromFile_ProviderReturnsNull_StudentsListIsEmpty()
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(null));

        // Act
        service.LoadFromFile(FilePath);

        // Assert
        Assert.AreEqual(0, service.Students.Count);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void LoadFromFile_EmptyPath_ThrowsAndDoesNotCallProvider(string path)
    {
        // Arrange
        var mock = new MockStudentDataProvider();
        var service = new StudentService(mock);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => service.LoadFromFile(path));
        Assert.AreEqual(0, mock.LoadCalls.Count);
    }

    [TestMethod]
    public void LoadFromFile_ProviderFails_DataAccessExceptionIsPropagated()
    {
        // Arrange — мок імітує збій рівня доступу до даних
        var mock = new MockStudentDataProvider
        {
            ExceptionOnLoad = new DataAccessException("Файл не знайдено")
        };
        var service = new StudentService(mock);

        // Act & Assert
        Assert.ThrowsException<DataAccessException>(() => service.LoadFromFile(FilePath));
    }

    [TestMethod]
    public void SaveToFile_ValidPath_PassesAllStudentsToProvider()
    {
        // Arrange
        var mock = new MockStudentDataProvider(CreateMixedStudents());
        var service = new StudentService(mock);
        service.LoadFromFile(FilePath);

        // Act
        service.SaveToFile("out.json");

        // Assert
        Assert.AreEqual(1, mock.SaveCalls.Count);
        Assert.AreEqual("out.json", mock.SaveCalls[0].Path);
        CollectionAssert.AreEqual(service.Students.ToList(), mock.SaveCalls[0].Items);
    }

    [TestMethod]
    public void SaveToFile_EmptyPath_ThrowsAndDoesNotCallProvider()
    {
        // Arrange
        var mock = new MockStudentDataProvider();
        var service = new StudentService(mock);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => service.SaveToFile(""));
        Assert.AreEqual(0, mock.SaveCalls.Count);
    }

    // =============================== Додавання ===============================

    [TestMethod]
    public void Add_ValidStudent_AddsToList()
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(new List<Student>()));
        Student student = StudentFactory.Create();

        // Act
        service.Add(student);

        // Assert
        Assert.AreEqual(1, service.Students.Count);
        Assert.AreSame(student, service.Students[0]);
    }

    [TestMethod]
    public void Add_Null_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(new List<Student>()));

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => service.Add(null));
    }

    [TestMethod]
    public void Add_DuplicateTicket_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(new List<Student>()));
        service.Add(StudentFactory.Create("Перший", ticket: "КВ55555555"));

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(
            () => service.Add(StudentFactory.Create("Другий", ticket: "КВ55555555")));
    }

    // =============================== Операція 1: підрахунок ===============================

    [TestMethod]
    public void CountExcellentMaleFifthYearStudents_MixedList_CountsOnlyMatching()
    {
        // Arrange
        StudentService service = CreateServiceWith(CreateMixedStudents());

        // Act
        int count = service.CountExcellentMaleFifthYearStudents();

        // Assert — Коваленко і Шевчук
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public void CountExcellentMaleFifthYearStudents_EmptyList_ReturnsZero()
    {
        // Arrange
        StudentService service = CreateServiceWith(new List<Student>());

        // Act
        int count = service.CountExcellentMaleFifthYearStudents();

        // Assert
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    [DataRow(Gender.Female, 5, 95.0, false)]
    [DataRow(Gender.Male, 4, 95.0, false)]
    [DataRow(Gender.Male, 6, 95.0, false)]
    [DataRow(Gender.Male, 5, 89.0, false)]
    [DataRow(Gender.Male, 5, 90.0, true)]
    public void CountExcellentMaleFifthYearStudents_SingleStudent_RespectsEachCondition(
        Gender gender, int course, double grade, bool shouldBeCounted)
    {
        // Arrange
        StudentService service = CreateServiceWith(new List<Student>
        {
            StudentFactory.Create(course: course, gender: gender, averageGrade: grade)
        });

        // Act
        int count = service.CountExcellentMaleFifthYearStudents();

        // Assert
        Assert.AreEqual(shouldBeCounted ? 1 : 0, count);
    }

    [TestMethod]
    public void GetExcellentMaleFifthYearStudents_MixedList_ReturnsCorrectStudents()
    {
        // Arrange
        StudentService service = CreateServiceWith(CreateMixedStudents());

        // Act
        IReadOnlyList<Student> result = service.GetExcellentMaleFifthYearStudents();

        // Assert
        CollectionAssert.AreEquivalent(
            new[] { "Коваленко", "Шевчук" },
            result.Select(s => s.LastName).ToArray());
    }

    // =============================== Операція 2: дані з файлу ===============================

    [TestMethod]
    public void CountExcellentMaleFifthYearStudentsFromFile_UsesDataFromProvider()
    {
        // Arrange — заглушка підставляє «вміст файлу»
        var service = new StudentService(new StubStudentDataProvider(CreateMixedStudents()));

        // Act
        int count = service.CountExcellentMaleFifthYearStudentsFromFile(FilePath);

        // Assert
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public void GetExcellentMaleFifthYearStudentsFromFile_ReadsFileAndFilters()
    {
        // Arrange
        var mock = new MockStudentDataProvider(CreateMixedStudents());
        var service = new StudentService(mock);

        // Act
        IReadOnlyList<Student> result = service.GetExcellentMaleFifthYearStudentsFromFile("group.json");

        // Assert
        Assert.AreEqual(1, mock.LoadCalls.Count);
        Assert.AreEqual("group.json", mock.LoadCalls[0]);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(s => s.Gender == Gender.Male && s.Course == 5 && s.IsExcellent));
    }

    [TestMethod]
    public void GetExcellentMaleFifthYearStudentsFromFile_EmptyPath_ThrowsArgumentException()
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(CreateMixedStudents()));

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(
            () => service.GetExcellentMaleFifthYearStudentsFromFile(" "));
    }

    // =============================== Операція 3: випуск з відзнакою ===============================

    [TestMethod]
    public void GraduateWithHonors_EligibleStudent_GetsHonorsDiploma()
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(new List<Student>()));
        Student student = StudentFactory.Create(course: 5, averageGrade: 96);

        // Act
        service.GraduateWithHonors(student);

        // Assert
        Assert.IsTrue(student.IsGraduated);
        Assert.AreEqual(DiplomaType.WithHonors, student.Diploma);
    }

    [TestMethod]
    public void GraduateWithHonors_Null_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(new List<Student>()));

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => service.GraduateWithHonors(null));
    }

    [TestMethod]
    public void GraduateWithHonors_AlreadyGraduated_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(new List<Student>()));
        Student student = CreateGraduated("Олійник");

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => service.GraduateWithHonors(student));
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(4)]
    [DataRow(6)]
    public void GraduateWithHonors_NotFifthCourse_ThrowsAndStudentIsNotChanged(int course)
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(new List<Student>()));
        Student student = StudentFactory.Create(course: course, averageGrade: 99);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => service.GraduateWithHonors(student));
        Assert.IsFalse(student.IsGraduated);
        Assert.AreEqual(DiplomaType.None, student.Diploma);
    }

    [TestMethod]
    public void GraduateWithHonors_NotExcellent_ThrowsAndStudentIsNotChanged()
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(new List<Student>()));
        Student student = StudentFactory.Create(course: 5, averageGrade: 75);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => service.GraduateWithHonors(student));
        Assert.IsFalse(student.IsGraduated);
    }

    [TestMethod]
    public void CanGraduateWithHonors_VariousStudents_ReturnsExpected()
    {
        // Arrange
        var service = new StudentService(new StubStudentDataProvider(new List<Student>()));

        // Act & Assert
        Assert.IsTrue(service.CanGraduateWithHonors(StudentFactory.Create(course: 5, averageGrade: 90)));
        Assert.IsTrue(service.CanGraduateWithHonors(
            StudentFactory.Create(course: 5, gender: Gender.Female, averageGrade: 99)));
        Assert.IsFalse(service.CanGraduateWithHonors(null));
        Assert.IsFalse(service.CanGraduateWithHonors(CreateGraduated("Олійник")));
        Assert.IsFalse(service.CanGraduateWithHonors(StudentFactory.Create(course: 4, averageGrade: 99)));
        Assert.IsFalse(service.CanGraduateWithHonors(StudentFactory.Create(course: 5, averageGrade: 80)));
    }

    [TestMethod]
    public void GraduateAllWithHonors_MixedList_GraduatesOnlyEligible()
    {
        // Arrange
        StudentService service = CreateServiceWith(CreateMixedStudents());

        // Act
        int graduated = service.GraduateAllWithHonors();

        // Assert — Коваленко, Шевчук, Мельник (диплом з відзнакою не залежить від статі)
        Assert.AreEqual(3, graduated);
        Assert.IsFalse(service.Students.Single(s => s.LastName == "Бондар").IsGraduated);
        Assert.IsFalse(service.Students.Single(s => s.LastName == "Ткачук").IsGraduated);
        Assert.AreEqual(4, service.Students.Count(s => s.Diploma == DiplomaType.WithHonors));
    }

    [TestMethod]
    public void GraduateAllWithHonors_AfterGraduation_CountBecomesZero()
    {
        // Arrange
        StudentService service = CreateServiceWith(CreateMixedStudents());

        // Act
        service.GraduateAllWithHonors();

        // Assert — випускники вже не студенти, тому не рахуються
        Assert.AreEqual(0, service.CountExcellentMaleFifthYearStudents());
    }

    [TestMethod]
    public void GraduateAllWithHonorsAndSave_HasEligible_SavesOnce()
    {
        // Arrange
        var mock = new MockStudentDataProvider(CreateMixedStudents());
        var service = new StudentService(mock);
        service.LoadFromFile(FilePath);

        // Act
        int graduated = service.GraduateAllWithHonorsAndSave(FilePath);

        // Assert — перевірка поведінки через мок
        Assert.AreEqual(3, graduated);
        Assert.AreEqual(1, mock.SaveCalls.Count);
        Assert.AreEqual(FilePath, mock.SaveCalls[0].Path);
        Assert.AreEqual(4, mock.SaveCalls[0].Items.Count(s => s.Diploma == DiplomaType.WithHonors));
    }

    [TestMethod]
    public void GraduateAllWithHonorsAndSave_NobodyEligible_DoesNotSave()
    {
        // Arrange
        var mock = new MockStudentDataProvider(new List<Student>
        {
            StudentFactory.Create(course: 3, averageGrade: 99),
            StudentFactory.Create(course: 5, averageGrade: 70)
        });
        var service = new StudentService(mock);
        service.LoadFromFile(FilePath);

        // Act
        int graduated = service.GraduateAllWithHonorsAndSave(FilePath);

        // Assert
        Assert.AreEqual(0, graduated);
        Assert.AreEqual(0, mock.SaveCalls.Count);
    }

    [TestMethod]
    public void GraduateAllWithHonorsAndSave_EmptyPath_ThrowsAndNobodyGraduated()
    {
        // Arrange
        var mock = new MockStudentDataProvider(CreateMixedStudents());
        var service = new StudentService(mock);
        service.LoadFromFile(FilePath);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => service.GraduateAllWithHonorsAndSave(""));
        Assert.AreEqual(2, service.CountExcellentMaleFifthYearStudents());
        Assert.AreEqual(0, mock.SaveCalls.Count);
    }
}
