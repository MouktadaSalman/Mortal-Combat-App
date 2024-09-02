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
using MortalCombatBusinessServer;
using MortalCombatDataServer;
using Mortal_Combat_Data_Library;
using System.ServiceModel.Channels;



namespace MortalCombatClient
{
    /// <summary>
    /// Interaction logic for lobbyPage.xaml
    /// </summary>
    public partial class lobbyPage : Page { 

        private BusinessInterface foob;
        private string usernameOfCurrentPlayer;
        inLobbyPage nextPage;

        public lobbyPage(BusinessInterface inFoob, string username)
        {
            InitializeComponent();

            

            this.foob = inFoob;
            this.usernameOfCurrentPlayer = username;


            foreach(Lobby lobby  in foob.GetAllLobbies())
            {
                LobbyRoomList.Items.Add(lobby.LobbyName);
            }
        }

        private void JoinLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            string lobbyToJoin = LobbyRoomList.SelectedItem.ToString();
            string username = usernameOfCurrentPlayer;
            
            foob.AddPlayerToLobby(lobbyToJoin, username);
            

            nextPage = new inLobbyPage(lobbyToJoin);
            NavigationService.Navigate(nextPage);
        }

        private void CreateLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            foob.CreateLobby(NewLobbyName.Text);

            string lobbyName = NewLobbyName.Text;

            nextPage = new inLobbyPage(lobbyName);
            LobbyRoomList.Items.Add(lobbyName);

            NavigationService.Navigate(nextPage);
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            // Goes back to the loginPage
            NavigationService.GoBack();
        }
       
    }
}
