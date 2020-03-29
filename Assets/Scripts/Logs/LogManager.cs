using Assets.Scripts.Logs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

public static class LogManager
{
    internal static string LogFolderLocation = $"{AppDomain.CurrentDomain.BaseDirectory}\\Logs\\";

    public static Logger GetLoggerForCurrentClass([CallerMemberName]string callingClassName = "")
    {
        if (callingClassName == "")
            return null;

        Logger classLogger = _loggers.Find(x => x.ClassName == callingClassName);
        if (classLogger is null)
        {
            classLogger = new Logger(callingClassName);
            _loggers.Add(classLogger);
        }

        return classLogger;
    }

    private static List<Logger> _loggers = new List<Logger>();
}
