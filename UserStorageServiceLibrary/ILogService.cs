using System;

namespace UserStorageServiceLibrary
{
  public interface ILogService
  {
    void LogTrace(string message);

    void LogError(Exception exception);
  }
}
