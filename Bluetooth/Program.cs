using Bluetooth.Controllers;
using InTheHand.Net.Sockets;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // WebSocketClient'taki işlemleri çağır
        await WebSocketServer();
    }

    static async Task WebSocketServer()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5002/");
        listener.Start();

        Console.WriteLine("Server listening...");

        while (true)
        {
            var context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);
                var webSocket = webSocketContext.WebSocket;
                await ProcessWebSocketRequest(context, webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    static async Task ProcessWebSocketRequest(HttpListenerContext context, WebSocket webSocket)
    {
        Console.WriteLine("WebSocket connected.");

        try
        {
            var buffer = new byte[1024];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received message: {message}");

                // Cihaz adlarını düz metin olarak al ve JSON formatına dönüştür
                var deviceNames = message.Split(',').Select(device => device.Trim());

                // Create a JSON object with 'command' and 'devices' properties
                var responseObject = new
                {
                    command = "getDevices",
                    devices = deviceNames
                };

                var jsonResponse = JsonConvert.SerializeObject(responseObject);

                // WebSocket üzerinden JSON formatında gönder
                var responseMessage = Encoding.UTF8.GetBytes(jsonResponse);
                await webSocket.SendAsync(new ArraySegment<byte>(responseMessage), WebSocketMessageType.Text, true, CancellationToken.None);

                Console.WriteLine("Cihaz listesi WebSocket üzerinden gönderildi.");

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            Console.WriteLine("WebSocket disconnected.");
        }
    }
}
