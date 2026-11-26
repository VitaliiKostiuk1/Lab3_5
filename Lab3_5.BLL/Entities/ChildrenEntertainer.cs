using Lab3_5.BLL.Interfaces;

namespace Lab3_5.BLL.Entities;

/// <summary>
/// Абстрактний базовий клас для додаткових сутностей (Storyteller, Dentist).
/// Реалізує шаблонний метод EntertainChildren: загальні перевірки тут,
/// а конкретний «сценарій» розваги — у нащадках (DescribeEntertainment).
/// </summary>
public abstract class ChildrenEntertainer : Person, IChildrenEntertainer, ICook
{
    protected ChildrenEntertainer(string lastName, Equipment availableEquipment)
        : base(lastName)
    {
        AvailableEquipment = availableEquipment;
    }

    /// <summary>Назва професії українською (для повідомлень).</summary>
    public abstract string ProfessionName { get; }

    public abstract Equipment RequiredEquipment { get; }

    public abstract int MaxChildren { get; }

    public Equipment AvailableEquipment { get; private set; }

    /// <summary>Чого бракує: необхідне мінус наявне (побітово).</summary>
    public Equipment MissingEquipment => RequiredEquipment & ~AvailableEquipment;

    public bool CanEntertainChildren() => MissingEquipment == Equipment.None;

    public void AddEquipment(Equipment equipment)
    {
        if (equipment == Equipment.None)
        {
            throw new ArgumentException("Не вказано обладнання.", nameof(equipment));
        }

        AvailableEquipment |= equipment;
    }

    public void RemoveEquipment(Equipment equipment)
    {
        AvailableEquipment &= ~equipment;
    }

    /// <summary>
    /// Розважати дітей при наявності необхідного екіпірування та обладнання.
    /// </summary>
    public string EntertainChildren(int childrenCount)
    {
        if (childrenCount <= 0 || childrenCount > MaxChildren)
        {
            throw new ArgumentOutOfRangeException(nameof(childrenCount), childrenCount,
                $"Кількість дітей має бути від 1 до {MaxChildren}.");
        }

        if (!CanEntertainChildren())
        {
            throw new InvalidOperationException(
                $"{ProfessionName} {LastName} не може розважати дітей: бракує обладнання ({MissingEquipment}).");
        }

        return DescribeEntertainment(childrenCount);
    }

    /// <summary>Специфічний для кожної професії опис розваги.</summary>
    protected abstract string DescribeEntertainment(int childrenCount);

    /// <summary>Додаткове вміння «Готувати» — кожна професія готує по-своєму.</summary>
    public abstract string Cook(string dish);

    /// <summary>Спільна перевірка назви страви для нащадків.</summary>
    protected static string ValidateDish(string dish)
    {
        if (string.IsNullOrWhiteSpace(dish))
        {
            throw new ArgumentException("Назва страви не може бути порожньою.", nameof(dish));
        }

        return dish.Trim();
    }

    public override string ToString() =>
        $"{ProfessionName} {LastName}, обладнання: {AvailableEquipment}";
}
