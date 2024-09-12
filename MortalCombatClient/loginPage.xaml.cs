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
        private BusinessInterface duplexFoob;

        public loginPage(BusinessInterface inDuplexFoob)
        {
            InitializeComponent();
            this.duplexFoob = inDuplexFoob;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.ToString();

            

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

        private Player CreatePlayer(string pUserName)
        {
            Player newPlayer = new Player(pUserName, "Main");
            duplexFoob.AddPlayerToServer(newPlayer);
            return newPlayer;
        }

    }
}