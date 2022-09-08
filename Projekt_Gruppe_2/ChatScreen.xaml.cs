using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Projekt_Gruppe_2_test;
using System.Threading;
using System.Media;
using System.Security.Cryptography;

namespace Projekt_Gruppe_2
{

    /// <summary>
    /// Interaktionslogik für ChatScreen.xaml
    /// </summary>
    public partial class ChatScreen : Window
    {
        Message message = new Message()
        {
            Port = 13000,
            IPEmpfaenger = Globals.IPEmpfaenger,
            AliasSender = Globals.AliasSender
        };

        AliasReceiver aliasReceiver = new AliasReceiver();
        TCPSender sender1 = new TCPSender();
        public ChatScreen()
        {
            Thread thread1 = new Thread(threadDoWork);
            
            aliasReceiver.aliasReceiver = Globals.empfName;
            InitializeComponent();
            lblNameReceiver.Content = "Chat mit " + aliasReceiver.aliasReceiver;
            thread1.Start();
        }

        
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {          

            if (string.IsNullOrEmpty(message.DataFormat))
            {
                message.DataFormat = "textnachricht";

                //die Nachricht die übermittelt werden soll wird in einem Bytearray geschrieben
                //Byte[] payload = Encoding.ASCII.GetBytes(textboxMessage.Text);
                Byte[] payload = Encoding.ASCII.GetBytes(encryption(textboxMessage.Text));

                //setzte vom Objekt den Payload
                message.Payload = payload;
            }

            //setzte vom Objekt die aktuelle Zeit
            DateTime localDate = DateTime.Now;
            long unixTime = ((DateTimeOffset)localDate).ToUnixTimeSeconds();
            message.TimestampUnix = unixTime;
            
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.ConnectAsync("8.8.8.8", 65530);
                //socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                //versuch meine ip-Adresse zu bekommen, da wir mehrere IP-Adressen haben, muss die richtige gefunden werden
                //wir verbinden uns hierfür mit einem UDP-Socket und lesen dann dessen lokalen Endpunkt aus
                string localIP = endPoint.Address.ToString();

                //setzte vom Objekt die IP-Empfänger auf die gefundene Ip-Adresse
                message.IPSender = localIP;
            }
            
            //erstelle aus dem Objekt einen String und verschlüssele es
            string stringjson = JsonConvert.SerializeObject(message);
            
            //encryption(stringjson);
                        
            //starte die Methode senden mit der IP-Empfänger, dem stringjson und dem port
            sender1.send(Globals.IPEmpfaenger, stringjson, message.Port);

            Globals.messageList.Add(message);

            //setzte DataFormat wieder auf null
            message.DataFormat = string.Empty;

            if (textboxMessage.Text != string.Empty)
            {                
                DateTime datetime = UnixTimeStampToDateTime(message.TimestampUnix);
                string date = datetime.ToString("yyyy-MM-dd");
                if (date == Globals.date)
                {
                    string time = datetime.ToString("HH:mm:ss");
                    listChat.Items.Add(time + " " + Globals.AliasSender + ": " + textboxMessage.Text);
                }
                else
                {
                    listChat.Items.Add(datetime + " " + Globals.AliasSender + ": " + textboxMessage.Text);                                
                    Globals.date = date;
                }
            }   
            else if (textboxMessage.Text == string.Empty)
            {
                MessageBox.Show("Bitte gib eine Nachricht ein.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            textboxMessage.Clear();

            //zum Ende scrollen
            listChat.TabIndex = listChat.Items.Count - 1;
        }


        public static void threadDoWork()
        {            
            TCPReceiver tcpReceiver = new TCPReceiver();
            int port = 13000;
            tcpReceiver.receive(port);                
        }
                        

        private void textboxMessage_GotMouseCapture(object sender, MouseEventArgs e)
        {
            textboxMessage.Clear();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new Connection();            
            this.Close();
            newWindow.Show();
        }

        private void btnData_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog openFileDlg = new OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();

            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            { 
                byte[] data = StreamFile(openFileDlg.FileName);
                string path = openFileDlg.FileName;
                string extension = Path.GetExtension(path);

                //SaveByteArrayToFileWithFileStream(data, extension);
                textboxMessage.Text = openFileDlg.FileName;
                message.Payload = data;
                message.DataFormat = extension;
            }
        }

        private void textboxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSend_Click(sender, e);                
            }
        }

        private void btnKonfetti_Click(object sender, RoutedEventArgs e)
        {
            if (konfettiGif.Visibility == Visibility.Hidden)
            {
                konfettiGif.Visibility = Visibility.Visible;
                listChat.Visibility = Visibility.Hidden;
                textboxMessage.Visibility = Visibility.Hidden;
            }
            else if (konfettiGif.Visibility == Visibility.Visible)
            {
                konfettiGif.Visibility = Visibility.Hidden;
                listChat.Visibility = Visibility.Visible;
                textboxMessage.Visibility = Visibility.Visible;
            }
        }

        public byte[] StreamFile(string filename)
        {
            byte[] fileData = null;

            using (FileStream fs = File.OpenRead(filename))
            {
                var binaryReader = new BinaryReader(fs);
                fileData = binaryReader.ReadBytes((int)fs.Length);
            }
            return fileData;
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        private void btnAktualisieren_Click(object sender, RoutedEventArgs e)
        {
            /*
            listChat.Items.Add(Globals.Payload);
            Globals.Payload = string.Empty;

            //zum Ende scrollen
            listChat.TabIndex = listChat.Items.Count - 1;
            */
        }

        /*
        public static string GetRandomKey(int length)
        {
            byte[] rgb = new byte[length];
            RNGCryptoServiceProvider rngCrypt = new RNGCryptoServiceProvider();
            rngCrypt.GetBytes(rgb);
            return Convert.ToBase64String(rgb);
        }
        */

        private string encryption(string _text)
        {
            //Globals.key = GetRandomKey(32);            
            Globals.key = "Xn2r5u8x/A?D(G+KbPeShVmYq3s6v9y$";          
            var encryptedString = AesOperation.EncryptString( Globals.key, _text);
            
            return encryptedString;
        }        
    }
}
