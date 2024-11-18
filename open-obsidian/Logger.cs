using Serilog;

public static class Logger {
    static Logger() {
        var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "logs"));
        var logFilePath = Path.Combine(projectRoot, "log.txt");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Infinite)
            .CreateLogger();
    }

    public static void Info(string message) {
        Log.Information(message);
    }

    public static void Error(string message) {
        Log.Error(message);
    }

    public static void Warn(string message) {
        Log.Warning(message);
    }

    public static void Debug(string message) {
        Log.Debug(message);
    }
}