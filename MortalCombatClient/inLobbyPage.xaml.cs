/* 
 * Module: inLobbyPage
 * Description: This module is responsible for the in-lobby functionality of the game. It allows players to chat, send messages, and leave the lobby.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, , 21494299
 * Version: 1.0.0.2
 */
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
        /* Class fields:
         * duplexFoob -> the business interface
         * curPlayer -> the player currently using the client
         * curLobby -> the lobby the player is currently in
         * playersInLobby -> the list of players in the lobby
         */
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

            onlinePlayersCount.Text = curLobby.PlayerCount.ToString();

            playersInLobby = new List<Player>();
              
            ((MainWindow)System.Windows.Application.Current.MainWindow).UpdateLobbyCallbackContext(this);
            
            Task task = loadLobbyMessagesAsync();            
        }


        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            string messageContent = messageBox.Text;

            await Task.Run(() =>
            {
                duplexFoob.DistributeMessageToLobby(curLobby.LobbyName, curPlayer.Username, messageContent);
            });
            
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
                if(message.MessageType == 1)
                {
                    showMessage(message.ToString());
                }
                else if(message.MessageType == 2)
                {
                    showLink(message.ContentF);
                }
                else
                {
                    System.Windows.MessageBox.Show("Encountered an unkown message type");
                }
            }
        }

        private async void selectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            //To extract file path + filename
            string filePath = string.Empty;
            string fileName = string.Empty;

            //Setting up file filters
            string filter = "Text Files (*.txt)|*.txt|" +
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

                //Assign hyper-link info
                var messageContent = new MessageDatabase.FileLinkBlock();
                messageContent.Sender = curPlayer.Username;
                messageContent.FileName = fileName;
                messageContent.Uri = "http://MortalCombatDataServer/FileDatabase/" + fileName;

                //Send hyperlink info through
                await Task.Run(() =>
                {
                    //Upload file
                    duplexFoob.UploadFile(filePath);

                    //Create info of message
                    duplexFoob.DistributeMessageToLobbyF(curLobby.LobbyName, curPlayer.Username, messageContent);
                });
            }
            else
            {
                System.Windows.MessageBox.Show("Failed to extract file path");
            }
        }

        //Handle hyper-link selection
        private void HandleRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //Extracting the filename from the URI
            string[] uri = e.Uri.OriginalString.Split('/');

            string fileName = uri.Last();

            //Download the file
            duplexFoob.DownloadFile(fileName);

            //Process handled
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
