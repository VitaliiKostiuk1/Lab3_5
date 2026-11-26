namespace Lab3_5.DAL;

/// <summary>
/// Власний виняток рівня DAL. Верхні рівні працюють з ним, а не з десятком
/// різних системних винятків (IOException, JsonException тощо).
/// </summary>
public class DataAccessException : Exception
{
    public DataAccessException(string message) : base(message)
    {
    }

    public DataAccessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
