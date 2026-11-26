using Lab3_5.BLL.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab3_5.Tests.BLL;

/// <summary>
/// Тести специфічних операцій стоматолога, яких НЕМАЄ у класі Student
/// і які відрізняються від казкаря (інше обладнання, ліміт 5 дітей, іграшки).
/// </summary>
[TestClass]
public class DentistTests
{
    private const Equipment FullKit = Equipment.ToothModel | Equipment.ToothbrushSet;

    [TestMethod]
    public void Constructor_SetsNameEquipmentAndProfession()
    {
        // Arrange & Act
        var dentist = new Dentist("Зубенко", Equipment.ToothModel);

        // Assert
        Assert.AreEqual("Зубенко", dentist.LastName);
        Assert.AreEqual(Equipment.ToothModel, dentist.AvailableEquipment);
        Assert.AreEqual("Стоматолог", dentist.ProfessionName);
        Assert.AreEqual(FullKit, dentist.RequiredEquipment);
        Assert.AreEqual(Dentist.MaxGroupSize, dentist.MaxChildren);
    }

    [TestMethod]
    [DataRow(Equipment.None, false)]
    [DataRow(Equipment.ToothModel, false)]
    [DataRow(Equipment.ToothbrushSet, false)]
    [DataRow(Equipment.ToothModel | Equipment.ToothbrushSet, true)]
    [DataRow(Equipment.FairyTaleBook | Equipment.PuppetTheater, false)] // обладнання казкаря не підходить
    public void CanEntertainChildren_DependsOnEquipment(Equipment equipment, bool expected)
    {
        // Arrange
        var dentist = new Dentist("Зубенко", equipment);

        // Act
        bool actual = dentist.CanEntertainChildren();

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void EntertainChildren_FullKit_ReturnsDescriptionWithoutGifts()
    {
        // Arrange
        var dentist = new Dentist("Зубенко", FullKit);

        // Act
        string result = dentist.EntertainChildren(3);

        // Assert
        StringAssert.Contains(result, "Стоматолог Зубенко");
        StringAssert.Contains(result, "3 дітей");
        Assert.IsFalse(result.Contains("іграшку"));
    }

    [TestMethod]
    public void EntertainChildren_WithToys_GivesToys()
    {
        // Arrange
        var dentist = new Dentist("Зубенко", FullKit | Equipment.Toys);

        // Act
        string result = dentist.EntertainChildren(2);

        // Assert
        StringAssert.Contains(result, "дарує кожному іграшку");
    }

    [TestMethod]
    public void EntertainChildren_OnlyToys_ThrowsInvalidOperationException()
    {
        // Arrange — іграшки є, але обов'язкового обладнання немає
        var dentist = new Dentist("Зубенко", Equipment.Toys);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => dentist.EntertainChildren(2));
    }

    [TestMethod]
    public void EntertainChildren_SixChildren_ExceedsCabinetLimit()
    {
        // Arrange — казкар упорався б з 6 дітьми, а стоматолог — ні
        var dentist = new Dentist("Зубенко", FullKit);
        var storyteller = new Storyteller("Казковський", Equipment.FairyTaleBook | Equipment.PuppetTheater);

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => dentist.EntertainChildren(6));
        Assert.IsFalse(string.IsNullOrEmpty(storyteller.EntertainChildren(6)));
    }

    [TestMethod]
    public void EntertainChildren_MaxGroupSize_Succeeds()
    {
        // Arrange
        var dentist = new Dentist("Зубенко", FullKit);

        // Act
        string result = dentist.EntertainChildren(Dentist.MaxGroupSize);

        // Assert
        StringAssert.Contains(result, $"{Dentist.MaxGroupSize} дітей");
    }

    [TestMethod]
    public void AddEquipment_SeveralItemsAtOnce_AllAdded()
    {
        // Arrange
        var dentist = new Dentist("Зубенко");

        // Act
        dentist.AddEquipment(Equipment.ToothModel | Equipment.ToothbrushSet);

        // Assert
        Assert.IsTrue(dentist.CanEntertainChildren());
        Assert.AreEqual(Equipment.None, dentist.MissingEquipment);
    }

    [TestMethod]
    public void Cook_ValidDish_ReturnsSugarFreeText()
    {
        // Arrange
        var dentist = new Dentist("Зубенко");

        // Act
        string result = dentist.Cook("сирники");

        // Assert
        StringAssert.Contains(result, "Стоматолог Зубенко");
        StringAssert.Contains(result, "без цукру");
    }

    [TestMethod]
    public void Cook_NullDish_ThrowsArgumentException()
    {
        // Arrange
        var dentist = new Dentist("Зубенко");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => dentist.Cook(null));
    }
}
