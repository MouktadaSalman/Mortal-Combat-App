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


namespace MortalCombatClient
{
    /// <summary>
    /// Interaction logic for lobbyPage.xaml
    /// </summary>
    public partial class lobbyPage : Page { 

        private BusinessInterface foob;
    
        public lobbyPage()
        {
            InitializeComponent();

            

        }


        
        private void JoinLobbyButton_Click(Object sender, RoutedEventArgs e)
        {

        }



        private void CreateLobbyButton_Click(Object sender, RoutedEventArgs e)
        {
            //foob.CreateLobby(NewLobbyName.Text);

            string lobbyName = NewLobbyName.Text;

            

            inLobbyPage nextPage = new inLobbyPage(lobbyName);

            

            NavigationService.Navigate(nextPage);
        }


        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {

            // Goes back to the loginPage
            NavigationService.GoBack();
        }
       
    }
}
