/* 
 * Module: loginPage
 * Description: This module is responsible for the login functionality of the game. It allows players to login to the game.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, , 21494299
 * Version: 1.0.0.2
 */
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
using Mortal_Combat_Data_Library;
using System.ServiceModel.Configuration;

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
            this.duplexFoob = inDuplexFoob;
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

            string username = UsernameBox.Text.ToString();

            try
            {
                Player player = await Task.Run(() => CreatePlayer(username));
                
                NavigationService.Navigate(new LobbyPage(duplexFoob, player));         
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