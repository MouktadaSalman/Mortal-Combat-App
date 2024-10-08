﻿/* 
 * Module: loginPage
 * Description: This module is responsible for the login functionality of the game. It allows players to login to the game.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, , 21494299
 * Version: 1.0.0.2
 */
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ServiceModel;
using MortalCombatBusinessServer;
using Mortal_Combat_Data_Library;
using System;


namespace MortalCombatClient
{
    //public delegate Player CreatePlayer(string pUserName);
    /// <summary>
    /// Interaction logic for loginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        /* Class Fields:
         * duplexFoob -> the business interface
         */
        private BusinessInterface duplexFoob;

        /* Constructor: loginPage
         * Description: The constructor of the login page
         * Parameters: inDuplexFoob (BusinessInterface)
         */
        public LoginPage(BusinessInterface inDuplexFoob)
        {
            InitializeComponent();

            duplexFoob = inDuplexFoob;
            
        }

        /* Method: Button_Click
         * Description: When the button is clicked, the username is checked for validity and a player is created 
         * Parameters: snender (object), e (RoutedEventArgs)
         */
        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as Button; // Cast sender to Button
            if (button != null)
            {
                button.IsEnabled = false; // Disable the button
            }

            try
            {     
                string username = UsernameBox.Text.ToString();

                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("Username cannot be empty or contain only spaces. Please enter a valid name.");
                }
                else
                {
                    Player player = await Task.Run(() => CreatePlayer(username));
                
                    NavigationService.Navigate(new LobbyPage(duplexFoob, player));         
                }
            }
            catch (FaultException<PlayerNameAlreadyEsistsFault> ex)
            {
                MessageBox.Show(ex.Detail.Issue);
            }
            finally
            {
                if (button != null)
                {
                    button.IsEnabled = true; // Re-enable the button after operation
                }
            }
        }

        /* Method: CreatePlayer
         * Description: Creates a player
         * Parameters: pUserName (string)
         * Result: Player
         */
        private Player CreatePlayer(string pUserName)
        {
            duplexFoob.CheckUsernameValidity(pUserName);
            Player newPlayer = new Player(pUserName);
            duplexFoob.AddPlayerToServer(newPlayer);
            return newPlayer;
        }
    }
}