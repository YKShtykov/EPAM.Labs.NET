using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using UserStorageServiceLibrary;
using System.Threading;
using System.IO;
using System.Xml.Serialization;

namespace ServiceConnectorLibrary
{
    public class MasterConnector : MarshalByRefObject
    {
        private Socket socket;
        public IPEndPoint endPoint;
        private int[] slavePorts;
        public readonly UserStorageService service;
        private bool сlosing = false;
        private bool сanBeClosed = false;
        public MasterConnector(int port)
        {
            service = new UserStorageService();
            service.StateChanged += SendNewState;
            socket = CreateAndBindSocket(port);
        }

        public void SetSlavePorts(int[] ports)
        {
            slavePorts = ports.Where(p=>p!=endPoint.Port).ToArray();
        }

        public void ListenConnections(object state)
        {
            while (!сlosing)
            {
                Socket handler = socket.Accept();
                handler.ReceiveTimeout = 50;
                byte[] bytes = new byte[1024];
                try
                {
                    if (сlosing) break;
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
            сanBeClosed = true;
        }

        public void SetState(object state)
        {
                SendNewState(new object(), new UserEventArgs("Add", new User() { personalId = 7 }));
        }

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

        private void SendMessage(byte[] msg, int port)
        {
            if (ReferenceEquals(msg, null)) throw new Exception();

            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
            Socket sct = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            bool connected = false;
            while (!connected)
            {
                try
                {
                    sct.Connect(ipEndPoint);
                    connected = false;
                }
                catch (Exception)
                {
                    continue;
                    //throw new Exception("connection failed");
                }
            }
            
            sct.Send(msg);
            Console.WriteLine(">> " + port);
        }

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

        private void HandleMessage(byte[] bytes)
        {
            var cast = MessageConverter.BytesToObject(bytes) is FindMessage;
            if (cast)
            {
                var message = (FindMessage)MessageConverter.BytesToObject(bytes);
                if (message.predicate != null)
                {
                    List<User> users = service.FindByPredicate(message.predicate).ToList();

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
                    User user = service.FindById(message.id);
                    if (ReferenceEquals(user, null)) user = new User() { personalId = 100 };//return;
                    new User() { personalId = 100 };
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
                Console.WriteLine("Master on port "+endPoint.Port+ "was updated");
            }
            
        }        

        public void Close()
        {
            сlosing = true;
            Console.WriteLine("Master");
            service.Dispose();
        }
    }
}
