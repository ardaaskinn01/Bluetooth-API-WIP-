using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class WebSocketClient
{
    public static async Task ConnectAndSendMessage()
    {
        using (ClientWebSocket webSocket = new ClientWebSocket())
        {
            // WebSocket URL'sini burada belirtin
            Uri serverUri = new Uri("ws://localhost:5002/");

            // WebSocket bağlantısını aç
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);

            // Mesajı hazırla
            string message = "getDevices";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // Sürekli olarak veri gönder
                await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
