namespace RestSharpClient.Utils;

public interface IErrorLogger
{
    void LogError(Exception ex, string infoMessage);
}
