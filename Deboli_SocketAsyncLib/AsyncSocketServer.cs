using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;

namespace Deboli_SocketAsyncLib
{
    public class AsyncSocketServer
    {
        // Variabili per il server
        TcpListener myServer;
        IPAddress myIP;
        int myPort;

        // Variabili per il client
        List<TcpClient> myClient;

        public AsyncSocketServer()
        {
            myClient = new List<TcpClient>();
        }

        /// <summary>
        /// Metti in ascolto il server
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="nPort"></param>
        public async void ServerInAscolto(IPAddress ipAddress = null, int nPort = 23000)
        {
            // Controlli generali
            if (ipAddress == null)
            {
                ipAddress = IPAddress.Any;
            }

            if (nPort < 0 || nPort > 65535)
            {
                nPort = 23000;
            }

            // Assegno il valore alle variabili
            myIP = ipAddress;
            myPort = nPort;

            myServer = new TcpListener(myIP, myPort);

            Debug.WriteLine($"Server in ascolto alla porta {myPort} con l'indirizzo IP: {myIP}");

            myServer.Start();

            while (true)
            {
                TcpClient client = await myServer.AcceptTcpClientAsync();

                myClient.Add(client);
                
                // Debug.WriteLine($"I client connessi sono: {myClient.Count}");
                // Debug.WriteLine($"Client connesso: {client.Client.RemoteEndPoint}");

                RiceviMessaggio(client);
            }
        }

        /// <summary>
        /// Metodo che permette di ricevere al server un messaggio
        /// </summary>
        public async void RiceviMessaggio(TcpClient client)
        {
            NetworkStream stream = null;
            StreamReader reader = null;

            try
            {
                stream = client.GetStream();
                reader = new StreamReader(stream);
                char[] buffer = new char[512];
                int nBytes = 0;

                while (true)
                {
                    string richiestaIP = Convert.ToString(client.Client.RemoteEndPoint);
                    Debug.WriteLine(richiestaIP);
                    
                    //Debug.WriteLine("In attesa del messaggio");

                    // Ricezione messaggio asincrono
                    nBytes = await reader.ReadAsync(buffer, 0, buffer.Length);

                    if (nBytes == 0)
                    {
                        RimuoviClient(client);
                        //Debug.WriteLine("Client disconnesso");
                        break;
                    }

                    string recvText = new string(buffer);

                    if (recvText.Contains("time"))
                    {
                        InviaMessaggio(DateTime.Now.ToShortTimeString(), client);
                    }
                    else if (recvText.Contains("date"))
                    {
                        InviaMessaggio(DateTime.Now.ToShortDateString(), client);
                    }
                    else
                    {
                        InviaMessaggio("Non ho capito", client);
                    }

                    //Debug.WriteLine($"Cleinte: {client.Client.RemoteEndPoint} |||| N byte: {nBytes}; Messaggio: {recvText}");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Errroe: " + ex.Message);
            }
        }

        /// <summary>
        /// Invia un messaggio al client 
        /// </summary>
        public void InviaMessaggio(string messaggio, TcpClient client)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(messaggio);
                client.GetStream().WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Rimuovi il client dal server
        /// </summary>
        private void RimuoviClient(TcpClient client)
        {
            if (myClient.Contains(client))
            {
                myClient.Remove(client);
            }
        }

        /// <summary>
        /// Chiudi la connessione tra il server e il/i client
        /// </summary>
        public void DisconnettiServer()
        {
            try
            {
                foreach (TcpClient client in myClient)
                {
                    client.Close();
                    RimuoviClient(client);
                    Debug.WriteLine($"Client {myClient.Count} disconnesso");
                }

                myServer.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Errore: " + ex.Message);
            }
        }
    }
}
