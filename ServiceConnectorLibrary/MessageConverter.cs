using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ServiceConnectorLibrary
{
  /// <summary>
  /// Class for converting messages to bytes arrays
  /// </summary>
  class MessageConverter
  {
    /// <summary>
    /// Converts message to bytes array
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static byte[] ObjectToBytes(IMessage message)
    {
      if (message == null)
        return null;
      BinaryFormatter bf = new BinaryFormatter();
      using (MemoryStream ms = new MemoryStream())
      {
        bf.Serialize(ms, message);
        return ms.ToArray();
      }
    }

    /// <summary>
    /// Converts bytes array to message
    /// </summary>
    /// <param name="arr"></param>
    /// <returns></returns>
    public static IMessage BytesToObject(byte[] arr)
    {
      if (arr == null)
        return null;
      BinaryFormatter bf = new BinaryFormatter();
      using (MemoryStream ms = new MemoryStream())
      {
        ms.Write(arr, 0, arr.Length);
        ms.Seek(0, SeekOrigin.Begin);
        var obj = (IMessage)bf.Deserialize(ms);
        return obj;
      }
    }
  }
}
