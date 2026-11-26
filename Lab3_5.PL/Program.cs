using System.Text;
using Lab3_5.BLL.Entities;
using Lab3_5.BLL.Services;
using Lab3_5.DAL;

namespace Lab3_5.PL;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (OperatingSystem.IsWindows())
        {
            // Щоб у консолі Windows коректно вводилась кирилиця
            Console.InputEncoding = Encoding.Unicode;
        }

        // ---- Composition Root ----
        // Єдине місце, де «збираються» залежності (ручний DI):
        // PL створює конкретний DAL і передає його в BLL через інтерфейс.
        IDataProvider<Student> dataProvider = new JsonDataProvider<Student>();
        IStudentService studentService = new StudentService(dataProvider);

        var menu = new ConsoleMenu(studentService);
        menu.Run();
    }
}
