using Cliente.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Cliente.Services
{
    public class Recibir
    {
        private readonly UdpClient clienteReceptor;
        private readonly Thread hiloReceptor;

        public Recibir()
        {
            try
            {
                clienteReceptor = new UdpClient(54001);
                clienteReceptor.EnableBroadcast = true;

                Thread hilo = new(RecibirRespuesta);
                hilo.IsBackground = true;
                hilo.Start();
                Debug.WriteLine("Cliente receptor inicializado en puerto 54001");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al inicializar el cliente receptor: {ex.Message}");
                throw;
            }
        }

        public event Action<Pregunta>? RespuestaRecibida;

        private void RecibirRespuesta()
        {
            while (true)
            {
                try
                {
                    IPEndPoint remitente = new IPEndPoint(IPAddress.Any, 0);
                    Debug.WriteLine("Esperando pregunta...");
                    byte[] buffer = clienteReceptor.Receive(ref remitente);

                    string json = Encoding.UTF8.GetString(buffer);
                    Debug.WriteLine($"Pregunta recibida de {remitente.Address}:{remitente.Port}");
                    Debug.WriteLine($"JSON recibido: {json}");

                    var pregunta = JsonSerializer.Deserialize<Pregunta>(json);
                    if (pregunta != null)
                    {
                        Debug.WriteLine($"Pregunta deserializada correctamente: {pregunta.Enunciado}");
                        RespuestaRecibida?.Invoke(pregunta);
                    }
                    else
                    {
                        Debug.WriteLine("Error: La pregunta deserializada es null");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error al recibir la pregunta: {ex.Message}");
                    Task.Delay(1000).Wait(); 
                }
            }
        }
    }
}
