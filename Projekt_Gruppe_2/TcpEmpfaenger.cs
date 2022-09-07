using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace Projekt_Gruppe_2
{    


    public class TcpEmpfaenger
    {        
        public void empfangen(int port)
        {
            //wir wollen von allen empfangen können, deshalb IPAddress.Any
            TcpListener empfaenger = new TcpListener(IPAddress.Any, port); 

            //starten des TCPListeners
            empfaenger.Start();

            //zum Lesen der Nachricht
            Byte[] bytes = new Byte[31575]; 
            string nachricht = null;

            while (true)
            {
                empfaenger.Start();

                //erstellen eines TCPClients
                TcpClient client = empfaenger.AcceptTcpClient();
                nachricht = null;

                //Daten empfangen
                NetworkStream stream = client.GetStream();
                int i;

                

                //den empfangenen Bytearray durchgehen
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //alles was in dem Bytearray drin steht zu den ursprünglichen Charaktern zurück konvertieren
                    nachricht = Encoding.ASCII.GetString(bytes, 0, i);

                    //aus dem String wieder ein Objekt der Klasse Message machen
                    Message message = JsonConvert.DeserializeObject<Message>(nachricht);

                    Globals.messageList.Add(message);
                    
                    if (message.DataFormat == "textnachricht")
                    {
                        //aus dem Bytearray des Payloads sollen auch wieder die ursprünglichen Charakter hergestellt werden
                        string payload = Encoding.UTF8.GetString(message.Payload, 0, message.Payload.Length);
                        DateTime datetime = UnixTimeStampToDateTime(message.TimestampUnix);
                        string date = datetime.ToString("yyyy-MM-dd");
                        if (date == Globals.date)
                        {
                            string time = datetime.ToString("HH:mm:ss");
                            Globals.Payload =time + " " + Globals.empfName + ": " + payload;

                        }
                        else
                        {
                            Globals.Payload = datetime+ " " + Globals.empfName + ": " + payload;
                            Globals.date = date;
                        }
                        
                        //Nachricht bzw. Payload ausgeben
                        //Globals.Payload = Globals.empfName + ": " + payload + "\t\t\t" + datetime;
                    }
                    else
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        // Launch OpenFileDialog by calling ShowDialog method
                        Nullable<bool> result = saveFileDialog.ShowDialog();

                        // Get the selected file name and display in a TextBox.
                        // Load content of file in a TextBlock
                        if (result == true)
                        {
                            string filepath = saveFileDialog.FileName + message.DataFormat;
                            using var stream1 = File.Create(filepath);
                            stream1.Write(message.Payload, 0, message.Payload.Length);
                        }
                    }

                    
                    //ChatScreen cs = Application.Current.MainWindow as ChatScreen;
                    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,new Action(() => cs.AddItem(Globals.Payload)));

                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ChatScreen chatScreen = new ChatScreen(); }));

                    //ChatScreen chatScreen = new ChatScreen();
                    //chatScreen.AddItem(Globals.Payload);


                    //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => chatScreen.AddItem(Globals.Payload)));
                }
                                

                client.Close();
                empfaenger.Stop();
            }
        }
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
       
    }
}
