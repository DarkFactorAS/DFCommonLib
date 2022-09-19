using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DFCommonLib.Logger
{
    public interface IDFLogger<T>
    {
        void Startup(string appName);
        void LogDebug(string message);
        void LogInfo(string message);
        void LogImportant(string message);
        void LogWarning(string message);
        int LogError(string message);       
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
            if ( lastGroup != null )
            {
                return lastGroup;
            }
            return groupName;
        }

        public void Startup(string appName)
        {
            var group = GetClassName();
            var message = string.Format("Init application {0}",appName);
            DFLogger.PrintStartup(DFLogLevel.INFO, group,message);

            LogInfo("******************************************************");    
            LogInfo("***                                                ***");    
            LogInfo(string.Format("***  Starting {0,-20}{1,20:N0}", appName, "***"));    
            LogInfo("***                                                ***");    
            LogInfo("******************************************************");    
        }

        public void LogInfo( string message )
        {
            var group = GetClassName();
            DFLogger.LogOutput(DFLogLevel.INFO, group, message );
        }

        public void LogDebug( string message )
        {
            #if DEBUG
                var group = GetClassName();
                DFLogger.LogOutput(DFLogLevel.DEBUG, group, message );
            #endif
        }

        public void LogImportant( string message )
        {
            #if DEBUG
                var group = GetClassName();
                DFLogger.LogOutput(DFLogLevel.IMPORTANT, group, message );
            #endif
        }

        public void LogWarning( string message )
        {
            var group = GetClassName();
            DFLogger.LogOutput(DFLogLevel.WARNING, group, message );
        }

        public int LogError(string message)
        {
            var group = GetClassName();
            return DFLogger.LogOutput(DFLogLevel.ERROR, group, message);
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
