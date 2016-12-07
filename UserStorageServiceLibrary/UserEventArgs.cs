using System;

namespace UserStorageServiceLibrary
{
  [Serializable]
  public class UserEventArgs : EventArgs
  {
    public readonly string operation;
    public readonly User user;

    public UserEventArgs(string operation, User user)
    {
      this.operation = operation;
      this.user = user;
    }

  }
}
