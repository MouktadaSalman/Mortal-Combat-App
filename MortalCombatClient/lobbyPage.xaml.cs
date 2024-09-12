/* 
 * Module: lobbyPage
 * Description: This module is responsible for the lobby functionality of the game. It allows players to create, join, and leave lobbies.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, , 21494299
 * Version: 1.0.0.2
 */
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
    public partial class LobbyPage : Page
    {
        private BusinessInterface duplexFoob;
        private InLobbyPage nextPage;
        private Player curPlayer;
        private List<Lobby> lobbiesInServer;

        public LobbyPage(BusinessInterface inFoob, Player player)
        {
            InitializeComponent();

            duplexFoob = inFoob;
            curPlayer = player;

            if (lobbiesInServer == null)
            {
                lobbiesInServer = new List<Lobby>();
            }

            ((MainWindow)System.Windows.Application.Current.MainWindow).UpdateLobbyCallbackContext(this);

            RefreshLists();
        }

        private void JoinLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            if (LobbyRoomList.SelectedItem != null)
            {

                string selectedLobbyName = LobbyRoomList.SelectedItem.ToString();
                duplexFoob.AddPlayertoLobby(curPlayer, selectedLobbyName);
                RefreshLists();

                Lobby lobby = GetLobbyUsingName(selectedLobbyName);

                if (lobby != null)
                {
                    NavigationService.Navigate(new InLobbyPage(duplexFoob, curPlayer, lobby));
                }
            } 
            else
            {
                MessageBox.Show("Choose one of the lobbies then click 'Join' \n Note: If there are no lobbies, you can create one");
            }
        }
        

        private void CreateLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string createdLobbyName = NewLobbyName.Text;
                duplexFoob.CheckLobbyNameValidity(createdLobbyName);

                Lobby lobby = CreateLobby(createdLobbyName);

                duplexFoob.AddPlayertoLobby(curPlayer, createdLobbyName);
                lobbiesInServer.Add(lobby);

                NavigationService.Navigate(new InLobbyPage(duplexFoob, curPlayer, lobby));
            
                RefreshLists();
            }catch (FaultException<LobbyNameAlreadyExistsFault> ex)
            {
                MessageBox.Show(ex.Detail.Issue);
            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            duplexFoob.RemovePlayerFromServer(curPlayer.Username);
            // Goes back to the loginPage
            NavigationService.GoBack();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshLists();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (LobbyRoomList.SelectedItem == null)
            {
                MessageBox.Show("Please select a lobby to delete");
                return;
            }

            string selectedLobbyName = LobbyRoomList.SelectedItem.ToString();
            try
            {
                duplexFoob.DeleteLobby(selectedLobbyName);

                Lobby lobby = GetLobbyUsingName(selectedLobbyName);
                lobbiesInServer.Remove(lobby);
            }
            catch(FaultException<PlayersStilInLobbyFault> ex)
            {
                MessageBox.Show(ex.Detail.Issue);
            }

            RefreshLists();
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

        public Lobby CreateLobby(string lobbyName)
        {
            Lobby newLobby = new Lobby(lobbyName);
            duplexFoob.AddLobbyToServer(lobbyName);
            lobbiesInServer.Add(newLobby);
            return newLobby;
        }

        public void RefreshLists()
        {            
            LobbyRoomList.Items.Clear();
            lobbiesInServer.Clear();
            foreach (string lobbyName in duplexFoob.GetAllLobbyNames())
            {
                LobbyRoomList.Items.Add(lobbyName.ToString());

                Lobby lobby = new Lobby(lobbyName);
                lobbiesInServer.Add(lobby);
            }            
        }
    }
}
