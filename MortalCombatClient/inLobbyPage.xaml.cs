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
using Mortal_Combat_Data_Library;

namespace MortalCombatClient
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class inLobbyPage : Page
    {

        private BusinessInterface foob;
        private Player curPlayer; 
        private Lobby curLobby;
        public inLobbyPage(BusinessInterface inFoob, Player player, Lobby lobby)
        {
            InitializeComponent();

            foob = inFoob;
            curPlayer = player;
            curLobby = lobby;
            lobbyNameTextBox.Text = player.JoinedLobbyName;
        }


        private void sendButton_Click(object sender, RoutedEventArgs e)
        {

        }


        // Still unsure on how to handle file sharing...
        private void selectFilesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void leaveLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            curPlayer.JoinedLobbyName = "Main";
            curLobby.PlayerCount--;
            NavigationService.GoBack();            
        }
    }
}
