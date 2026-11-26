namespace Lab3_5.BLL.Entities;

/// <summary>
/// Стоматолог. Розважає дітей у кабінеті — в ігровій формі вчить чистити зуби.
/// Потрібні модель щелепи та набір дитячих зубних щіток.
/// Специфіка: у кабінеті вміщується не більше 5 дітей;
/// якщо є іграшки — дарує їх сміливим пацієнтам.
/// </summary>
public class Dentist : ChildrenEntertainer
{
    public const int MaxGroupSize = 5;

    public Dentist(string lastName, Equipment availableEquipment = Equipment.None)
        : base(lastName, availableEquipment)
    {
    }

    public override string ProfessionName => "Стоматолог";

    public override Equipment RequiredEquipment => Equipment.ToothModel | Equipment.ToothbrushSet;

    public override int MaxChildren => MaxGroupSize;

    protected override string DescribeEntertainment(int childrenCount)
    {
        string result = $"Стоматолог {LastName} в ігровій формі вчить {childrenCount} дітей чистити зуби на моделі щелепи";

        if (AvailableEquipment.HasFlag(Equipment.Toys))
        {
            result += " і дарує кожному іграшку";
        }

        return result + ".";
    }

    public override string Cook(string dish)
    {
        string validDish = ValidateDish(dish);
        return $"Стоматолог {LastName} готує «{validDish}» без цукру — для здорових зубів.";
    }
}
