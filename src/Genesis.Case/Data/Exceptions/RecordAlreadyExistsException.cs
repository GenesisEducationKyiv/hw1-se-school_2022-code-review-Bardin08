namespace Data.Exceptions;

public class RecordAlreadyExistsException : Exception
{
    public RecordAlreadyExistsException(string? message)
        : base("Item: " + message + " - is already exists")
    {
    }
}