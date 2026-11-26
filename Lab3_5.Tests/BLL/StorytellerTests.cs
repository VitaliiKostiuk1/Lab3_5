using Lab3_5.BLL.Entities;
using Lab3_5.BLL.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lab3_5.Tests.BLL;

/// <summary>
/// Тести специфічних операцій казкаря, яких НЕМАЄ у класі Student:
/// розважати дітей при наявності екіпірування, робота з обладнанням.
/// </summary>
[TestClass]
public class StorytellerTests
{
    private const Equipment FullKit = Equipment.FairyTaleBook | Equipment.PuppetTheater;

    [TestMethod]
    public void Constructor_SetsNameAndEquipment()
    {
        // Arrange & Act
        var storyteller = new Storyteller("Казковський", Equipment.FairyTaleBook);

        // Assert
        Assert.AreEqual("Казковський", storyteller.LastName);
        Assert.AreEqual(Equipment.FairyTaleBook, storyteller.AvailableEquipment);
        Assert.AreEqual("Казкар", storyteller.ProfessionName);
    }

    [TestMethod]
    public void Constructor_WithoutEquipment_HasNone()
    {
        // Arrange & Act
        var storyteller = new Storyteller("Казковський");

        // Assert
        Assert.AreEqual(Equipment.None, storyteller.AvailableEquipment);
        Assert.IsFalse(storyteller.CanEntertainChildren());
    }

    [TestMethod]
    public void Constructor_InvalidLastName_ThrowsArgumentException()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentException>(() => new Storyteller("123"));
    }

    [TestMethod]
    public void RequiredEquipment_IsBookAndPuppetTheater()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський");

        // Act
        Equipment required = storyteller.RequiredEquipment;

        // Assert
        Assert.AreEqual(FullKit, required);
        Assert.AreEqual(Storyteller.MaxGroupSize, storyteller.MaxChildren);
    }

    [TestMethod]
    [DataRow(Equipment.None, false)]
    [DataRow(Equipment.FairyTaleBook, false)]
    [DataRow(Equipment.PuppetTheater, false)]
    [DataRow(Equipment.FairyTaleBook | Equipment.PuppetTheater, true)]
    [DataRow(Equipment.FairyTaleBook | Equipment.PuppetTheater | Equipment.Costume, true)]
    [DataRow(Equipment.ToothModel | Equipment.ToothbrushSet, false)] // обладнання стоматолога не підходить
    public void CanEntertainChildren_DependsOnEquipment(Equipment equipment, bool expected)
    {
        // Arrange
        var storyteller = new Storyteller("Казковський", equipment);

        // Act
        bool actual = storyteller.CanEntertainChildren();

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void MissingEquipment_OnlyBook_ReturnsPuppetTheater()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський", Equipment.FairyTaleBook | Equipment.Toys);

        // Act
        Equipment missing = storyteller.MissingEquipment;

        // Assert
        Assert.AreEqual(Equipment.PuppetTheater, missing);
    }

    [TestMethod]
    public void EntertainChildren_FullKit_ReturnsDescription()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський", FullKit);

        // Act
        string result = storyteller.EntertainChildren(12);

        // Assert
        StringAssert.Contains(result, "Казковський");
        StringAssert.Contains(result, "12 дітям");
        Assert.IsFalse(result.Contains("костюмі"));
    }

    [TestMethod]
    public void EntertainChildren_WithCostume_MentionsCostume()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський", FullKit | Equipment.Costume);

        // Act
        string result = storyteller.EntertainChildren(5);

        // Assert
        StringAssert.Contains(result, "у казковому костюмі");
    }

    [TestMethod]
    public void EntertainChildren_MissingEquipment_ThrowsInvalidOperationException()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський", Equipment.FairyTaleBook);

        // Act & Assert
        var ex = Assert.ThrowsException<InvalidOperationException>(() => storyteller.EntertainChildren(5));
        StringAssert.Contains(ex.Message, nameof(Equipment.PuppetTheater));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-3)]
    [DataRow(Storyteller.MaxGroupSize + 1)]
    public void EntertainChildren_InvalidChildrenCount_ThrowsArgumentOutOfRangeException(int children)
    {
        // Arrange
        var storyteller = new Storyteller("Казковський", FullKit);

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => storyteller.EntertainChildren(children));
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(Storyteller.MaxGroupSize)]
    public void EntertainChildren_BoundaryChildrenCount_Succeeds(int children)
    {
        // Arrange
        var storyteller = new Storyteller("Казковський", FullKit);

        // Act
        string result = storyteller.EntertainChildren(children);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public void AddEquipment_MissingItem_AllowsEntertaining()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський", Equipment.FairyTaleBook);

        // Act
        storyteller.AddEquipment(Equipment.PuppetTheater);

        // Assert
        Assert.IsTrue(storyteller.CanEntertainChildren());
        Assert.AreEqual(FullKit, storyteller.AvailableEquipment);
    }

    [TestMethod]
    public void AddEquipment_None_ThrowsArgumentException()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => storyteller.AddEquipment(Equipment.None));
    }

    [TestMethod]
    public void RemoveEquipment_RequiredItem_ForbidsEntertaining()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський", FullKit);

        // Act
        storyteller.RemoveEquipment(Equipment.FairyTaleBook);

        // Assert
        Assert.IsFalse(storyteller.CanEntertainChildren());
        Assert.AreEqual(Equipment.FairyTaleBook, storyteller.MissingEquipment);
    }

    [TestMethod]
    public void Storyteller_CanBeUsedThroughInterfaces()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський", FullKit);

        // Act
        IChildrenEntertainer entertainer = storyteller;
        ICook cook = storyteller;

        // Assert
        Assert.IsTrue(entertainer.CanEntertainChildren());
        StringAssert.Contains(cook.Cook("вареники"), "казку");
    }

    [TestMethod]
    public void Cook_ValidDish_ReturnsStorytellerSpecificText()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський");

        // Act
        string result = storyteller.Cook(" пиріжки ");

        // Assert
        StringAssert.Contains(result, "Казкар Казковський");
        StringAssert.Contains(result, "«пиріжки»");
    }

    [TestMethod]
    public void Cook_EmptyDish_ThrowsArgumentException()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => storyteller.Cook(" "));
    }

    [TestMethod]
    public void ToString_ContainsProfessionNameAndEquipment()
    {
        // Arrange
        var storyteller = new Storyteller("Казковський", Equipment.Costume);

        // Act
        string text = storyteller.ToString();

        // Assert
        StringAssert.Contains(text, "Казкар Казковський");
        StringAssert.Contains(text, nameof(Equipment.Costume));
    }
}
