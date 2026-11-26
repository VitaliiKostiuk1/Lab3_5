using Lab3_5.BLL.Entities;

namespace Lab3_5.BLL.Interfaces;

/// <summary>
/// Операція з колонки «Додаткові сутності та операції» варіанту 13:
/// «Розважати дітей при наявності необхідного екіпірування та обладнання».
/// </summary>
public interface IChildrenEntertainer
{
    /// <summary>Обладнання, яке є в наявності.</summary>
    Equipment AvailableEquipment { get; }

    /// <summary>Обладнання, без якого розважати дітей неможливо.</summary>
    Equipment RequiredEquipment { get; }

    /// <summary>Максимальна кількість дітей за один раз.</summary>
    int MaxChildren { get; }

    /// <summary>Чи є все необхідне обладнання.</summary>
    bool CanEntertainChildren();

    /// <summary>Розважити дітей (лише якщо є все обладнання).</summary>
    string EntertainChildren(int childrenCount);
}
