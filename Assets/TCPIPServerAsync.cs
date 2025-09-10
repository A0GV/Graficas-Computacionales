using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TCPIPServerAsync : MonoBehaviour
{
    Thread SocketThread;
    volatile bool keepReading = false;

    Socket listener;
    Socket handler;

    // Agrega referencia pública a CruceBehavior
    public CruceBehavior cruceBehavior;

    void Start()
    {
        Application.runInBackground = true;
        // Si no se asignó en el inspector, búscalo automáticamente
        if (cruceBehavior == null)
            cruceBehavior = FindFirstObjectByType<CruceBehavior>();
        startServer();
    }

    void startServer()
    {
        SocketThread = new Thread(networkCode);
        SocketThread.IsBackground = true;
        SocketThread.Start();
    }

    private string getIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
            }
        }
        return localIP;
    }

    void networkCode()
    {
        // Data buffer for incoming data.
        byte[] bytes = new byte[4096];

        //Create EndPoint
        IPAddress IPAdr = IPAddress.Parse("127.0.0.1");
        IPEndPoint localEndPoint = new IPEndPoint(IPAdr, 1101);

        // Create a TCP/IP socket.
        listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);
            Debug.Log("Servidor TCP escuchando en " + localEndPoint.ToString());
            while (true)
            {
                Debug.Log("Waiting for Connection");
                handler = listener.Accept();
                Debug.Log("Client Connected");
                keepReading = true;
                while (keepReading && handler != null && handler.Connected)
                {
                    try
                    {
                        bytes = new byte[4096];
                        int bytesRec = handler.Receive(bytes);
                        if (bytesRec <= 0)
                        {
                            Debug.Log("Cliente desconectado");
                            keepReading = false;
                            break;
                        }
                        string data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        UnityEngine.Debug.Log($"Mensaje recibido: {data}");
                        // Parsear mensaje tipo: Spawn {path_id} ({path.turn_type}) from {direction} at {spawn_pos}$
                        if (data.StartsWith("Spawn "))
                        {
                            try
                            {
                                // Ejemplo: Spawn 9 (right) from south at (16, -8)$
                                // Quitar el $ final si existe
                                string cleanData = data.TrimEnd('$', '\n', '\r');
                                // Buscar "Spawn " y " ("
                                int idxPath = cleanData.IndexOf(' ');
                                int idxParen = cleanData.IndexOf('(');
                                int idxFrom = cleanData.IndexOf("from ");
                                int idxAt = cleanData.IndexOf("at ");
                                if (idxPath > -1 && idxParen > idxPath && idxFrom > idxParen && idxAt > idxFrom)
                                {
                                    string pathIdStr = cleanData.Substring(idxPath + 1, idxParen - idxPath - 2).Trim();
                                    int pathId = int.Parse(pathIdStr);
                                    string spawnPosStr = cleanData.Substring(idxAt + 3).Trim();
                                    UnityEngine.Debug.Log($"[PARSE] path_id: {pathId}, spawn_pos: {spawnPosStr}");

                                    // Encolar el pathId para spawn en CruceBehavior
                                    if (cruceBehavior != null)
                                    {
                                        cruceBehavior.EnqueueCarToSpawn(pathId);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                UnityEngine.Debug.Log($"[PARSE ERROR] {ex.Message}");
                            }
                        }
                    }
                    catch (SocketException ex)
                    {
                        Debug.Log($"Cliente desconectado: {ex.Message}");
                        keepReading = false;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Error general: {ex.Message}");
                        keepReading = false;
                        break;
                    }
                }
                // Cerrar conexión actual
                if (handler != null)
                {
                    try
                    {
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Error al cerrar conexión: {ex.Message}");
                    }
                    handler = null;
                }
                Debug.Log("Conexión cerrada, esperando nueva conexión...");
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error en el servidor: {e.ToString()}");
        }
        finally
        {
            if (listener != null)
            {
                listener.Close();
            }
        }
    }

    void stopServer()
    {
        keepReading = false;

        if (handler != null)
        {
            try
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception ex)
            {
                Debug.Log($"Error al cerrar handler: {ex.Message}");
            }
            handler = null;
        }

        if (listener != null)
        {
            try
            {
                listener.Close();
            }
            catch (Exception ex)
            {
                Debug.Log($"Error al cerrar listener: {ex.Message}");
            }
            listener = null;
        }

        if (SocketThread != null && SocketThread.IsAlive)
        {
            SocketThread.Join(1000); // Esperar hasta 1 segundo
            if (SocketThread.IsAlive)
            {
                SocketThread.Abort();
            }
        }

        Debug.Log("Servidor detenido");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            stopServer();
        }
    }

    void OnApplicationQuit()
    {
        stopServer();
    }

    void OnDisable()
    {
        stopServer();
    }
}