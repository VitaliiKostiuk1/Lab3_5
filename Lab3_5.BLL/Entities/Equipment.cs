namespace Lab3_5.BLL.Entities;

/// <summary>
/// Екіпірування та обладнання для розваги дітей.
/// [Flags] дозволяє зберігати КІЛЬКА предметів в одній змінній:
/// Equipment.FairyTaleBook | Equipment.PuppetTheater.
/// </summary>
[Flags]
public enum Equipment
{
    None = 0,
    FairyTaleBook = 1,  // книга казок
    PuppetTheater = 2,  // ляльковий театр
    Costume = 4,        // казковий костюм
    ToothModel = 8,     // модель щелепи
    ToothbrushSet = 16, // набір дитячих зубних щіток
    Toys = 32           // іграшки-подарунки
}
