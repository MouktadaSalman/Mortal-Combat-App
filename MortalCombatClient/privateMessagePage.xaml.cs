/* 
 * Module: lobbyPage
 * Description: This module is responsible for private messaging between 2 players(instances), 
 *              it handles the callbacks and the incoming messages and showing them in the chat box.
 * Author: Ahmed 
 * ID: 21467369
 * Version: 1.0.0.2
 */
using Mortal_Combat_Data_Library;
using MortalCombatBusinessServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;


namespace MortalCombatClient
{
    /// <summary>
    /// Interaction logic for privateMessagePage.xaml
    /// </summary>
    public partial class PrivateMessagePage : Page
    {
        private BusinessInterface duplexFoob;
        private Player curPlayer;
        public string MessageRecipient { get; private set; }


        /* Constructor: PrivateMessagePage
         * Description: The constructor of the private message page
         * Parameters: inDuplexFoob (BusinessInterface), player (Player), recipient (string)
         */
        public PrivateMessagePage(BusinessInterface inDuplexFoob, Player player, string recipient)
        {
            InitializeComponent();

            duplexFoob = inDuplexFoob;
            curPlayer = player;

            playerNameTextBox.Text = recipient;
            MessageRecipient = recipient;

            var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                mainWindow.CreateChannel();
            }

            mainWindow.UpdatePrivateCallbackContext(GetChatKey(player.Username, recipient), this);
            LoadChatHistory();
        }


        /* Method: SendButton_Click
         * Description: Sends a message to the recipient
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {

            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

            try
            {
                string messageContent = messageBox.Text;
                duplexFoob.SendPrivateMessage(curPlayer.Username, MessageRecipient, messageContent);
                AddMessageToListBox($"{curPlayer.Username}: {messageContent}");
                messageBox.Clear();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error sending message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /* Method: LoadNewMessagesButton_Click
         * Description: Loads new messages
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private void LoadNewMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

            try
            {
                MessagesListBox.Items.Clear();
                LoadChatHistory();

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading new messages: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /* Method: LeaveChatButton_Click
         * Description: Leaves the chat
         * Parameters: sender (object), e (RoutedEventArgs)
         */
        private void LeaveChatButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the connection is faulted
            if (((ICommunicationObject)duplexFoob).State == CommunicationState.Faulted)
            {
                var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                mainWindow.CreateChannel();
            }

            try
            {
                // Notify the MainWindow that we're closing this chat
                ((MainWindow)System.Windows.Application.Current.MainWindow).ClosePrivateMessagePage(GetChatKey(curPlayer.Username, MessageRecipient));

                // Navigate back to the previous page
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error leaving chat: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /* Method: LoadChatHistory
         * Description: Loads the chat history between the current player and the recipient
         */
        private void LoadChatHistory()
        {
            var messages = duplexFoob.GetPrivateMessages(curPlayer.Username, MessageRecipient);
            foreach (var message in messages)
            {
                AddMessageToListBox($"{message.Sender}: {message.Content}");
            }
        }

        /* Method: HandleIncomingMessage
         * Description: Handles incoming messages
         * Parameters: sender (string), content (string)
         */
        public void HandleIncomingMessage(string sender, string content)
        {
            Dispatcher.Invoke(() =>
            {
                AddMessageToListBox($"{sender}: {content}");
            });
        }

        /* Method: AddMessageToListBox
         * Description: Adds a message to the list box
         * Parameters: message (string)
         */
        public void AddMessageToListBox(string message)
        {
            MessagesListBox.Items.Add(message);
        }

        /* Method: GetChatKey
         * Description: Gets the chat key
         * Parameters: user1 (string), user2 (string)
         * Result: string
         */
        private string GetChatKey(string user1, string user2)
        {
            return string.Compare(user1, user2, StringComparison.Ordinal) < 0
                ? $"{user1}:{user2}"  //checks to open the same chat between both players
                : $"{user2}:{user1}"; //no matter who the sender is and who the recipent is
        }
    }
}