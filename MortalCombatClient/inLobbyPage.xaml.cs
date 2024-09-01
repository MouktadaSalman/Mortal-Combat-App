using MortalCombatBusinessServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

namespace MortalCombatClient
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class inLobbyPage : Page
    {

        private BusinessInterface foob;
        public inLobbyPage(string lobbyName)
        {
            InitializeComponent();


            lobbyNameTextBox.Text = lobbyName;
        }


        private void sendButton_Click(object sender, RoutedEventArgs e)
        {

        }


        // Still unsure on how to handle file sharing...
        private void selectFilesButton_Click(Object sender, RoutedEventArgs e)
        {

        }

        private void leaveLobbyButton_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.GoBack();
            
        }
    }
}
