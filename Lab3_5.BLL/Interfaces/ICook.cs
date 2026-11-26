namespace Lab3_5.BLL.Interfaces;

/// <summary>
/// Додаткове вміння варіанту 13 — «Готувати».
/// Реалізується і студентом, і додатковими сутностями (Storyteller, Dentist).
/// </summary>
public interface ICook
{
    /// <summary>Приготувати страву.</summary>
    /// <param name="dish">Назва страви.</param>
    /// <returns>Опис результату.</returns>
    string Cook(string dish);
}
