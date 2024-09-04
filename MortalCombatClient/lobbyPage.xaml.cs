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
    public partial class lobbyPage : Page
    {
        private BusinessInterface foob;
        private string curLobbyName;
        private inLobbyPage nextPage;
        private Player curPlayer;
        private List<Lobby> lobbiesInServer;

        public lobbyPage(BusinessInterface inFoob, Player player)
        {
            InitializeComponent();

            foob = inFoob;
            curPlayer = player;

            if (lobbiesInServer == null)
            {
                lobbiesInServer = new List<Lobby>();
            }

            RefreshLobbyList();
        }

        private void JoinLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            if (LobbyRoomList.SelectedItem != null)
            {
                string selectedLobbyName = LobbyRoomList.SelectedItem.ToString();
                curPlayer.JoinedLobbyName = selectedLobbyName;


                Lobby lobby = GetLobbyUsingName(selectedLobbyName);

                if (lobby != null)
                {
                    nextPage = new inLobbyPage(foob, curPlayer, lobby);
                    lobby.PlayerCount++;
                    NavigationService.Navigate(nextPage);
                }

            } else
            {
                MessageBox.Show("Choose one of the lobbies then click 'Join' \n Note: If there are no lobbies, you can create one");
            }
        }
        

        private void CreateLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            string createdLobbyName = NewLobbyName.Text;

            if (LobbyNameIsValid(createdLobbyName))
            {
                Lobby lobby = CreateLobby(createdLobbyName);
                curPlayer.JoinedLobbyName = createdLobbyName;                
                lobby.PlayerCount++;
                nextPage = new inLobbyPage(foob, curPlayer, lobby);

                LobbyRoomList.Items.Add(curLobbyName);
                NavigationService.Navigate(nextPage);
            }
            else
            {
                return;
            }

            RefreshLobbyList();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            //foob.RemovePlayerFromServer(username);
            // Goes back to the loginPage
            NavigationService.GoBack();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshLobbyList();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedLobbyName = LobbyRoomList.SelectedItem.ToString();
            bool lobbyHasPlayers;
            foob.DeleteLobby(selectedLobbyName, out lobbyHasPlayers);
            if (lobbyHasPlayers)
            {
                MessageBox.Show("Lobby has players, cannot delete");
                return;
            }

            if (!curPlayer.JoinedLobbyName.Equals("Main"))
            {
                curPlayer.JoinedLobbyName = "Main";
            }

            RefreshLobbyList();
        }

        public Lobby GetLobbyUsingName(string lobbyName)
        {
            foreach (Lobby lobby in lobbiesInServer)
            {
                if (lobby.LobbyName.Equals(lobbyName))
                {
                    return lobby;
                }
            }
            return null;
        }

        public bool LobbyNameIsValid(string inLobbyName)
        {
            if (NewLobbyName.Text == "")
            {
                MessageBox.Show("Please enter a lobby name");
            }
            bool isValid;

            foob.CheckLobbyNameValidity(inLobbyName, out isValid);

            if (!isValid)
            {
                MessageBox.Show("Lobby name already Taken");
                return false;
            }
            return true;
        }

        public Lobby CreateLobby(string lobbyName)
        {
            Lobby newLobby = new Lobby(lobbyName);
            foob.AddLobbyToServer(newLobby);
            lobbiesInServer.Add(newLobby);
            return newLobby;
        }
        
        public void RefreshLobbyList()
        {
            LobbyRoomList.Items.Clear();
            foreach (string lobbyName in foob.GetAllLobbyNames())
            {
                LobbyRoomList.Items.Add(lobbyName.ToString());
            }

            //foreach (Lobby l in lobbiesInServer)
            //{
            //    if (!LobbyRoomList.Items.Contains(l.LobbyName))
            //    {
            //        LobbyRoomList.Items.Add(l.LobbyName);
            //    }
            //}
        }
    }
}
