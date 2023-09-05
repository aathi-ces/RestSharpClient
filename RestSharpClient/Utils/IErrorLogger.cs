using log4net.Config;
using log4net.Core;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace RestSharpClient.Utils
{
    public interface IErrorLogger
    {
        void LogError(Exception ex, string infoMessage);
    }

    public class ErrorLogger : IErrorLogger
    {
        private static readonly log4net.ILog log =
                log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);
        public void LogError(Exception ex, string infoMessage)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
            _logger.Error(infoMessage, ex);
        }
    }
}

