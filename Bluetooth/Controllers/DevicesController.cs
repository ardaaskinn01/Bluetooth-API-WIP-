using InTheHand.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bluetooth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController
    {
        private readonly BluetoothClient _bluetoothClient;

        public DevicesController()
        {
            _bluetoothClient = new BluetoothClient();
        }

        public class WebSocketMessage
        {
            public string Command { get; set; }
        }

        // In DevicesController, modify the return type of GetDevices to return a string instead of IEnumerable<BluetoothDeviceInfo>
        public async Task<string> GetDevices()
        {
            Console.WriteLine("GetDevices fonksiyonu çağrıldı.");
            try
            {
                var devices = await Task.Run(() => _bluetoothClient.DiscoverDevices());

                if (devices != null)
                {
                    Console.WriteLine("Cihaz listesi WebSocket üzerinden gönderildi.");
                    // Convert devices to JSON and return as a string
                    return JsonConvert.SerializeObject(devices);
                }
                else
                {
                    Console.WriteLine("Cihazlar null olarak alındı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cihazlar alınırken bir hata oluştu: {ex.Message}");
                return null;
            }

            return null;
        }

    }
}
