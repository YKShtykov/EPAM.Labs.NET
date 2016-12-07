using System;

namespace UserStorageServiceLibrary
{
  /// <summary>
  /// Event argumets consists info about user search
  /// </summary>
  [Serializable]
  public class UserNotFoundEventArgs : EventArgs
  {
    public readonly string field;
    public readonly object value;

    public UserNotFoundEventArgs(string field, object value)
    {
      this.field = field;
      this.value = value;
    }
  }
}
