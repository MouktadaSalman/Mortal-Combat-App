using Mortal_Combat_Data_Library;
using MortalCombatBusinessServer;
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

namespace MortalCombatClient
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class PrivateMessagePage : Page
    {
        private BusinessInterface duplexFoob;
        private Player curPlayer;
        public string MessageRecipient { get; private set; }

        public PrivateMessagePage(BusinessInterface inDuplexFoob, Player player, string recipient)
        {
            InitializeComponent();

            duplexFoob = inDuplexFoob;
            curPlayer = player;
      
            playerNameTextBox.Text = recipient;
            MessageRecipient = recipient;

            ((MainWindow)Application.Current.MainWindow).UpdatePrivateCallbackContext(GetChatKey(player.Username, recipient), this);
            LoadChatHistory();
        }

        private void LoadChatHistory()
        {
            var messages = duplexFoob.GetPrivateMessages(curPlayer.Username, MessageRecipient);
            foreach (var message in messages)
            {
                AddMessageToListBox($"{message.Sender}: {message.Content}");
            }
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string messageContent = messageBox.Text;
                duplexFoob.SendPrivateMessage(curPlayer.Username, MessageRecipient, messageContent);
                AddMessageToListBox($"{curPlayer.Username}: {messageContent}");
                messageBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void loadNewMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessagesListBox.Items.Clear();
                LoadChatHistory();
                MessageBox.Show("Messages refreshed successfully.", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading new messages: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void leaveChatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Notify the MainWindow that we're closing this chat
                ((MainWindow)Application.Current.MainWindow).ClosePrivateMessagePage(GetChatKey(curPlayer.Username, MessageRecipient));

                // Navigate back to the previous page
                    NavigationService.GoBack();
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error leaving chat: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void HandleIncomingMessage(string sender, string content)
        {
            Dispatcher.Invoke(() =>
            {
                AddMessageToListBox($"{sender}: {content}");
            });
        }

        public void AddMessageToListBox(string message)
        {
            MessagesListBox.Items.Add(message);
            
        }

        private string GetChatKey(string user1, string user2)
        {
            return string.Compare(user1, user2, StringComparison.Ordinal) < 0
                ? $"{user1}:{user2}"  //checks to open the same chat between both players
                : $"{user2}:{user1}"; //no matter who the sender is and who the recipent is
        }
    }
}