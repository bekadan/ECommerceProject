namespace Core.Exceptions.Types;

public abstract class AppException : Exception
{
    public int StatusCode { get; }

    protected AppException(string message, int statusCode = 500)
        : base(message)
    {
        StatusCode = statusCode;
    }

    protected AppException(string message, Exception innerException, int statusCode = 500)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
