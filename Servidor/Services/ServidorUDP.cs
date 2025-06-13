using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Servidor.Models;
using System.Diagnostics;

namespace Servidor.Services
{
    public class ServidorUDP
    {
        UdpClient servidor;
        public ServidorUDP()
        {
            servidor = new(54003);
            servidor.EnableBroadcast = true;

            Thread hilo = new(RecibirRespuesta);
            hilo.IsBackground = true;
            hilo.Start();
        }
        public event Action<string, string, string>? RespuestaRecibida;

        void RecibirRespuesta()
        {
            while (true)
            {
                try
                {
                    IPEndPoint cliente = new(IPAddress.Any, 0);
                    byte[] buffer = servidor.Receive(ref cliente);
                    var json = Encoding.UTF8.GetString(buffer);
                    
                    Debug.WriteLine($"Respuesta recibida de {cliente.Address}:{cliente.Port}");
                    Debug.WriteLine($"JSON recibido: {json}");
                    
                    var rect = JsonSerializer.Deserialize<Respuesta>(json);

                    if (rect != null)
                    {
                        Debug.WriteLine($"Respuesta deserializada - Nombre: {rect.nombre}, Respuesta: {rect.respuesta}, EndpointId: {rect.EndpointId}");
                        rect.EndpointId = $"{cliente.Address}:{cliente.Port}";
                        RespuestaRecibida?.Invoke(rect.nombre, rect.respuesta, rect.EndpointId);
                    }
                    else
                    {
                        Debug.WriteLine("Error: La respuesta deserializada es null");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error al recibir respuesta: {ex.Message}");
                }
            }
        }
    }
}
