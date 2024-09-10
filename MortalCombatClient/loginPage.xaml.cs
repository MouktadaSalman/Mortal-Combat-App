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
    public partial class loginPage : Page
    {
        /* Class Fields:
         * duplexFoob -> the business interface
         */
        private BusinessInterface duplexFoob;

        /* Constructor: loginPage
         * Description: The constructor of the login page
         * Parameters: inDuplexFoob (BusinessInterface)
         */
        public loginPage(BusinessInterface inDuplexFoob)
        {
            InitializeComponent();
            this.duplexFoob = inDuplexFoob;
        }

        /* Method: Button_Click
         * Description: When the button is clicked, the username is checked for validity and a player is created 
         * Parameters: snender (object), e (RoutedEventArgs)
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.ToString();

            //Task<Player> task = new Task<Player>(() => CreatePlayer(username));
            //task.Start();

            //Player curPlayer = await task;

            if (usernameIsValid(username))
            {
                Player player = CreatePlayer(username);
                
                NavigationService.Navigate(new lobbyPage(duplexFoob, player));         
            }
            else
            {
                return;
            }
        }

        /* Method: usernameIsValid
         * Description: Checks if the username is valid
         * Parameters: pUserName (string)
         * Result: bool
         */
        public bool usernameIsValid(string pUserName)
        {
            if (UsernameBox.Text == "")
            {
                MessageBox.Show("Please enter a username");
            }
            bool isValid;

            duplexFoob.CheckUsernameValidity(pUserName, out isValid);

            if (!isValid)
            {
                MessageBox.Show("Username already Taken");
                return false;
            }
            return true;
        }

        /* Method: CreatePlayer
         * Description: Creates a player
         * Parameters: pUserName (string)
         * Result: Player
         */
        private Player CreatePlayer(string pUserName)
        {
            Player newPlayer = new Player(pUserName, "Main");
            duplexFoob.AddPlayerToServer(newPlayer);
            return newPlayer;
        }

    }
}