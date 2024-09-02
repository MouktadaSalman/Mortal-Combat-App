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
using System.ServiceModel;
using MortalCombatBusinessServer;

namespace MortalCombatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private BusinessInterface foob;

        public MainWindow()
        {
            InitializeComponent();


            ChannelFactory<BusinessInterface> channelFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            tcp.SendTimeout = TimeSpan.FromMinutes(5);
            tcp.ReceiveTimeout = TimeSpan.FromMinutes(5);
            tcp.OpenTimeout = TimeSpan.FromMinutes(1);
            tcp.CloseTimeout = TimeSpan.FromMinutes(1);

            string URL = "net.tcp://localhost:8200/MortalCombatBusinessService";
            channelFactory = new ChannelFactory<BusinessInterface>(tcp, URL);
            foob = channelFactory.CreateChannel();

            MainFrame.NavigationService.Navigate(new loginPage(foob));
        }
    }
}
