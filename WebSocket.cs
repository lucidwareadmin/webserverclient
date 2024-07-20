using System.Net.WebSockets;
using System.Text;

public class WebSocket{
private string url;
    public WebSocket(string _url)
    {
        url = _url;
    }
    public async Task Connect()
    {
        ClientWebSocket webSocket = new ClientWebSocket();

    try
    {
        Console.WriteLine($"Connecting to WebSocket server at: {url}");
        await webSocket.ConnectAsync(new Uri(url), CancellationToken.None);

        if (webSocket.State == WebSocketState.Open)
        {
            Console.WriteLine($"WebSocket connected!");

            // Start sending and receiving messages
            await Task.WhenAll(Receive(webSocket), Send(webSocket));
        }
        else
        {
            Console.WriteLine($"WebSocket connection failed with state: {webSocket.State}");
        }
    }
    catch (WebSocketException ex)
    {
        Console.WriteLine($"WebSocket connection failed: {ex.Message}");
    }
    finally
    {
        if (webSocket.State == WebSocketState.Open)
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        webSocket.Dispose();
    }
    }

    static async Task Send(ClientWebSocket webSocket)
    {
        while (webSocket.State == WebSocketState.Open)
        {
            Console.Write("Enter a message (or 'exit' to close): ");
            string message = Console.ReadLine();

            if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "User requested closure", CancellationToken.None);
                break;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    static async Task Receive(ClientWebSocket webSocket)
    {
        byte[] buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received message: {message}");
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("WebSocket closed by the server.");
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed connection", CancellationToken.None);
            }
        }
    }
}