using Lab3_5.BLL.Entities;
using Lab3_5.BLL.Interfaces;
using Lab3_5.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab3_5.Tests.BLL;

/// <summary>
/// Тести класу Student (і базового Person). Мета — 100% покриття.
/// Кожен тест побудований за принципом Triple A:
///   Arrange — підготовка, Act — дія, Assert — перевірка.
/// Назви: Метод_Сценарій_ОчікуванийРезультат.
/// </summary>
[TestClass]
public class StudentTests
{
    // =============================== Конструктори ===============================

    [TestMethod]
    public void Constructor_ValidData_SetsAllProperties()
    {
        // Arrange
        const string lastName = "Коваленко";
        const int course = 5;
        const string ticket = "КВ12345678";
        const double grade = 92.5;
        const string code = "3012345678";

        // Act
        var student = new Student(lastName, course, ticket, Gender.Female, grade, code);

        // Assert
        Assert.AreEqual(lastName, student.LastName);
        Assert.AreEqual(course, student.Course);
        Assert.AreEqual(ticket, student.StudentTicket);
        Assert.AreEqual(Gender.Female, student.Gender);
        Assert.AreEqual(grade, student.AverageGrade, 1e-9);
        Assert.AreEqual(code, student.IdentificationCode);
        Assert.IsFalse(student.IsGraduated);
        Assert.AreEqual(DiplomaType.None, student.Diploma);
    }

    [TestMethod]
    public void DefaultConstructor_CreatesStudentWithDefaultValues()
    {
        // Arrange & Act
        var student = new Student();

        // Assert
        Assert.AreEqual(string.Empty, student.LastName);
        Assert.AreEqual(Student.MinCourse, student.Course);
        Assert.AreEqual(Gender.Male, student.Gender);
        Assert.AreEqual(0.0, student.AverageGrade, 1e-9);
        Assert.IsFalse(student.IsGraduated);
        Assert.AreEqual(DiplomaType.None, student.Diploma);
    }

    [TestMethod]
    public void Constructor_InvalidCourse_ThrowsArgumentOutOfRangeException()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new Student("Коваленко", 9, "КВ12345678", Gender.Male, 90, "3012345678"));
    }

    [TestMethod]
    public void Student_ImplementsICook()
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act
        ICook cook = student;

        // Assert
        Assert.IsInstanceOfType(cook, typeof(ICook));
    }

    // =============================== Прізвище (Person) ===============================

    [TestMethod]
    [DataRow("Сидоренко-Коваль")]
    [DataRow("Дем'яненко")]
    [DataRow("Smith")]
    public void LastName_ValidValue_IsSet(string lastName)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act
        student.LastName = lastName;

        // Assert
        Assert.AreEqual(lastName, student.LastName);
    }

    [TestMethod]
    public void LastName_ValueWithSpaces_IsTrimmed()
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act
        student.LastName = "  Петренко  ";

        // Assert
        Assert.AreEqual("Петренко", student.LastName);
    }

    [TestMethod]
    public void LastName_Null_ThrowsArgumentException()
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => student.LastName = null);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void LastName_EmptyOrWhitespace_ThrowsArgumentException(string lastName)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => student.LastName = lastName);
    }

    [TestMethod]
    [DataRow("Петренко1")]
    [DataRow("-Петренко")]
    [DataRow("Петренко Іван")]
    [DataRow("Пет@ренко")]
    public void LastName_InvalidCharacters_ThrowsArgumentException(string lastName)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => student.LastName = lastName);
    }

    [TestMethod]
    public void LastName_TooLong_ThrowsArgumentException()
    {
        // Arrange
        var student = StudentFactory.Create();
        string tooLong = new string('А', Person.MaxLastNameLength + 1);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => student.LastName = tooLong);
    }

    [TestMethod]
    public void LastName_MaxLength_IsSet()
    {
        // Arrange
        var student = StudentFactory.Create();
        string maxLength = new string('А', Person.MaxLastNameLength);

        // Act
        student.LastName = maxLength;

        // Assert
        Assert.AreEqual(maxLength, student.LastName);
    }

    // =============================== Курс ===============================

    [TestMethod]
    [DataRow(1)]
    [DataRow(3)]
    [DataRow(6)]
    public void Course_ValueInRange_IsSet(int course)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act
        student.Course = course;

        // Assert
        Assert.AreEqual(course, student.Course);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(7)]
    [DataRow(int.MaxValue)]
    public void Course_ValueOutOfRange_ThrowsArgumentOutOfRangeException(int course)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => student.Course = course);
    }

    // =============================== Студентський квиток ===============================

    [TestMethod]
    [DataRow("КВ12345678")]
    [DataRow("ЇЄ00000000")]
    public void StudentTicket_ValidFormat_IsSet(string ticket)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act
        student.StudentTicket = ticket;

        // Assert
        Assert.AreEqual(ticket, student.StudentTicket);
    }

    [TestMethod]
    public void StudentTicket_ValueWithSpaces_IsTrimmed()
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act
        student.StudentTicket = " КВ12345678 ";

        // Assert
        Assert.AreEqual("КВ12345678", student.StudentTicket);
    }

    [TestMethod]
    public void StudentTicket_Null_ThrowsArgumentException()
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => student.StudentTicket = null);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("КВ1234567")]    // 7 цифр
    [DataRow("КВ123456789")]  // 9 цифр
    [DataRow("KB12345678")]   // латиниця
    [DataRow("кв12345678")]   // малі літери
    [DataRow("КВ1234567А")]   // літера замість цифри
    [DataRow("К12345678")]    // одна літера
    public void StudentTicket_InvalidFormat_ThrowsArgumentException(string ticket)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => student.StudentTicket = ticket);
    }

    // =============================== Стать ===============================

    [TestMethod]
    [DataRow(Gender.Male)]
    [DataRow(Gender.Female)]
    public void Gender_DefinedValue_IsSet(Gender gender)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act
        student.Gender = gender;

        // Assert
        Assert.AreEqual(gender, student.Gender);
    }

    [TestMethod]
    public void Gender_UndefinedValue_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => student.Gender = (Gender)42);
    }

    // =============================== Середній бал ===============================

    [TestMethod]
    [DataRow(0.0)]
    [DataRow(59.99)]
    [DataRow(100.0)]
    public void AverageGrade_ValueInRange_IsSet(double grade)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act
        student.AverageGrade = grade;

        // Assert
        Assert.AreEqual(grade, student.AverageGrade, 1e-9);
    }

    [TestMethod]
    [DataRow(-0.01)]
    [DataRow(100.01)]
    [DataRow(double.NaN)]
    [DataRow(double.PositiveInfinity)]
    public void AverageGrade_InvalidValue_ThrowsArgumentOutOfRangeException(double grade)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => student.AverageGrade = grade);
    }

    // =============================== Ідентифікаційний код ===============================

    [TestMethod]
    public void IdentificationCode_TenDigits_IsSet()
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act
        student.IdentificationCode = " 0123456789 ";

        // Assert
        Assert.AreEqual("0123456789", student.IdentificationCode);
    }

    [TestMethod]
    public void IdentificationCode_Null_ThrowsArgumentException()
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => student.IdentificationCode = null);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("123456789")]    // 9 цифр
    [DataRow("12345678901")]  // 11 цифр
    [DataRow("12345abcde")]
    [DataRow("12345 67890")]
    public void IdentificationCode_InvalidFormat_ThrowsArgumentException(string code)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => student.IdentificationCode = code);
    }

    // =============================== Відмінник ===============================

    [TestMethod]
    [DataRow(90.0, true)]     // рівно межа — відмінник
    [DataRow(100.0, true)]
    [DataRow(89.99, false)]   // трохи нижче межі — ні
    [DataRow(60.0, false)]
    public void IsExcellent_DependsOnAverageGrade(double grade, bool expected)
    {
        // Arrange
        var student = StudentFactory.Create(averageGrade: grade);

        // Act
        bool actual = student.IsExcellent;

        // Assert
        Assert.AreEqual(expected, actual);
    }

    // =============================== Випуск (властивості) ===============================

    [TestMethod]
    public void GraduationProperties_CanBeSet()
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act
        student.IsGraduated = true;
        student.Diploma = DiplomaType.Regular;

        // Assert
        Assert.IsTrue(student.IsGraduated);
        Assert.AreEqual(DiplomaType.Regular, student.Diploma);
    }

    // =============================== Додаткове вміння «Готувати» ===============================

    [TestMethod]
    public void Cook_ValidDish_ReturnsDescriptionWithNameAndDish()
    {
        // Arrange
        var student = StudentFactory.Create(lastName: "Ткаченко");

        // Act
        string result = student.Cook("  борщ ");

        // Assert
        StringAssert.Contains(result, "Ткаченко");
        StringAssert.Contains(result, "«борщ»");
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("  ")]
    public void Cook_EmptyDish_ThrowsArgumentException(string dish)
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => student.Cook(dish));
    }

    [TestMethod]
    public void Cook_NullDish_ThrowsArgumentException()
    {
        // Arrange
        var student = StudentFactory.Create();

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => student.Cook(null));
    }

    // =============================== ToString ===============================

    [TestMethod]
    public void ToString_NotGraduatedMale_ContainsCourseAndMaleMark()
    {
        // Arrange
        var student = StudentFactory.Create(lastName: "Мельник", course: 3, gender: Gender.Male,
            ticket: "КВ11112222", identificationCode: "5555555555");

        // Act
        string text = student.ToString();

        // Assert
        StringAssert.Contains(text, "Мельник");
        StringAssert.Contains(text, "3 курс");
        StringAssert.Contains(text, "стать: ч");
        StringAssert.Contains(text, "КВ11112222");
        StringAssert.Contains(text, "5555555555");
    }

    [TestMethod]
    public void ToString_GraduatedWithHonorsFemale_ContainsHonorsAndFemaleMark()
    {
        // Arrange
        var student = StudentFactory.Create(gender: Gender.Female);
        student.IsGraduated = true;
        student.Diploma = DiplomaType.WithHonors;

        // Act
        string text = student.ToString();

        // Assert
        StringAssert.Contains(text, "диплом з відзнакою");
        StringAssert.Contains(text, "стать: ж");
    }

    [TestMethod]
    public void ToString_GraduatedWithRegularDiploma_DoesNotMentionHonors()
    {
        // Arrange
        var student = StudentFactory.Create();
        student.IsGraduated = true;
        student.Diploma = DiplomaType.Regular;

        // Act
        string text = student.ToString();

        // Assert
        StringAssert.Contains(text, "випускник");
        Assert.IsFalse(text.Contains("відзнакою"));
    }
}
