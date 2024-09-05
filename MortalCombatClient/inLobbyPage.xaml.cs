using MortalCombatBusinessServer;
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
using Mortal_Combat_Data_Library;

namespace MortalCombatClient
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class inLobbyPage : Page
    {

        private BusinessInterface duplexFoob;
        private Player curPlayer; 
        private Lobby curLobby;
        public inLobbyPage(BusinessInterface inDuplexFoob, Player player, Lobby lobby)
        {
            InitializeComponent();

            duplexFoob = inDuplexFoob;
            curPlayer = player;
            curLobby = lobby;
            lobbyNameTextBox.Text = player.JoinedLobbyName;

            ((MainWindow)Application.Current.MainWindow).UpdateCallbackContext(this);

            Task task = loadLobbyMessagesAsync();
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            
            string messageContent = messageBox.Text;

            duplexFoob.DistributeMessageToLobby(curLobby.LobbyName, curPlayer.Username, messageContent);
            
            
            showMessage($"{curPlayer.Username}: {messageContent}");
            messageBox.Clear();
        }

        public void loadNewMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            MessagesListBox.Items.Clear();
            Task task = loadLobbyMessagesAsync();
        }

        public void showMessage(string message)
        {
            
            MessagesListBox.Items.Add(message);
        }

        public async Task loadLobbyMessagesAsync()
        {
            
            
            var lobbyMessages = await Task.Run(() => duplexFoob.GetDistributedMessages(curPlayer.Username,curLobby.LobbyName));
            foreach (var message in lobbyMessages)
            {

               showMessage(message.ToString());
            }
        }

        // Still unsure on how to handle file sharing...
        private void selectFilesButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void leaveLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            curPlayer.JoinedLobbyName = "Main";
            curLobby.PlayerCount--;
            NavigationService.GoBack();            
        }
    }
}
