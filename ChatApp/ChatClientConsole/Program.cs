using ChatClient.Net;

namespace ChatClientConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            while (true)
            {
                if (!server._connected)
                {
                    try
                    {
                        Console.Write("Host: ");
                        string host = Console.ReadLine();
                        Console.Write("Port: ");
                        int port = int.Parse(Console.ReadLine());
                        Console.Write("Username: ");
                        string username = Console.ReadLine();
                        Console.Title = $"({username}) !exit to disconnect";
                        server.ConnectToServer(username, host, port);
                    }
                    catch
                    {
                        Console.WriteLine("Invalid data has been inserted");
                        Thread.Sleep(2000);
                        Console.Clear();
                    }
                }
                //Console.Clear();
                //Console.WriteLine("Docker internal connection: 'host.docker.internal'");
                //Thread.Sleep(1000);
            }
        }
    }
}
