class Program
{
    static async Task Main(string[] args)
    {
        string url = "ws://localhost:5000/ws";
        //string url = "ws://lucidware.ca/ws";
        WebSocket socket = new WebSocket(url);
        await socket.Connect();
    }
}