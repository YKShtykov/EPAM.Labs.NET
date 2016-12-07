using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using UserStorageServiceLibrary;

namespace ServiceConnectorLibrary
{
  /// <summary>
  /// Class for master nodes
  /// </summary>
  public class MasterNode : MarshalByRefObject
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

    private int[] slavePorts;
    private bool сlosing = false;

    public MasterNode(int port)
    {
      service = new UserStorageService();
      service.StateChanged += SendNewState;
      socket = CreateAndBindSocket(port);
    }

    /// <summary>
    /// Method for connecting master to slaves
    /// </summary>
    /// <param name="ports"></param>
    public void SetSlavePorts(int[] ports)
    {
      slavePorts = ports.Where(p => p != endPoint.Port).ToArray();
    }

    /// <summary>
    /// Method for listening
    /// </summary>
    /// <param name="state"></param>
    public void ListenConnections(object state)
    {
      while (!сlosing)
      {
        Socket handler = socket.Accept();
        handler.ReceiveTimeout = 100;
        byte[] bytes = new byte[1024];
        try
        {
          if (сlosing) return;

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
    /// When User Storage Service state changes this method runs
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    private void SendNewState(object sender, UserEventArgs eventArgs)
    {
      var message = new UpdateMessage()
      {
        senderPort = endPoint.Port,
        operation = eventArgs.operation,
        item = eventArgs.user
      };
      byte[] bytes = MessageConverter.ObjectToBytes(message);

      foreach (var port in slavePorts)
      {
        SendMessage(bytes, port);
      }
    }

    /// <summary>
    /// Send byte message to some port
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="port"></param>
    private void SendMessage(byte[] msg, int port)
    {
      if (ReferenceEquals(msg, null)) throw new Exception();

      IPHostEntry ipHost = Dns.GetHostEntry("localhost");
      IPAddress ipAddr = ipHost.AddressList[0];
      IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
      Socket sct = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
      try
      {
        sct.Connect(ipEndPoint);
        sct.Send(msg);
        Console.WriteLine(">> " + port);
      }
      catch (Exception)
      {
        Console.WriteLine("connection from " + endPoint.Port + " to " + port + " failed");
      }

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
      Console.WriteLine("Master socket created on port " + portNumber);

      return sct;
    }

    /// <summary>
    /// Income messages handler
    /// </summary>
    /// <param name="bytes"></param>
    private void HandleMessage(byte[] bytes)
    {
      var cast = MessageConverter.BytesToObject(bytes) is FindMessage;
      if (cast)
      {
        var message = (FindMessage)MessageConverter.BytesToObject(bytes);
        List<User> users = new List<User>();
        switch (message.field)
        {
          case "FirstName":
            users = service.FindByFirstName((string)message.value).ToList();
            break;
          case "LastName":
            users = service.FindByLastname((string)message.value).ToList();
            break;
          case "BirthDate":
            users = service.FindByBirthDate((DateTime)message.value).ToList();
            break;
          case "Gender":
            users = service.FindByGender((Gender)message.value).ToList();
            break;
          case "Id":
            users.Add(service.FindById((int)message.value));
            break;
        }
        foreach (var user in users)
        {
          var sMessage = new UpdateMessage();
          sMessage.operation = "Add";
          sMessage.item = user;
          bytes = MessageConverter.ObjectToBytes(sMessage);
          SendMessage(bytes, message.senderPort);
        }        
      }
      else
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
        Console.WriteLine("Master on port " + endPoint.Port + "was updated");
      }

    }

    /// <summary>
    /// Closes node
    /// </summary>
    public void Close()
    {
      сlosing = true;
      Console.WriteLine("Master");
      service.Dispose();
    }
  }
}
