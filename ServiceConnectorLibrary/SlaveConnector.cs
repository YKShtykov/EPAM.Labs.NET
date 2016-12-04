using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UserStorageServiceLibrary;

namespace ServiceConnectorLibrary
{
    public class SlaveConnector : MarshalByRefObject
    {
        private Socket socket;
        private int masterPort;
        public IPEndPoint endPoint;
        public readonly UserStorageService service;
        private bool closing = false;
        private bool сanBeClosed = false;

        public SlaveConnector(int portNumber)
        {
            service = new UserStorageService();
            service.UserNotFound += FindUser;
            socket = CreateAndBindSocket(portNumber);
        }

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
            сanBeClosed = true;
        }
        public void Find(object state)
        {
            FindUser(this, new UserNotFoundEventArgs(null, 100));
        }
        private void FindUser(object sender, UserNotFoundEventArgs eventArgs)
        {
            var message = new FindMessage()
            {
                senderPort = endPoint.Port,
                id = eventArgs.personalId,
                predicate = eventArgs.predicate
            };

            byte[] bytes = MessageConverter.ObjectToBytes(message);
            SendMessage(bytes);
        }

        public void SetMasterPort(int port)
        {
            masterPort = port;
        }

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
            }
            catch (Exception)
            {
                throw new Exception("connection failed");
            }
            sct.Send(msg);
            Console.WriteLine(">>" + masterPort);
        }

        private void HandleMessage(byte[] bytes)
        {
            var message =(UpdateMessage) MessageConverter.BytesToObject(bytes);
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

        public void Close()
        {
            Console.WriteLine("slave");
            service.Dispose();
        }
    }
}
