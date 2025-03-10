namespace FriedMacroCode;

public enum LogType
{
    None = 0,
    Info = 1,
    Warning = 2,
    Error = 4,
    Detail = 8
}
public interface Ilogger
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogDetail(string message);


    void FilterLogs(LogType filter);
    void ClearLogs();
}
