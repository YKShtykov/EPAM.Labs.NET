using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace UserStorageServiceLibrary
{
    class DefaultLogService : ILogService
  {
    private static Logger logger = LogManager.GetCurrentClassLogger();
    public void LogError(Exception exception)
    {
      logger.Error(exception);
    }

    public void LogTrace(string message)
    {
      logger.Trace(message);
    }
  }
}
