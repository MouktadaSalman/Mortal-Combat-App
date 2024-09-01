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
    /// Interaction logic for loginPage.xaml
    /// </summary>
    public partial class loginPage : Page
    {
        private BusinessInterface foob;

        public loginPage()
        {
            InitializeComponent();

            ChannelFactory<BusinessInterface> channelFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost:8200/MortalCombatBusinessService";
            channelFactory = new ChannelFactory<BusinessInterface>();
            foob = channelFactory.CreateChannel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foob.AddPlayerToServer(UsernameBox.Text.ToString());
            NavigationService.Navigate(new lobbyPage());
        }
    }
}
