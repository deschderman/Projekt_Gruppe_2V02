using System.Collections.Generic;using System.Windows;
using System.Windows.Input;

namespace Projekt_Gruppe_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public static class Globals
    {
        public static string empfName;
        public static string IPEmpfaenger;
        public static string AliasSender;
        public static string Payload;
        public static string date;
        public static List<Message> messageList = new List<Message>();
        public static string key;
        public static int messageCounter;
        public static int port;
    }

    public partial class MainWindow : Window
    {
        
        string name = "";
        string pw = "";

        public MainWindow()
        {            
            InitializeComponent();                
            pwdbox.Clear();
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void txtboxUsername_GotMouseCapture(object sender, MouseEventArgs e)
        {
            txtboxUsername.Clear();
            txtboxUsername.FontStyle = FontStyles.Normal;
        }

        private void btnAnmelden_Click(object sender, RoutedEventArgs e)
        {
            validierung();
        }

        private void pwdbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                validierung();
            }
        }

        private void validierung()
        {
            ///*
            bool valid = false;/*
            using (DB db = new DB())
                foreach (User user in db.Benutzer)
                {
                    name = user.AliasSender;
                    pw = user.Passwort;

                    if (name == txtboxUsername.Text)
                    {
                        if (pw == pwdbox.Password)
                       */ {
                            Globals.AliasSender = txtboxUsername.Text;   
                            if(txtboxUsername.Text == string.Empty || txtboxUsername.Text == "Username")
                            {  
                                MessageBox.Show("Bitte einen Usernamen eingeben", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;                             
                            }else if (txtboxUsername.Text != string.Empty)
                            {
                                valid = true;
                                var newWindow = new Connection();
                                this.Close();
                                newWindow.Show();
                            }
                                                        
                }
            if (valid == false)
            {
                falscheEingabe();                
            }
        }

        private void falscheEingabe()
        {
            MessageBox.Show("Username oder Passwort ist falsch.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            txtboxUsername.Text = "Username";
            pwdbox.Clear();
        }
      }
   }

