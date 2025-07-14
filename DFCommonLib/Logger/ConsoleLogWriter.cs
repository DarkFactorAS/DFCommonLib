using System;
namespace DFCommonLib.Logger
{
    public class ConsoleLogWriter : ILogOutputWriter
    {
        public int LogMessage(DFLogLevel logLevel, string group, string message)
        {
            return 0;            
        }

        public void LogMessage(DFLogLevel logLevel, string group, string message, int errorId)
        {
            SetSeverityColor(logLevel);
            var logName = GetLogLevelName(logLevel);
            if ( errorId != 0 )
            {
                Console.WriteLine("[{0}][{1}][{2}] {3,-70}", logName, group, errorId,message);
            }
            else
            {                
                Console.WriteLine("[{0}][{1}] {2,-70}", logName, group,message);
            }
        }
        
        public string GetName()
        {
            return "ConsoleLogWriter";
        }

        private static string GetLogLevelName(DFLogLevel logLevel)
        {
            switch (logLevel)
            {
                case DFLogLevel.UNKNOWN:
                case DFLogLevel.INFO:
                    return "INFO     ";
                case DFLogLevel.DEBUG:
                    return "DEBUG    ";
                case DFLogLevel.IMPORTANT:
                    return "IMPORTANT";
                case DFLogLevel.WARNING:
                    return "WARNING  ";
                case DFLogLevel.ERROR:
                    return "ERROR    ";
                case DFLogLevel.EXCEPTION:
                    return "EXCEPTION";
                default:
                    return "???      "; // Default case for unknown log levels
            }
        }

        private static void SetSeverityColor(DFLogLevel logLevel)
        {
            switch (logLevel)
            {
                case DFLogLevel.UNKNOWN:
                case DFLogLevel.INFO:
                    SetColor(ConsoleColor.White);
                    break;
                case DFLogLevel.IMPORTANT:
                    SetColor(ConsoleColor.Blue);
                    break;
                case DFLogLevel.DEBUG:
                case DFLogLevel.WARNING:
                    SetColor(ConsoleColor.DarkYellow);
                    break;
                case DFLogLevel.ERROR:
                case DFLogLevel.EXCEPTION:
                    SetColor(ConsoleColor.Red);
                    break;
            }
        }

        private static void SetColor(ConsoleColor color)
        {
            if (color == ConsoleColor.Black)
                color = ConsoleColor.White;
            Console.ForegroundColor = color;
        }
    }
}
