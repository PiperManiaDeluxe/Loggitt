using System.Diagnostics;

namespace Loggitt;

/// <summary>
/// Dead simple logging class
/// </summary>
/// <remarks>
/// DEBUG code logs only log if a debugger is attached
/// </remarks>
public static class Loggitt
{
    /// <summary>
    /// Enable or disable log file
    /// </summary>
    public static bool LogToFile = true;

    /// <summary>
    /// Path to log file
    /// </summary>
    public static string LogFile = ".log";

    /// <summary>
    /// The format string that will be applied to every log message
    /// {1} will be replaced with the LogCode of the message
    /// {2} is the contents of the provided message
    /// </summary>
    public static string LogFormat = "{0:yyyy-MM-dd HH:mm:ss.fff} [{1}] {2}";

    /// <summary>
    /// Enable or disable logging to the console
    /// </summary>
    public static bool LogToConsole = true;

    /// <summary>
    /// Enable or disable coloring the console logs
    /// </summary>
    public static bool UseConsoleColors = true;

    /// <summary>
    /// Clears the console with a colored background when a fatal error occurs
    /// </summary>
    public static bool ShowFatalErrorScreen = true;

    /// <summary>
    /// The color of the fatal error screen
    /// </summary>
    public static ConsoleColor ShowFatalErrorScreenColor = ConsoleColor.DarkRed;

    /// <summary>
    /// If true an exception will be thrown on a fatal error, with the contens: "Fatal error: {message}"
    /// </summary>
    public static bool FatalLogThrowsException = true;

    /// <summary>
    /// Log codes
    /// </summary>
    /// <remarks>
    /// INFO: Informational messages
    /// WARN: Warning messages
    /// ERROR: Error messages
    /// FATAL: Fatal errors
    /// SUCCESS: Success messages
    /// DEBUG: Debug messages
    /// NETWORK: Network messages
    /// </remarks>
    [Flags]
    public enum LogCode
    {
        INFO,
        WARN,
        ERROR,
        FATAL,
        SUCCESS,
        DEBUG, // Only logs if a debugger is attached
        NETWORK
    }

    /// <summary>
    /// Color mapping for log codes, only used if UseConsoleColors is true
    /// </summary>
    public static Dictionary<LogCode, ConsoleColor> LogCodeColors =
        new()
        {
            { LogCode.INFO, ConsoleColor.White },
            { LogCode.WARN, ConsoleColor.Yellow },
            { LogCode.ERROR, ConsoleColor.Red },
            { LogCode.FATAL, ConsoleColor.DarkRed },
            { LogCode.SUCCESS, ConsoleColor.Green },
            { LogCode.DEBUG, ConsoleColor.DarkGray },
            { LogCode.NETWORK, ConsoleColor.Blue }
        };

    /// <summary>
    /// Logs msg with LogCode code
    /// </summary>
    /// <param name="msg">The contents of the message to be logged</param>
    /// <param name="code">The type of log to display</param>
    public static void Log(string msg, LogCode code)
    {
        // Only log debug messages if we are debugging
        if (code == LogCode.DEBUG && !Debugger.IsAttached)
            return;

        string msgOut = string.Format(LogFormat, DateTime.Now, code, msg);

        // Handle fatal log screen
        if (code == LogCode.FATAL && ShowFatalErrorScreen)
        {
            Console.BackgroundColor = ShowFatalErrorScreenColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            Console.WriteLine(">==== FATAL ERROR ====<");
            Console.WriteLine(msgOut);
            Console.WriteLine(">==== FATAL ERROR ====<");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        // Log to console
        else if (LogToConsole)
        {
            if (UseConsoleColors)
                Console.ForegroundColor = LogCodeColors[code];

            Console.WriteLine(msgOut);

            if (UseConsoleColors)
                Console.ResetColor();
        }

        // Log to file
        File.AppendAllText(LogFile, msg + Environment.NewLine);

        // Throw exception if fatal
        if (code == LogCode.FATAL && FatalLogThrowsException)
            throw new Exception("Fatal error: " + msg);
    }

    /// <summary>
    /// Logs msg as a Info type log
    /// </summary>
    /// <param name="msg">The contents of the message to be logged</param>
    public static void Info(string msg) => Log(msg, LogCode.INFO);

    /// <summary>
    /// Logs msg as a Warn type log
    /// </summary>
    /// <param name="msg">The contents of the message to be logged</param>
    public static void Warn(string msg) => Log(msg, LogCode.WARN);

    /// <summary>
    /// Logs msg as a Error type log
    /// </summary>
    /// <param name="msg">The contents of the message to be logged</param>
    public static void Error(string msg) => Log(msg, LogCode.ERROR);

    /// <summary>
    /// Logs msg as a Fatal type log
    /// </summary>
    /// <param name="msg">The contents of the message to be logged</param>
    /// <remarks>
    /// If FatalLogThrowsException is true this will also throw an exception with the message "Fatal error: {message}".
    /// </remarks>
    public static void Fatal(string msg) => Log(msg, LogCode.FATAL);

    /// <summary>
    /// Logs msg as a Success type log
    /// </summary>
    /// <param name="msg">The contents of the message to be logged</param>
    public static void Success(string msg) => Log(msg, LogCode.SUCCESS);

    /// <summary>
    /// Logs msg as a Debug type log
    /// </summary>
    /// <param name="msg">The contents of the message to be logged</param>
    /// <remarks>
    /// Only logs if debugger is attached
    /// </remarks>
    public static void Debug(string msg) => Log(msg, LogCode.DEBUG);

    /// <summary>
    /// Logs msg as a Network type log
    /// </summary>
    /// <param name="msg">The contents of the message to be logged</param>

    public static void Network(string msg) => Log(msg, LogCode.NETWORK);
}
