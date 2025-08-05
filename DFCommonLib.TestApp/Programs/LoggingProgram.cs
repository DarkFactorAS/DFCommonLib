

using DFCommonLib.Logger;
using Microsoft.AspNetCore.Mvc;

namespace DFCommonLib.TestApp.Programs
{

    public class LoggingProgram
    {
        public LoggingProgram()
        {
            IDFLogger<LoggingProgram> logger = new DFLogger<LoggingProgram>();

            logger.Startup("Log Test", "9.9.9");
            logger.LogInfo("This is an info message.");
            logger.LogInfo("This is an info message with args. {0}:{1}", "arg1", "arg2");
            logger.LogDebug("This is a debug message. {0}:{1}", "arg1", "arg2");
            logger.LogImportant("This is an important message. {0}:{1}", "arg1", "arg2");
            logger.LogWarning("This is a warning message. {0}:{1}", "arg1", "arg2");
            logger.LogError("This is an error message. {0}:{1}", "arg1", "arg2");
            logger.LogException("This is an exception message", new System.Exception("Sample exception"));
            logger.LogException("This is an exception message {0}:{1}", new System.Exception("Sample exception"), "arg1", "arg2");
        }
    }
}