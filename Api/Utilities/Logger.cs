using System.Drawing;
using MongoDB.Bson;

namespace Api;

public class Logger{
    
    private Logger(){}

    private static Logger? _instance;

    private readonly Dictionary<LogLevel,ConsoleColor> _colors= new Dictionary<LogLevel,ConsoleColor>(){
        { LogLevel.Error,       ConsoleColor.Red},
        { LogLevel.Information, ConsoleColor.White },
        { LogLevel.Warning,     ConsoleColor.Yellow },
        { LogLevel.Debug,       ConsoleColor.Blue },
        { LogLevel.Success,     ConsoleColor.Green}
    };

    public static Logger Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Logger();
            }
            return _instance;
        }
    }

    public void Log(object? sender, LogEventArgs message){

        Console.ForegroundColor = message.logLevel.HasValue ? _colors[message.logLevel.Value]: ConsoleColor.White;

        Console.WriteLine($"[{message.logLevel?.ToString().ToUpper()}][{DateTime.UtcNow}]\t{sender?.GetType().Name ?? "Unknown"}: {message.Message}");

        Console.ResetColor();
    }
}

public class LogEventArgs : EventArgs{
    public string Message {get;}
    public LogLevel? logLevel{get;}

    public LogEventArgs(string message, LogLevel? logLevel=LogLevel.Information){
        Message = message;
        this.logLevel = logLevel;
    }
}

public enum LogLevel {
    Debug = 0,
    Information = 1,
    Warning = 2,
    Error = 3,
    Success=4

}