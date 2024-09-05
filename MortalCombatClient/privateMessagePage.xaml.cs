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
    public partial class privateMessagePage : Page
    {
        private BusinessInterface duplexFoob;
        private Player curPlayer;
        private string messageRecipent;
        public privateMessagePage(BusinessInterface inDupexFoob, Player player, string recipent)
        {
            InitializeComponent();


            duplexFoob = inDupexFoob;

            curPlayer = player;

            messageRecipent = recipent;
            playerNameTextBox.Text = recipent;

            ((MainWindow)Application.Current.MainWindow).UpdatePrivateCallbackContext(this);

        }



        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
           string messageContent = messageBox.Text;

            duplexFoob.SendPrivateMessage(curPlayer.Username, messageRecipent, messageContent);

            showMessage($"{curPlayer.Username}: {messageContent}");
            messageBox.Clear();
        }

        public void showMessage(string message)
        {

            MessagesListBox.Items.Add(message);
        }

        public void loadNewMessagesButton_Click(object sender, RoutedEventArgs e)
        {

            MessagesListBox.Items.Clear();
            Task task = loadLobbyMessagesAsync();
        }

        public async Task loadLobbyMessagesAsync()
        {

            var Messages = await Task.Run(() => duplexFoob.GetPrivateMessages(curPlayer.Username, messageRecipent));
            foreach (var message in Messages)
            {

                showMessage(message.ToString());
            }
        }


        private void leaveChatButton_Click(Object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
