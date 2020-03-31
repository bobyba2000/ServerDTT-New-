using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Threading;

namespace ServerDTT_New_.SupportClass
{
    public class Server
    {
        IPEndPoint IP;
        Socket server;
        public List<Socket> ClientList;
        public List<int> ClientIDList = new List<int>();
        MainWindow mainWindow;
        public Server(MainWindow main)
        {
            mainWindow = main;
            Connect();
        }
        public void SendTSInfo(int round, List<SupportClass.Student> studentList)
        {
            for(int i=0;i<ClientList.Count();i++)
            {
                string message = round.ToString() + "_0";
                for(int j=0;j<studentList.Count();j++)
                {
                    message += "_" + studentList[j].Name;
                    message += "_" + studentList[j].Point.ToString();
                }
                Send(ClientList[i], message);
            }
        }
        void Connect()
        {
            IP = new IPEndPoint(IPAddress.Any, 9999);
            ClientList = new List<Socket>();
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            server.Bind(IP);
            Thread Listen = new Thread(() =>
              {
                  try
                  {
                      while (true)
                      {
                          server.Listen(100);
                          Socket client = server.Accept();
                          ClientList.Add(client);
                          Thread receive = new Thread(Receive);
                          receive.IsBackground = true;
                          receive.Start(client);
                      }
                  }
                  catch
                  {
                      IP = new IPEndPoint(IPAddress.Any, 9999);
                      server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                  }
              });
            Listen.IsBackground = true;
            Listen.Start();
        }
        public void Send(Socket client,string message)
        {
            System.IO.StreamWriter streamWriter = System.IO.File.AppendText("Log.txt");
            streamWriter.Write("Send: " + message + "\n");
            streamWriter.Close();

            client.Send(Serialize(message));
        }
        void Sort()
        {
            for (int i = 0; i < ClientIDList.Count(); i++)
                for (int j = i + 1; j < ClientIDList.Count(); j++)
                    if (ClientIDList[i] > ClientIDList[j])
                    {
                        int x = ClientIDList[i];
                        ClientIDList[i] = ClientIDList[j];
                        ClientIDList[j] = x;
                        Socket socket = ClientList[i];
                        ClientList[i] = ClientList[j];
                        ClientList[j] = socket;
                    }
        }
        void Receive(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while(true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    string message = (string)DeSerialize(data);

                    System.IO.StreamWriter streamWriter = System.IO.File.AppendText("Log.txt");
                    streamWriter.Write("Receive: " + message + "\n");
                    streamWriter.Close();

                    if (message[0] == '0')
                    {
                        int x = int.Parse(message[2] + "");
                        ClientIDList.Add(x);
                        Sort();
                        continue;
                    }
                    mainWindow.Dispatcher.Invoke(() => mainWindow.SolveMessage(message));
                }
            }
            catch
            {
                for (int i = 0; i < ClientList.Count(); i++)
                    if (client == ClientList[i])
                    {
                        ClientIDList.Remove(ClientIDList[i]);
                        break;
                    }
                ClientList.Remove(client);
                client.Close();
                Sort();
            }
        }
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
        object DeSerialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
    }
}
