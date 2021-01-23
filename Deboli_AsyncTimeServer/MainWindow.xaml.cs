using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Deboli_SocketAsyncLib;

namespace Deboli_AsyncTimeServer
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AsyncSocketServer myServer;

        public MainWindow()
        {
            InitializeComponent();
            myServer = new AsyncSocketServer();
        }

        private void Btn_Connetti_Click(object sender, RoutedEventArgs e)
        {
            Lbl_StatoServer.Content = "Server Online";
            Btn_Disconnetti.IsEnabled = true;
            //Btn_Connetti.IsEnabled = false;

            // Attivo e metto il server in ascolto
            myServer.ServerInAscolto();
        }

        private void Btn_Disconnetti_Click(object sender, RoutedEventArgs e)
        {
            Lbl_StatoServer.Content = "Server Offline";
            //Btn_Disconnetti.IsEnabled = false;
            //Btn_Connetti.IsEnabled = true;

            // Disconnetto il server
            myServer.DisconnettiServer();
        }
    }
}
