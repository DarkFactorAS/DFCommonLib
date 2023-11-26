using System;
namespace DFCommonLib.Logger
{
    public interface ILogOutputWriter
    {
        int LogMessage(DFLogLevel logLevel, string group, string message);
        void LogMessage(DFLogLevel logLevel, string group, string message, int errorId);
        string GetName();
    }
}
