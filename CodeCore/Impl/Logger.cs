using Serilog;

namespace CodeCore.Impl
{
    public class Logger : ILogger
    {
        private readonly Appsettings appsettings;

        public Logger(Appsettings appsettings)
        {
            InitSerilog();
            this.appsettings = appsettings;
        }

        private void InitSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log", ".log"),
                              rollingInterval: RollingInterval.Day,
                              retainedFileCountLimit: 100,
                              outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}",
                              shared: true)
                .CreateLogger();
        }

        public void Debug(params string[] args)
        {
            if (appsettings.Debug)
            {
                Info(args);
            }
        }

        public void Error(params string[] msg)
        {
            Log.Error(string.Join(" ", msg));
        }

        public void Error(Exception ex, params string[] msg)
        {
            Log.Error(ex, string.Join(" ", msg));
        }

        public void Info(params string[] args)
        {
            Log.Information(string.Join(" ", args));
        }

        public void IfInfo(bool yes, params string[] args)
        {
            if (yes)
            {
                Info(args);
            }
        }
    }
}
