
/* 
 * Module: inLobbyPage
 * Description: This module is responsible for the in-lobby functionality of the game. It allows players to chat, send messages, and leave the lobby.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, 21494299
 * Version: 1.0.0.2
 */

using MortalCombatBusinessServer;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using Mortal_Combat_Data_Library;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.ServiceModel;

namespace MortalCombatClient
{
    public partial class InLobbyPage : Page
    {
        /* Class fields:
         * duplexFoob -> the business interface
         * curPlayer -> the player currently using the client
         * curLobby -> the lobby the player is currently in
         */
        private BusinessInterface duplexFoob;
        private Player curPlayer;
        private Lobby curLobby;

        public InLobbyPage(BusinessInterface inDuplexFoob, Player player, Lobby lobby)
        {
            InitializeComponent();

            duplexFoob = inDuplexFoob;
            curPlayer = player;
            curLobby = lobby;
            lobbyNameTextBox.Text = lobby.LobbyName;

            var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                mainWindow.CreateChannel();
            }

            mainWindow.UpdateInLobbyCallbackContext(this);

            Task task = LoadLobbyMessagesAsync();
        }

        /* Method: SendButton_Click
         * Description: The method to handle the event in which when the send button is 
         *              clicked (async). It creates a pull request to a new thread to 
         *              send the message to the lobby chat
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

            //Get the message input
            string messageContent = messageBox.Text;

            //Null check to see if message content is empty
            if(!string.IsNullOrEmpty(messageContent))
            {
                //If its not empty start a new task/create a pull request to a new thread
                await Task.Run(() =>
                {
                    duplexFoob.DistributeMessageToLobby(curLobby.LobbyName, curPlayer.Username, messageContent);
                });
            }

            messageBox.Clear();
        }

        /* Method: SelectFilesButton_Click
         * Description: The method to handle the event in which when the user wants to share
         *              a file to the lobby chat (async). It creates a pull request to a new
         *              thread to send the file link to the lobby chat
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private async void SelectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

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

            //Open the file explorer
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

                //Send hyperlink info through via a new thread/pull request
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

        /* Method: OpenChatButton_Click
         * Description: The method to handle the event in which when a user wants to
         *              initiate a private chat with a fellow lobby member
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private void OpenChatButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

            if (onlinePlayers.SelectedItem != null) { 
            string recipent = onlinePlayers.SelectedItem.ToString();
            if (!recipent.Equals(curPlayer.Username))
            {
                PrivateMessagePage nextPage = new PrivateMessagePage(duplexFoob, curPlayer, recipent);
                NavigationService.Navigate(nextPage);
            }
            else
            {
                System.Windows.MessageBox.Show("You can not send a message to yourself");
            }
        } else
            {
                System.Windows.MessageBox.Show("You must choose a player before clicking 'Open Chat'");
            }
        }

        /* Method: RefreshButton_Click
         * Description: The method to handle the event in which when a user wants to
         *              refresh the lobby (both chat & player list)
         * Parameters: sender (object), e (RoutedEventArgs)
         *            
         */
        public void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

            //Clear the chat
            MessagesListBox.Items.Clear();

            //Initiate a new task to refresh the GUI (separate thread)
            Task task = LoadLobbyMessagesAsync();
        }

        /* Method: LeaveLobbyButton_click
         * Description: To handle the event when player leaves
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private void LeaveLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            //error occurs here from client side
            duplexFoob.RemovePlayerFromLobby(curPlayer.Username, curLobby.LobbyName);
            onlinePlayers.Items.Remove(curPlayer.Username);
            RefreshLists();
            NavigationService.GoBack();
        }

        /* Method: ShowMessage
         * Description: To show the string messages of users into lobby chat
         * Parameters: message (string)
         */
        public void ShowMessage(string message)
        {
            //Update GUI
            Dispatcher.Invoke(() =>
            {
                MessagesListBox.Items.Add(message);
            });
        }

        /* Method: ShowLink
         * Description: To show the link of the file being shared by users 
         *              into lobby chat
         * Parameters: message (FileLinkBlock)
         */
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

            //Direct a method when link is clicked (event handler)
            link.RequestNavigate += HandleRequestNavigate;

            //Update GUI
            Dispatcher.Invoke(() =>
            {
                MessagesListBox.Items.Add(block);
            });
        }

        /* Method: RefreshLists
         * Description: To refresh the player list
         */
        public void RefreshLists()
        {
            //Clear player list
            onlinePlayers.Items.Clear();
            Lobby lobby = duplexFoob.GetLobbyByName(curLobby.LobbyName);

            //Get all the players in current lobby
            foreach (Player player in lobby._playerInLobby)
            {
                if (!onlinePlayers.Items.Contains(player.Username))
                {
                    onlinePlayers.Items.Add(player.Username);
                }
            }
        }

        /* Method: LoadLobbyMessagesAsync
         * Description: The method that runs the task (async) of refreshing the GUI
         *              on a separate thread
         */
        public async Task LoadLobbyMessagesAsync()
        {
            //Call method to refresh the player list
            RefreshLists();

            //Get all the lobby messages via a pull request
            var lobbyMessages = await Task.Run(() => duplexFoob.GetDistributedMessages(curPlayer.Username,curLobby.LobbyName));

            //Iterate through every message
            foreach (var message in lobbyMessages)
            {
                //If it is a text message
                if(message.MessageType == 1)
                {
                    //Show the message
                    ShowMessage(message.ToString()); 
                }
                //If it is a hyperlink to a file
                else if(message.MessageType == 2)
                {
                    //Show message
                    ShowLink(message.ContentF);
                }
                //If somehow a different message type went through
                else
                {
                    System.Windows.MessageBox.Show("Encountered an unkown message type");
                }
            }

        }

        /* Method: HandleRequestNavigate
         * Description: To handle the event in which when a file link is clicked
         */
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
    }
}
