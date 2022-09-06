using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace Projekt_Gruppe_2
{

    /// <summary>
    /// Interaktionslogik für Verbindung.xaml
    /// </summary>
    /// 
    public static class Globals
    {
        public static string empfName;
        public static string IPEmpfaenger;
        public static string AliasSender;
        public static string Payload;
        public static string date;
        public static string fillList;
        public static List<Message> messageList;
        public static int messageCounter;
    }

    public partial class Verbindung : Window
    { 
        Message message = new Message()
        {
            Port = 13000
        };
        
        public Verbindung()
        {
            InitializeComponent();
            txtNameSender.Text = Globals.AliasSender;
            message.AliasSender = txtNameSender.Text;
            txtPort.Text = Convert.ToString(message.Port);

            string localIP = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();

                //setzte vom Objekt die IP-Empfänger auf die gefundene Ip-Adresse
                message.IPSender = localIP;
                txtIPSender.Text = localIP;
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {

            if (txtNameEmpf.Text == string.Empty)
            {
                MessageBox.Show("Bitte einen Empfänger eingeben.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            } else if (txtNameEmpf.Text != string.Empty)
            {
                AliasEmpfänger empf = new AliasEmpfänger();
                empf.AliasEmpf = txtNameEmpf.Text;
                Globals.empfName = txtNameEmpf.Text;
            }
            
            TcpSender sender1 = new TcpSender();
            string ipadresse = txtIpEmpf.Text;

            bool IsIP(string ipadresse)
            {
                return System.Text.RegularExpressions.Regex.IsMatch(ipadresse, @"\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?).){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$\b");
            }

            if (!IsIP(ipadresse))
            {
                MessageBox.Show("Bitte eine IP-Adresse eingeben.","Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                txtIpEmpf.Clear();
                return;
            }else
                if (IsIP(ipadresse))
            {
                //setzte vom Objekt die Ip-Adresse
                message.IPEmpfaenger = ipadresse;
                Globals.IPEmpfaenger = ipadresse;
                              
                var newWindow = new ChatScreen();
                this.Close();
                newWindow.Show();
            }

        }
        private void txtNameEmpf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnConnect_Click(sender, e);
            }
        }

    }
}
