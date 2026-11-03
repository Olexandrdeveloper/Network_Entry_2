using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Entry_Network2_Client
{
    internal class Client
    {
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            clientSocket.Connect(serverEndPoint);

            _ = Task.Run(() =>
            {
                try
                {
                    byte[] bytes = new byte[1024];
                    while (true)
                    {
                        int bytesRec = clientSocket.Receive(bytes);
                        if (bytesRec == 0) break;

                        string message = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        Console.Write(message);
                    }
                }
                catch
                {
                }
            });

            try
            {
                while (true)
                {
                    string input = Console.ReadLine() ?? string.Empty;
                    if (!string.IsNullOrEmpty(input))
                    {
                        clientSocket.Send(Encoding.UTF8.GetBytes(input));
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Помилка відправки");
            }
            finally
            {
                clientSocket.Close();
            }
        }
    }
}
