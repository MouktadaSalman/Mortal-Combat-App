﻿/* 
 * Module: lobbyPage
 * Description: This module is responsible for the lobby functionality of the game. It allows players to create, join, and leave lobbies.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, 21494299
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
using System.Windows.Media.Animation;

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

            ((MainWindow)System.Windows.Application.Current.MainWindow).UpdateLobbyCallbackContext(this);
            RefreshLists();
        }





        
        /* Method: JoinLobbyButton_Click
         * Description: The click listener when a user presses the button want to 
         *              join a lobby
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private void JoinLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            //Null check if the user clicks button without selection

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

        /* Method: CreateLobbyButton_Click
         * Description: The click listener when a user presses the button to create a lobby (async)
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private async void CreateLobbyButton_Click(object sender, RoutedEventArgs e)
        {
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

        /* Method: LogOutButton_Click
         * Description: The click listener when a user presses the button to log out
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            duplexFoob.RemovePlayerFromServer(curPlayer.Username);
            // Goes back to the loginPage
            NavigationService.GoBack();
        }

        /* Method: RefreshButton_Click
         * Description: The click listener when a user presses the button to refresh the lobby list
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshLists();
        }

        /* Method: DeleteButton_Click
         * Description: The click listener when a user presses the button to delete a lobby (async)
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
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
