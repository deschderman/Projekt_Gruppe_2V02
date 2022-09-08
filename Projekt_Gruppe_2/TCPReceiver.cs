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
using System.Media;

namespace Projekt_Gruppe_2
{    
    public class TCPReceiver
    {        
        public void receive(int port)
        {
            //wir wollen von allen empfangen können, deshalb IPAddress.Any
            TcpListener tcpListener = new TcpListener(IPAddress.Any, port); 

            //starten des TCPListeners
            tcpListener.Start();

            //zum Lesen der Nachricht
            Byte[] bytes = new Byte[31575]; 
            string getMessage = null;

            while (true)
            {
                tcpListener.Start();

                //erstellen eines TCPClients
                TcpClient client = tcpListener.AcceptTcpClient();
                getMessage = null;

                //Daten empfangen
                NetworkStream stream = client.GetStream();
                int i;                               

                //den empfangenen Bytearray durchgehen
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    
                    //alles was in dem Bytearray drin steht zu den ursprünglichen Charaktern zurück konvertieren und entschlüsseln
                    getMessage = Encoding.ASCII.GetString(bytes, 0, i);

                    //aus dem String wieder ein Objekt der Klasse Message machen
                    Message message = JsonConvert.DeserializeObject<Message>(getMessage);

                    Globals.messageList.Add(message);
                    
                    if (message.DataFormat == "textnachricht")
                    {
                        sendMessage(message);
                        notification();
                    }
                    else
                    {
                        sendFile(message);
                        notification();
                    }

                    //ChatScreen cs = Application.Current.MainWindow as ChatScreen;
                    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,new Action(() => cs.AddItem(Globals.Payload)));

                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { ChatScreen chatScreen = new ChatScreen(); chatScreen.listChat.Items.Add(Globals.Payload); }));

                    //ChatScreen chatScreen = new ChatScreen();
                    //chatScreen.AddItem(Globals.Payload);
                    //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => chatScreen.AddItem(Globals.Payload)));
                }                                

                client.Close();
                tcpListener.Stop();
            }
        }

        private static void sendMessage(Message message)
        {
            //aus dem Bytearray des Payloads sollen auch wieder die ursprünglichen Charakter hergestellt werden
            string payload = Encoding.UTF8.GetString(message.Payload, 0, message.Payload.Length);
            payload = decryption(payload);
            DateTime datetime = UnixTimeStampToDateTime(message.TimestampUnix);
            string date = datetime.ToString("yyyy-MM-dd");

            //Nachricht bzw. Payload ausgeben
            if (date == Globals.date)
            {
                string time = datetime.ToString("HH:mm:ss");
                Globals.Payload = time + " " + Globals.empfName + ": " + payload;

            }
            else
            {
                Globals.Payload = datetime + " " + Globals.empfName + ": " + payload;
                Globals.date = date;
            }
        }

        private static void notification()
        {
            SoundPlayer player = new SoundPlayer("C:/Users/user/Documents/Praxisblock II/Projekt_Messenger/Projekt_Gruppe_2V02/Projekt_Gruppe_2/Sound/notification2.wav");
            player.Load();
            player.Play();
        }

        private static void sendFile(Message message)
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

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        private static string decryption(string _encryptedString)
        {            
            var decryptedString = AesOperation.DecryptString(Globals.key, _encryptedString);            
            return decryptedString;
        }
       
    }
}
