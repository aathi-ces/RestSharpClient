using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Core;

namespace RestSharpClient.Utils;
public class ErrorLogger : IErrorLogger
{
  private static readonly log4net.ILog log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);
  public void LogError(Exception ex, string infoMessage)
  {
    var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
    XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
    _logger.Error(infoMessage, ex);
  }
}
