using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

public class UdpChatServer
{
    class Program
    {
        static ServerObject server; // сервер
        static Thread listenThread; // потока для прослушивания
        static void Main(string[] args)
        {
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        List<ClientObject> clients = new List<ClientObject>(); // все подключения

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
                clients.Remove(client);
        }
        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 5001);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message, string from,string to)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].ip.ToString() == to ||(to == "79.174.44.240" && clients[i].ip.ToString() == "192.168.1.1"))
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
                Console.WriteLine(String.Format("{0} to {1}", clients[i].ip.ToString(), to));
            }
        }
        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
    }
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal IPAddress ip { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
            ip = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message;
                // получаем имя пользователя
                // посылаем сообщение о входе в чат всем подключенным пользователям
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        var parts = message.Split(' ');
                        string a = Convert.ToString(parts[0]);
                        if (a == "/send")
                        {
                            Console.WriteLine(a);
                            string from = Convert.ToString(parts[2]);
                            string to = Convert.ToString(parts[4]);
                            Console.WriteLine(to);
                            Console.WriteLine(from);
                            string text = message.Remove(0, message.IndexOf(':') + 1);
                            string m = from + ":" + text + Environment.NewLine;
                            MySqlCommand command = new MySqlCommand(); ;
                            string connectionString, commandString;
                            connectionString = "Data source=79.174.44.240;UserId=triu;Password=120evazus;database=triumph;";
                            MySqlConnection connection = new MySqlConnection(connectionString);
                            commandString = string.Format("SELECT Ip FROM chat WHERE Nickname='{0}'",to);
                            command.CommandText = commandString;
                            command.Connection = connection;
                            MySqlDataReader reader;
                            command.Connection.Open();
                            reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                    to = reader["Ip"].ToString();        
                            }
                            reader.Close();
                            command.Connection.Close();
                            server.BroadcastMessage(m, from, to);
                        }
                    }
                    catch(Exception e)
                    {
                        string connectionString,sss;
                        connectionString = "Data source=79.174.44.240;UserId=triu;Password=120evazus;database=triumph;";
                        MySqlConnection connection2 = new MySqlConnection(connectionString);
                        if (ip.ToString() != "192.168.1.1")
                            sss = String.Format("UPDATE chat SET Online='false' WHERE Ip='{0}'", ip);
                        else sss = String.Format("UPDATE chat SET Online='false' WHERE Ip='79.174.44.240'", ip);
                        MySqlCommand cmd = new MySqlCommand(sss, connection2);
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                        Console.WriteLine(e);
                        message = String.Format("{0}: покинул чат", this.Id);
                        Console.WriteLine(message);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            byte[] data = new byte[1024]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }

}