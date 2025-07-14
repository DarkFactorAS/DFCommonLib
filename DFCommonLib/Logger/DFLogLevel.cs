﻿namespace DFCommonLib.Logger
{
    public enum DFLogLevel
    {
        UNKNOWN = 0,
        INFO = 1,
        DEBUG = 2,
        IMPORTANT = 10,
        WARNING = 20,
        ERROR = 50,
        EXCEPTION = 100,
        DISABLED = 1000,
        FORCE_PRINT = 1001,
    }
}
