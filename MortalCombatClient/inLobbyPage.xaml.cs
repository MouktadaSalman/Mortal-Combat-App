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

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            string messageContent = messageBox.Text;
               
            await Task.Run(() =>
            {
                 duplexFoob.DistributeMessageToLobby(curLobby.LobbyName, curPlayer.Username, messageContent, 1);
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

        public void showMessage(string message)
        {
           
                MessagesListBox.Items.Add(message);
           
        }

        public void showLink(TextBlock inlink)
        {
            var item = new ListBoxItem();
            item.Content = inlink;

            MessagesListBox.Items.Add(item);
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
                    showLink((TextBlock)message.Content);
                }
               
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
                //Set a TextBlock' to allow text + hyper-link on same line
                TextBlock messageContent = new TextBlock();
                Run pSender = new Run(curPlayer.Username + ": ");

                string[] f = filePath.Split('\\');
                fileName = f.Last();

                //Hyper-link initialization
                Hyperlink hyper = new Hyperlink(new Run(fileName))
                {
                    NavigateUri = new Uri("https://MortalCombatDataServer/FileDatabase/" + fileName)
                };


                //Send hyper-link
                messageContent.Inlines.Add(pSender);
                messageContent.Inlines.Add(hyper);

                //store TextBlock type of message content to an 'object' placeholder
                object ob = messageContent;
                //showLink((TextBlock)ob);
                duplexFoob.DistributeMessageToLobby(curLobby.LobbyName, curPlayer.Username, ob, 2);

                

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
