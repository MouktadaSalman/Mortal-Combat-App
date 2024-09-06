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
        private List<Player> playersInLobby;
        public inLobbyPage(BusinessInterface inDuplexFoob, Player player, Lobby lobby)
        {
            InitializeComponent();

            duplexFoob = inDuplexFoob;
            curPlayer = player;
            curLobby = lobby;
            lobbyNameTextBox.Text = player.JoinedLobbyName;

         
            playersInLobby = new List<Player>();
              
            ((MainWindow)Application.Current.MainWindow).UpdateLobbyCallbackContext(this);

            
            Task task = loadLobbyMessagesAsync();
            
        }


        public void RefreshLists()
        {
            playersInLobby.Clear();
            onlinePlayers.Items.Clear();
            foreach (string playerName in duplexFoob.GetPlayersInLobby(curLobby))
            {


                if (!onlinePlayers.Items.Contains(playerName))
                {
                    onlinePlayers.Items.Add(playerName);
                }
                

                Player player = new Player(playerName, curLobby.LobbyName);
                playersInLobby.Add(player);

            }

        }

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            string messageContent = messageBox.Text;

               
                await Task.Run(() =>
                {
                    duplexFoob.DistributeMessageToLobby(curLobby.LobbyName, curPlayer.Username, messageContent);
                });

            
            MessagesListBox.Items.Clear();
            Task task = loadLobbyMessagesAsync();

            messageBox.Clear();
        }

        

        private void sendMessageButton_Click (object sender, RoutedEventArgs e)
        {
           string recipent = onlinePlayers.SelectedItem.ToString();
           privateMessagePage nextPage = new privateMessagePage(duplexFoob, curPlayer, recipent);
           NavigationService.Navigate(nextPage);
        }

        public void loadNewMessagesButton_Click(object sender, RoutedEventArgs e)
        {

            MessagesListBox.Items.Clear();
            Task task = loadLobbyMessagesAsync();
        }

        public void showMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                MessagesListBox.Items.Add(message);
            });
        }

        public void showLink(MessageDatabase.FileLinkBlock message)
        {
            TextBlock block = new TextBlock();
            block.Inlines.Add(new Run(message.Sender + ": "));

            //Setup hyperlink
            //Hyperlink link = new Hyperlink(new Run(message.FileName));
            Hyperlink link = new Hyperlink(new Run(message.FileName))
            {
                NavigateUri = new Uri(message.Uri)
            };

            //Combine both componenets
            block.Inlines.Add(link);

            //Direct a method when link is clicked
            link.RequestNavigate += HandleRequestNavigate;

            Dispatcher.Invoke(() =>
            {
                MessagesListBox.Items.Add(block);
            });
        }

        public async Task loadLobbyMessagesAsync()
        {

            RefreshLists();
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

            onlinePlayers.Items.Remove(curPlayer);
            playersInLobby.Remove(curPlayer);
            NavigationService.GoBack();            
        }



    }
}
