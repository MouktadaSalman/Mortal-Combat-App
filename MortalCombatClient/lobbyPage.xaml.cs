/* 
 * Module: lobbyPage
 * Description: This module is responsible for the lobby functionality of the game. It allows players to create, join, and leave lobbies.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, 21494299
 * Version: 1.0.0.2
 */
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MortalCombatBusinessServer;
using Mortal_Combat_Data_Library;

namespace MortalCombatClient
{
    public partial class LobbyPage : Page
    {
        /* Class fields:
         * duplexFoob -> the connection instance to the business server
         * curplayer -> the player of this client instance
         */
        private BusinessInterface duplexFoob;
        private Player curPlayer;

        /* Constructor: LobbyPage
         * Description: The constructor of the lobby page
         * Parameters: inFoob (BusinessInterface), player (Player)
         */
        public LobbyPage(BusinessInterface inFoob, Player player)
        {
            InitializeComponent();

            duplexFoob = inFoob;
            curPlayer = player;

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                mainWindow.CreateChannel();
            }

            mainWindow.UpdateLobbyCallbackContext(this);
            RefreshLists();
        }

        /* Method: CreateLobbyButton_Click
         * Description: The click listener when a user presses the button to create a lobby (async)
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private async void CreateLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

            try
            {
                string createdLobbyName = NewLobbyName.Text;
                duplexFoob.CheckLobbyNameValidity(createdLobbyName);

                Lobby lobby = await Task.Run(() => CreateLobby(createdLobbyName));

                duplexFoob.AddPlayertoLobby(curPlayer, createdLobbyName);

                RefreshLists();
            
                NavigationService.Navigate(new InLobbyPage(duplexFoob, curPlayer, lobby));
            }
            catch (FaultException<LobbyNameAlreadyExistsFault> ex)
            {
                MessageBox.Show(ex.Detail.Issue);
            }
        }

        /* Method: JoinLobbyButton_Click
         * Description: The click listener when a user presses the button want to 
         *              join a lobby
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private void JoinLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

            try
            {
                if (LobbyRoomList.SelectedItem != null)
                {
                    //Get the lobby name
                    string selectedLobbyName = LobbyRoomList.SelectedItem.ToString();
                    duplexFoob.AddPlayertoLobby(curPlayer, selectedLobbyName);
                    RefreshLists();

                    Lobby lobby = duplexFoob.GetLobbyByName(selectedLobbyName);

                    NavigationService.Navigate(new InLobbyPage(duplexFoob, curPlayer, lobby));                
                } 
                else
                {
                    MessageBox.Show("Choose one of the lobbies then click 'Join' \n Note: If there are no lobbies, you can create one");
                }

            }
            catch(Exception)
            {
                MessageBox.Show("An issue occured\n Try refreshing list before joining lobby");
            }
        }


        /* Method: LogOutButton_Click
         * Description: The click listener when a user presses the button to log out
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private async void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

            try
            {
                await Task.Run(() => duplexFoob.RemovePlayerFromServer(curPlayer.Username));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            // Goes back to the loginPage
            NavigationService.GoBack();
        }

        /* Method: RefreshButton_Click
         * Description: The click listener when a user presses the button to refresh the lobby list
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

            RefreshLists();
        }

        /* Method: DeleteButton_Click
         * Description: The click listener when a user presses the button to delete a lobby (async)
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

            if (LobbyRoomList.SelectedItem == null)
            {
                MessageBox.Show("Please select a lobby to delete");
                return;
            }

            string selectedLobbyName = LobbyRoomList.SelectedItem.ToString();
            try
            {
                await Task.Run(() => duplexFoob.DeleteLobby(selectedLobbyName));                
            }
            catch(FaultException<PlayersStilInLobbyFault> ex)
            {
                MessageBox.Show(ex.Detail.Issue);
            }

            RefreshLists();
        }

        /* Method: CreateLobby
         * Description: Creates a lobby
         * Parameters: lobbyName (string)
         * Result: Lobby
         */
        public Lobby CreateLobby(string lobbyName)
        {
            Lobby newLobby = new Lobby(lobbyName);
            duplexFoob.AddLobbyToServer(lobbyName);
            return newLobby;
        }

        /* Method: RefreshLists
         * Description: Refreshes the lobby list
         */
        public void RefreshLists()
        {            
            LobbyRoomList.Items.Clear();
            foreach (string lobbyName in duplexFoob.GetAllLobbyNames())
            {
                LobbyRoomList.Items.Add(lobbyName.ToString());
            }            
        }
    }
}
