using System;
using UserStorageServiceLibrary;

namespace ServiceConnectorLibrary
{
  public interface IMessage
  {
  }

  [Serializable]
  public class FindMessage : IMessage
  {
    public int senderPort;
    public object value;
    public string field;
  }

  [Serializable]
  public class UpdateMessage : IMessage
  {
    public int senderPort;
    public string operation;
    public User item;
  }
}
