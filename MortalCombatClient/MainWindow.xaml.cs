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
using System.ServiceModel;
using MortalCombatBusinessServer;
using Mortal_Combat_Data_Library;
using System.Runtime.Remoting.Contexts;

namespace MortalCombatClient
{
    public partial class MainWindow : Window
    {
        private BusinessInterface duplexFoob;
        private callbacks Callbacks;
        private Dictionary<string, privateMessagePage> privateMessagePages;
        private Player currentPlayer;

        public MainWindow()
        {
            InitializeComponent();
            Callbacks = new callbacks();
            privateMessagePages = new Dictionary<string, privateMessagePage>();


            /*
             * This line creates a context for the callbacks, allowing the server
             * to call back to the client.Used for setting up the duplex channel
             * for communication between client and server.
             * It will be changed between inLobbyPage and privateMessagingPage when required.
             * */
            InstanceContext callbackInstance = new InstanceContext(Callbacks);
            DuplexChannelFactory<BusinessInterface> channelFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            tcp.SendTimeout = TimeSpan.FromMinutes(5);
            tcp.ReceiveTimeout = TimeSpan.FromMinutes(5);
            tcp.OpenTimeout = TimeSpan.FromMinutes(1);
            tcp.CloseTimeout = TimeSpan.FromMinutes(1);

            string URL = "net.tcp://localhost:8200/MortalCombatBusinessService";
            channelFactory = new DuplexChannelFactory<BusinessInterface>(callbackInstance, tcp, new EndpointAddress(URL));
            duplexFoob = channelFactory.CreateChannel();
            MainFrame.NavigationService.Navigate(new loginPage(duplexFoob));
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is inLobbyPage lobbyPage)
            {
                UpdateLobbyCallbackContext(lobbyPage);
            }
            else if (e.Content is privateMessagePage privateMessagePage)
            {
                UpdatePrivateCallbackContext(privateMessagePage.MessageRecipient, privateMessagePage);
            }
            else
            {
                
                Callbacks.UpdateLobbyPage(null);
                foreach (var recipient in privateMessagePages.Keys)
                {
                    Callbacks.UpdatePrivatePage(recipient, null);
                }
                privateMessagePages.Clear();
            }
        }


        public void UpdateLobbyCallbackContext(inLobbyPage lobbyPage)
        {
            Callbacks.UpdateLobbyPage(lobbyPage);
        }

        public void UpdatePrivateCallbackContext(string recipient, privateMessagePage privateMessagePage)
        {
            Callbacks.UpdatePrivatePage(recipient, privateMessagePage);
            if (privateMessagePage != null)
            {
                privateMessagePages[recipient] = privateMessagePage;
            }
            else
            {
                privateMessagePages.Remove(recipient);
            }
        }

        public void ClosePrivateMessagePage(string recipient)
        {
            UpdatePrivateCallbackContext(recipient, null);
        }

        public void HandleIncomingPrivateMessage(string sender, string recipient, string content)
        {
            
            StorePrivateMessage(sender, recipient, content);

            
            string chatKey = GetChatKey(sender, recipient);
            if (privateMessagePages.TryGetValue(chatKey, out var privatePage))
            {
                privatePage.HandleIncomingMessage(sender, content);
            }
            
        }

        private string GetChatKey(string user1, string user2)
        {
            // Create a consistent key for the chat regardless of who is sender/recipient
            return string.Compare(user1, user2, StringComparison.Ordinal) < 0
                ? $"{user1}:{user2}"
                : $"{user2}:{user1}";
        }

        private void StorePrivateMessage(string sender, string recipient, string content)
        {
            // Store the message in the database or local storage
            // This is a placeholder - implement according to your data storage method
            duplexFoob.StorePrivateMessage(sender, recipient, content);
        }

        public void SetCurrentPlayer(Player player)
        {
            currentPlayer = player;
        }

        public Player GetCurrentPlayer()
        {
            return currentPlayer;
        }
    }
}