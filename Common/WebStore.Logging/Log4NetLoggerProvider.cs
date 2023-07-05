using System.Collections.Concurrent;
using System.Xml;

namespace WebStore.Logging;

public class Log4NetLoggerProvider : ILoggerProvider
{
    private readonly string _ConfigurationFile;
    private readonly ConcurrentDictionary<string, Log4netLogger> _Loggers = new();

    public Log4NetLoggerProvider(string ConfigurationFile)
    {
        _ConfigurationFile = ConfigurationFile;
    }

    public ILogger CreateLogger(string Category) => _Loggers.GetOrAdd(
        Category,
        (category, config) =>
        {
            var xml = new XmlDocument();
            xml.Load(config);
            return new Log4netLogger(category, xml["log4net"] ?? throw new InvalidOperationException("Не удалось извлечь из xml-документа элемент log4net"));
        },
        _ConfigurationFile);

    public void Dispose()
    {
        //var loggers = _Loggers.Values.ToArray();
        _Loggers.Clear();
        //foreach (var logger in loggers.OfType<IDisposable>())
        //    logger.Dispose();
    }
}
