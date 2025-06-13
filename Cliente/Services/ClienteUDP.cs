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
    public class ClienteUDP
    {
        UdpClient cliente;
        IPEndPoint conexion;
        private string endpointId;

        public ClienteUDP()
        {
            try
            {
                cliente = new UdpClient();
                cliente.EnableBroadcast = true;
                
                cliente.Client.Bind(new IPEndPoint(IPAddress.Any, 0));
                var localEndpoint = (cliente.Client.LocalEndPoint as IPEndPoint)!;
                endpointId = $"{localEndpoint.Address}:{localEndpoint.Port}";
                Debug.WriteLine($"Cliente UDP inicializado con endpoint: {endpointId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al inicializar el cliente UDP: {ex.Message}");
                throw;
            }
        }

        public void Enviar(Respuesta res)
        {
            try
            {
                conexion = new(IPAddress.Broadcast, 54003);
                res.EndpointId = endpointId;
                var json = JsonSerializer.Serialize(res);
                Debug.WriteLine($"Enviando respuesta - Nombre: {res.nombre}, Respuesta: {res.respuesta}, EndpointId: {res.EndpointId}");
                Debug.WriteLine($"JSON a enviar: {json}");
                
                byte[] mensajeBinario = Encoding.UTF8.GetBytes(json ?? "Error#404XD");
                cliente.Send(mensajeBinario, mensajeBinario.Length, conexion);
                Debug.WriteLine("Respuesta enviada correctamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al enviar respuesta: {ex.Message}");
                throw;
            }
        }

        public void Cerrar()
        {
            try
            {
                cliente.Close();
                Debug.WriteLine("Cliente UDP cerrado correctamente");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cerrar el cliente UDP: {ex.Message}");
            }
        }
    }
}
