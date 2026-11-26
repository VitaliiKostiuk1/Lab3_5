namespace Lab3_5.BLL.Entities;

/// <summary>
/// Казкар. Розважає дітей, якщо має книгу казок і ляльковий театр.
/// Специфіка: може працювати з великою групою (до 30 дітей),
/// а за наявності костюма — виступає в образі казкового персонажа.
/// </summary>
public class Storyteller : ChildrenEntertainer
{
    public const int MaxGroupSize = 30;

    public Storyteller(string lastName, Equipment availableEquipment = Equipment.None)
        : base(lastName, availableEquipment)
    {
    }

    public override string ProfessionName => "Казкар";

    public override Equipment RequiredEquipment => Equipment.FairyTaleBook | Equipment.PuppetTheater;

    public override int MaxChildren => MaxGroupSize;

    protected override string DescribeEntertainment(int childrenCount)
    {
        string result = $"Казкар {LastName} читає казку {childrenCount} дітям і показує виставу лялькового театру";

        // Специфічна поведінка: костюм не обов'язковий, але робить виставу яскравішою
        if (AvailableEquipment.HasFlag(Equipment.Costume))
        {
            result += " у казковому костюмі";
        }

        return result + ".";
    }

    public override string Cook(string dish)
    {
        string validDish = ValidateDish(dish);
        return $"Казкар {LastName} готує «{validDish}» і розповідає казку про цю страву.";
    }
}
