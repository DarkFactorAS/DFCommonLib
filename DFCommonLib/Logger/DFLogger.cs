using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DFCommonLib.Logger
{
    public interface IDFLogger<T>
    {
        void Startup(string appName, string appVersion);
        void LogInfo(string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogImportant(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        int LogError(string message, params object[] args);
        int LogException(string message, Exception ex);
        int LogException(string message, Exception ex, params object[] args);
    }

    public class OutputWriter
    {
        public DFLogLevel logLevel;
        public ILogOutputWriter logOutputWriter;

        public OutputWriter(DFLogLevel logLevel, ILogOutputWriter logOutputWriter)
        {
            this.logLevel = logLevel;
            this.logOutputWriter = logOutputWriter;
        }
    }

    public class DFLogger<T> : IDFLogger<T>
    {
        private string GetGroup()
        {
            return typeof(T).ToString();
        }

        private string GetClassName()
        {
            var groupName = GetGroup();
            var lastGroup = groupName.Split(".").LastOrDefault();
            if (lastGroup != null)
            {
                return lastGroup;
            }
            return groupName;
        }

        public void Startup(string appName, string appversion)
        {
            var group = GetClassName();
            var message = string.Format("Init application {0} v:{1}", appName, appversion);
            DFLogger.PrintStartup(DFLogLevel.INFO, group, message);

            LogInfo("******************************************************");
            LogInfo("***                                                ***");
            LogInfo(string.Format("***  Starting {0,-28} {1,7} ***", appName, appversion));
            LogInfo("***                                                ***");
            LogInfo("******************************************************");
        }

        public void LogInfo(string message, params object[] args)
        {
            var group = GetClassName();
            DFLogger.LogOutput(DFLogLevel.INFO, group, string.Format(message, args));
        }

        public void LogDebug(string message, params object[] args)
        {
            var group = GetClassName();
            DFLogger.LogOutput(DFLogLevel.DEBUG, group, string.Format(message, args));
        }

        public void LogImportant(string message, params object[] args)
        {
            var group = GetClassName();
            DFLogger.LogOutput(DFLogLevel.IMPORTANT, group, string.Format(message, args));
        }

        public void LogWarning(string message, params object[] args)
        {
            var group = GetClassName();
            DFLogger.LogOutput(DFLogLevel.WARNING, group, string.Format(message, args));
        }

        public int LogError(string message, params object[] args)
        {
            var group = GetClassName();
            return DFLogger.LogOutput(DFLogLevel.ERROR, group, string.Format(message, args));
        }

        public int LogException(string message, Exception ex)
        {
            var group = GetClassName();
            return DFLogger.LogOutput(DFLogLevel.EXCEPTION, group, string.Format("{0} => {1} ", message, ex.ToString()));
        }

        public int LogException(string message, Exception ex, params object[] args)
        {
            var group = GetClassName();
            var formattedMessage = string.Format(message, args);
            return DFLogger.LogOutput(DFLogLevel.EXCEPTION, group, string.Format("{0} => {1} ", formattedMessage, ex.ToString()));
        }
    }

    public class DFLogger
    {
        private static IList<OutputWriter> _ouputWriters = new List<OutputWriter>();

        public static void AddOutput(DFLogLevel logLevel, ILogOutputWriter outputWriter)
        {
            var oldLogger = _ouputWriters.Where(x => x == outputWriter).FirstOrDefault();
            if (oldLogger == null)
            {
                _ouputWriters.Add(new OutputWriter(logLevel, outputWriter));
            }
        }

        private static int InternalLogOutput(DFLogLevel checkLogLevel, DFLogLevel logLevel, string group, string message)
        {
            int errorCode = 0;

            foreach (OutputWriter outputWriter in _ouputWriters)
            {
                if (outputWriter.logLevel <= checkLogLevel && outputWriter.logOutputWriter != null)
                {
                    try
                    {
                        var err = outputWriter.logOutputWriter.LogMessage(logLevel, group, message);
                        if ( err != 0 )
                        {
                            errorCode = err;
                        }
                    }
                    catch(System.PlatformNotSupportedException ex)
                    {
                        outputWriter.logLevel = DFLogLevel.DISABLED;
                        LogOutput(DFLogLevel.ERROR, "DFLogger", string.Format("Removing {0} due to : {1} ", outputWriter.logOutputWriter.GetName(), ex.ToString()));
                    }
                    catch(Exception ex)
                    {
                        // Temp disable this and try to log error to other outputs
                        var tmpLogLevel = outputWriter.logLevel;
                        outputWriter.logLevel = DFLogLevel.DISABLED;
                        LogOutput(DFLogLevel.EXCEPTION, "DFLogger", string.Format("{0}:{1}", outputWriter.logOutputWriter.GetName(), ex.ToString()));
                        outputWriter.logLevel = tmpLogLevel;
                    }
                }
            }
            return errorCode;
        }

        private static void InternalLogOutputWithId(DFLogLevel checkLogLevel, DFLogLevel logLevel, string group, string message, int errorId)
        {
            foreach (OutputWriter outputWriter in _ouputWriters)
            {
                if (outputWriter.logLevel <= checkLogLevel && outputWriter.logOutputWriter != null)
                {
                    try
                    {
                        outputWriter.logOutputWriter.LogMessage(logLevel, group, message, errorId);
                    }
                    catch(System.PlatformNotSupportedException ex)
                    {
                        outputWriter.logLevel = DFLogLevel.DISABLED;
                        LogOutput(DFLogLevel.ERROR, "DFLogger", string.Format("Removing {0} due to : {1} ", outputWriter.logOutputWriter.GetName(), ex.ToString()));
                    }
                    catch(Exception ex)
                    {
                        // Temp disable this and try to log error to other outputs
                        var tmpLogLevel = outputWriter.logLevel;
                        outputWriter.logLevel = DFLogLevel.DISABLED;
                        LogOutput(DFLogLevel.EXCEPTION, "DFLogger", string.Format("{0}:{1}", outputWriter.logOutputWriter.GetName(), ex.ToString()));
                        outputWriter.logLevel = tmpLogLevel;
                    }
                }
            }
        }

        public static int LogOutput(DFLogLevel logLevel, string group, string message)
        {
            int errorId = InternalLogOutput(logLevel, logLevel, group, message);
            InternalLogOutputWithId(logLevel, logLevel, group, message, errorId);
            return errorId;
        }

        public static int PrintStartup(DFLogLevel logLevel, string group, string message)
        {
            return InternalLogOutput(DFLogLevel.FORCE_PRINT, logLevel, group, message);
        }
    }
}
