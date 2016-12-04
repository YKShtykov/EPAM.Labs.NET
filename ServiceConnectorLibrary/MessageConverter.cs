using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ServiceConnectorLibrary
{
    class MessageConverter
    {
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
