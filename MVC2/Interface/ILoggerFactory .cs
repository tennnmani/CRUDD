namespace MVC2.Interface
{
    public interface ILoggerFactory : IDisposable
    {
        ILogger CreateLogger(string categoryName);
        void AddProvider(ILoggerProvider provider);
    }
}
