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
    /// Interaction logic for inLobbyPage.xaml
    /// </summary>
    public partial class InLobbyPage : Page
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

        public InLobbyPage(BusinessInterface inDuplexFoob, Player player, Lobby lobby)
        {
            InitializeComponent();

            duplexFoob = inDuplexFoob;
            curPlayer = player;
            curLobby = lobby;
            lobbyNameTextBox.Text = lobby.LobbyName;

            playersInLobby = new List<Player>();

            ((MainWindow)System.Windows.Application.Current.MainWindow).UpdateInLobbyCallbackContext(this);

            Task task = LoadLobbyMessagesAsync();
        }


        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string messageContent = messageBox.Text;

            await Task.Run(() =>
            {
                duplexFoob.DistributeMessageToLobby(curLobby.LobbyName, curPlayer.Username, messageContent);
            });

            messageBox.Clear();
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            string recipent = onlinePlayers.SelectedItem.ToString();
            if (!recipent.Equals(curPlayer.Username))
            {
                PrivateMessagePage nextPage = new PrivateMessagePage(duplexFoob, curPlayer, recipent);
                NavigationService.Navigate(nextPage);
            } else
            {
                System.Windows.MessageBox.Show("You can not send a message to yourself");
            }
        }

        public void LoadNewMessagesButton_Click(object sender, RoutedEventArgs e)
        {

            MessagesListBox.Items.Clear();
            Task task = LoadLobbyMessagesAsync();
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

                Player player = new Player(playerName);
                playersInLobby.Add(player);
            }
        }

        public void ShowMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                MessagesListBox.Items.Add(message);
            });
        }

        public void ShowLink(MessageDatabase.FileLinkBlock message)
        {
            //Create the placeholder to hold hyperlink + sender
            TextBlock block = new TextBlock();
            block.Inlines.Add(new Run(message.Sender + ": "));

            //Setup hyperlink
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

        public async Task LoadLobbyMessagesAsync()
        {
            var lobbyMessages = await Task.Run(() => duplexFoob.GetDistributedMessages(curPlayer.Username,curLobby.LobbyName));
            foreach (var message in lobbyMessages)
            {
                if(message.MessageType == 1)
                {
                    ShowMessage(message.ToString());
                }
                else if(message.MessageType == 2)
                {
                    ShowLink(message.ContentF);
                }
                else
                {
                    System.Windows.MessageBox.Show("Encountered an unkown message type");
                }
            }
           RefreshLists();
        }

        private async void SelectFilesButton_Click(object sender, RoutedEventArgs e)
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
                else //If user doesn't select any file (press Cancel or 'x' button)
                {
                    filePath = null;
                }
            }

            //Check if none is selected
            if (!string.IsNullOrEmpty(filePath))
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
                System.Windows.MessageBox.Show("No file selected");
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

        private void LeaveLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            duplexFoob.RemovePlayerFromLobby(curPlayer.Username, curLobby.LobbyName);
            NavigationService.GoBack();
        }
    }
}
