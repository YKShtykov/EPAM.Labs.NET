using System;
using System.Net;
using System.Net.Sockets;
using UserStorageServiceLibrary;

namespace ServiceConnectorLibrary
{

  /// <summary>
  /// Class for slave nodes
  /// </summary>
  public class SlaveNode : MarshalByRefObject
  {
    /// <summary>
    /// Nodes' listening socket;
    /// </summary>
    public Socket socket { get; private set; }
    /// <summary>
    /// Nodes' endPoint
    /// </summary>
    public IPEndPoint endPoint { get; private set; }
    /// <summary>
    /// Nodes' UserStorageService for working with users
    /// </summary>
    public UserStorageService service { get; private set; }

    private int masterPort;
    private bool closing = false;

    public SlaveNode(int portNumber)
    {
      service = new UserStorageService();
      service.UserNotFound += FindUser;
      socket = CreateAndBindSocket(portNumber);
    }

    /// <summary>
    /// Method for listening
    /// </summary>
    /// <param name="state"></param>
    public void ListenConnection(object state)
    {
      while (true)
      {
        Socket handler = socket.Accept();
        handler.ReceiveTimeout = 50;
        byte[] bytes = new byte[1024];
        if (closing) break;
        try
        {
          int bytesRec = handler.Receive(bytes);
          Console.WriteLine("<<" + endPoint.Port);
          HandleMessage(bytes);
        }
        catch (Exception)
        {
          continue;
        }
        finally
        {
          handler.Shutdown(SocketShutdown.Both);
          handler.Close();
        }
      }
    }
    /// <summary>
    /// When User Storage Service cant find user in its storage this method runs
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private void FindUser(object sender, UserNotFoundEventArgs eventArgs)
    {
      var message = new FindMessage()
      {
        senderPort = endPoint.Port,
        field = eventArgs.field,
        value = eventArgs.value
      };

      byte[] bytes = MessageConverter.ObjectToBytes(message);
      SendMessage(bytes);
    }

    /// <summary>
    /// Set port of master node
    /// </summary>
    /// <param name="port"></param>
    public void SetMasterPort(int port)
    {
      masterPort = port;
    }

    /// <summary>
    /// Send byte message to some port
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="port"></param>
    private void SendMessage(byte[] msg)
    {
      if (ReferenceEquals(msg, null)) throw new Exception();

      IPHostEntry ipHost = Dns.GetHostEntry("localhost");
      IPAddress ipAddr = ipHost.AddressList[0];
      IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, masterPort);
      Socket sct = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
      try
      {
        sct.Connect(ipEndPoint);
        sct.Send(msg);
        Console.WriteLine(">>" + masterPort);
      }
      catch (Exception)
      {
        //throw new Exception("connection failed");
      }

    }

    /// <summary>
    /// Income messages handler
    /// </summary>
    /// <param name="bytes"></param>
    private void HandleMessage(byte[] bytes)
    {
      var message = (UpdateMessage)MessageConverter.BytesToObject(bytes);
      if (message.operation == "Add")
      {
        service.Add(message.item);
      }
      if (message.operation == "Remove")
      {
        service.Remove(message.item);
      }
      Console.WriteLine("Slave on port " + endPoint.Port + "was updated");
    }

    /// <summary>
    /// Creates new socket and bind it to portNumber
    /// </summary>
    /// <param name="portNumber"></param>
    /// <returns></returns>
    private Socket CreateAndBindSocket(int portNumber)
    {
      IPHostEntry ipHost = Dns.GetHostEntry("localhost");
      IPAddress ipAddr = ipHost.AddressList[0];
      IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, portNumber);
      endPoint = ipEndPoint;

      Socket sct = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
      sct.Bind(ipEndPoint);
      sct.Listen(10);
      Console.WriteLine("Slave socket created on port " + portNumber);

      return sct;
    }

    /// <summary>
    /// Closes node
    /// </summary>
    public void Close()
    {
      Console.WriteLine("slave");
      service.Dispose();
    }
  }
}
