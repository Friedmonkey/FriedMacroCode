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
    void AddLog(LogType type, string message);

    void FilterLogs(LogType filter);
    void ClearLogs();


    //log any of the types
    void LogDetail(string message) => AddLog(LogType.Detail, message);
    void LogInfo(string message) => AddLog(LogType.Info, message);
    void LogWarning(string message) => AddLog(LogType.Warning, message);
    void LogError(string message) => AddLog(LogType.Error, message);
}
