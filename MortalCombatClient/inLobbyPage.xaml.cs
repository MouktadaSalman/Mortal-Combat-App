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
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;

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
              
            ((MainWindow)System.Windows.Application.Current.MainWindow).UpdateLobbyCallbackContext(this);

            
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

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            
            string messageContent = messageBox.Text;

            duplexFoob.DistributeMessageToLobby(curLobby.LobbyName, curPlayer.Username, messageContent);
            
            
            showMessage($"{curPlayer.Username}: {messageContent}");
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
            
            MessagesListBox.Items.Add(message);
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

        private void selectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            //To extract file path + filename
            string filePath = string.Empty;
            string fileName = string.Empty;

            //Setting up file filters
            string filter = "All Files (*.*)|*.*|" +
                            "Text Files (*.txt)|*.txt|" +
                            "Image Files|";

            var imageCodes = ImageCodecInfo.GetImageEncoders();

            foreach(var code in imageCodes)
            {
                filter += code.FilenameExtension + ";";
            }

            using (var opf = new System.Windows.Forms.OpenFileDialog())
            {
                opf.Filter = filter;
                opf.FilterIndex = 2;
                opf.RestoreDirectory = true;

                if (opf.ShowDialog() == DialogResult.OK)
                {
                    filePath = opf.FileName;
                }
            }

            if (filePath != null)
            {
                string[] f = filePath.Split('\\');
                fileName = f.Last();

                //Hyper-link initialization
                Run fileRun = new Run(fileName);
                Hyperlink hyper = new Hyperlink(fileRun);

                //Send hyper-link
                var messageContent = hyper;

                duplexFoob.DistributeMessageToLobby(curLobby.LobbyName, curPlayer.Username, messageContent);

                showMessage($"{curPlayer.Username}: {messageContent}");

                System.Windows.MessageBox.Show("File path is: " + filePath);
            }
            else
            {
                System.Windows.MessageBox.Show("Failed to extract file path");
            }
        }

        //Handle hyper-link selection
        private void HandleRequestNavigate(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink)sender;
            var uri = link.NavigateUri.ToString();
            Process.Start(uri);
            e.Handled = true;
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
