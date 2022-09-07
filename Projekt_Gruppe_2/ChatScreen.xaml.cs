using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Projekt_Gruppe_2_test;
using System.Threading;
using WpfAnimatedGif;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Projekt_Gruppe_2
{

    /// <summary>
    /// Interaktionslogik für ChatScreen.xaml
    /// </summary>
    public partial class ChatScreen : Window
    {

        private ObservableCollection<Message> _message = new ObservableCollection<Message>();
        private Message _selectedMessage;

        Message message = new Message()
        {
            Port = 13000,
            IPEmpfaenger = Globals.IPEmpfaenger,
            AliasSender = Globals.AliasSender
        };

        AliasEmpfänger empf = new AliasEmpfänger();
        TcpSender sender1 = new TcpSender();
        public ChatScreen()
        {
            Thread thread1 = new Thread(threadAufgabe);
            thread1.IsBackground = true;
            empf.AliasEmpf = Globals.empfName;
            InitializeComponent();
            lblNameEmpf.Content = "Chat mit " + empf.AliasEmpf;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => thread1.Start()));
           // thread1.Start();            
        }

        
        private void btnSenden_Click(object sender, RoutedEventArgs e)
        {          

            if (string.IsNullOrEmpty(message.DataFormat))
            {
                message.DataFormat = "textnachricht";

                //die Nachricht die übermittelt werden soll wird in einem Bytearray geschrieben
                Byte[] payload = Encoding.ASCII.GetBytes(textboxNachricht.Text);

                //setzte vom Objekt den Payload
                message.Payload = payload;
            }


            //setzte vom Objekt die aktuelle Zeit
            DateTime localDate = DateTime.Now;
            long unixTime = ((DateTimeOffset)localDate).ToUnixTimeSeconds();
            message.TimestampUnix = unixTime;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                //versuch meine ip-Adresse zu bekommen, da wir mehrere IP-Adressen haben, muss die richtige gefunden werden
                //wir verbinden uns hierfür mit einem UDP-Socket und lesen dann dessen lokalen Endpunkt aus
                string localIP = endPoint.Address.ToString();

                //setzte vom Objekt die IP-Empfänger auf die gefundene Ip-Adresse
                message.IPSender = localIP;
            }
            
            //erstelle aus dem Objekt einen String
                string stringjson = JsonConvert.SerializeObject(message);
        

            
                //starte die Methode senden mit der IP-Empfänger, dem stringjson und dem port
                sender1.senden(Globals.IPEmpfaenger, stringjson, message.Port);
                Globals.messageList.Add(message);
                //setzte DataFormat wieder auf null
                message.DataFormat = string.Empty;
            if (textboxNachricht.Text != string.Empty)
            {                
                DateTime datetime = UnixTimeStampToDateTime(message.TimestampUnix);
                string date = datetime.ToString("yyyy-MM-dd");
                if (date == Globals.date)
                {
                    string time = datetime.ToString("HH:mm:ss");
                    listChat.Items.Add(time + " " + Globals.AliasSender + ": " + textboxNachricht.Text);
                }
                else
                {
                    listChat.Items.Add(datetime + " " + Globals.AliasSender + ": " + textboxNachricht.Text);                                
                    Globals.date = date;
                }
            }   
            else if (textboxNachricht.Text == string.Empty)
            {
                MessageBox.Show("Bitte gib eine Nachricht ein.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            textboxNachricht.Clear();

            //zum Ende scrollen
            listChat.TabIndex = listChat.Items.Count - 1;


        }

        internal static void AddItem()
        {
            throw new NotImplementedException();
        }

        public static void threadAufgabe()
        {            
            TcpEmpfaenger empfaenger = new TcpEmpfaenger();
            int port = 13000;
            empfaenger.empfangen(port);    
            
        }
                        

        private void textboxNachricht_GotMouseCapture(object sender, MouseEventArgs e)
        {
            textboxNachricht.Clear();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new Verbindung();            
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
                string extension = System.IO.Path.GetExtension(path);
                //SaveByteArrayToFileWithFileStream(data, extension);
                textboxNachricht.Text = openFileDlg.FileName;
                message.Payload = data;
                message.DataFormat = extension;
            }
        }

        private void textboxNachricht_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSenden_Click(sender, e);                
            }
        }

        private void btnKonfetti_Click(object sender, RoutedEventArgs e)
        {
            if (konfettiGif.Visibility == Visibility.Hidden)
            {
                konfettiGif.Visibility = Visibility.Visible;
                listChat.Visibility = Visibility.Hidden;
                textboxNachricht.Visibility = Visibility.Hidden;
            }
            else if (konfettiGif.Visibility == Visibility.Visible)
            {
                konfettiGif.Visibility = Visibility.Hidden;
                listChat.Visibility = Visibility.Visible;
                textboxNachricht.Visibility = Visibility.Visible;
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
            /*if(Globals.Payload != string.Empty)
            {
                SoundPlayerAction soundPlayerAction = new SoundPlayerAction();
                soundPlayerAction.Source = new Uri(@"Sound\chat.wav", UriKind.RelativeOrAbsolute);

                EventTrigger eventTrigger = new EventTrigger(); // this is the event you want to trigger the sound effect.

                eventTrigger.Actions.Add(soundPlayerAction);
                Triggers.Add(eventTrigger); // Add this event trigger to Window.Triggers collection.
            }*/
            listChat.Items.Add(Globals.Payload);
            Globals.Payload = string.Empty;

            //zum Ende scrollen
            listChat.TabIndex = listChat.Items.Count - 1;


        }

        public void AddItem(string s)
        {
            listChat.Items.Add(s);
        }

    }

}
