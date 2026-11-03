using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Network_Entry_2
{
    internal class Server
    {
        public static IPEndPoint IPEndPoint = new IPEndPoint(IPAddress.Any, 8888);
        public static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static bool isRunning = true;

        static void Main(string[] args)
        {
            serverSocket.Bind(IPEndPoint);
            serverSocket.Listen();

            try
            {
                while (isRunning)
                {
                    Socket socketClient = serverSocket.Accept();
                    Task.Run(() => HandleClient(socketClient));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void HandleClient(Socket socketClient)
        {
            try
            {
                while (isRunning)
                {
                    socketClient.Send(Encoding.UTF8.GetBytes("1. Add Message\n2. Exit\n> "));

                    byte[] bytes = new byte[1024];
                    int bytesRec = socketClient.Receive(bytes);
                    if (bytesRec == 0) break;

                    string choice = Encoding.UTF8.GetString(bytes, 0, bytesRec).Trim();

                    switch (choice)
                    {
                        case "1":
                            AddMessage(socketClient);
                            break;
                        case "2":
                            return;
                        default:
                            socketClient.Send(Encoding.UTF8.GetBytes("Невірний вибір\n"));
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                socketClient.Close();
            }
        }

        static void AddMessage(Socket socketClient)
        {
            byte[] bytes = new byte[1024];

            socketClient.Send(Encoding.UTF8.GetBytes("Введіть повідомлення: "));
            int bytesRec = socketClient.Receive(bytes);
            string data = Encoding.UTF8.GetString(bytes, 0, bytesRec).Trim();

            socketClient.Send(Encoding.UTF8.GetBytes("Введіть затримку (у секундах): "));
            int bytesRec2 = socketClient.Receive(bytes);
            string data2 = Encoding.UTF8.GetString(bytes, 0, bytesRec2).Trim();

            int delay = int.TryParse(data2, out int parsedDelay) ? parsedDelay : 0;

            socketClient.Send(Encoding.UTF8.GetBytes($"Нагадування додано! Спрацює через {delay} сек.\n\n"));

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(delay * 1000);
                    socketClient.Send(Encoding.UTF8.GetBytes($"\n[НАГАДУВАННЯ]: {data}\n> "));
                }
                catch
                {
                }
            });
        }
    }
}
