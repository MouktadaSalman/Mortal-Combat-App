/* 
 * Module: MainWindow
 * Description: This is the main window which will be acting as the base of the entire GUI for this 
 *              program, other pages will be kind of navigated through inside of the main window.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, , 21494299
 * Version: 1.0.0.2
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.ServiceModel;
using MortalCombatBusinessServer;
using Mortal_Combat_Data_Library;

namespace MortalCombatClient
{
    public partial class MainWindow : Window
    {
        private BusinessInterface duplexFoob;
        private Callbacks Callbacks;
        private Dictionary<string, PrivateMessagePage> privateMessagePages;
        private LobbyPage LobbyPage;
        private Player currentPlayer;
        InstanceContext callbackInstance;

        public MainWindow()
        {
            InitializeComponent();

            Callbacks = new Callbacks();
            privateMessagePages = new Dictionary<string, PrivateMessagePage>();
            callbackInstance = new InstanceContext(Callbacks);

            CreateChannel();

            MainFrame.NavigationService.Navigate(new LoginPage(duplexFoob));
        }

        public void CreateChannel()
        {
            /*
             * This line creates a context for the callbacks, allowing the server
             * to call back to the client.Used for setting up the duplex channel
             * for communication between client and server.
             * It will be changed between inLobbyPage and privateMessagingPage when required.
             * */
            
            DuplexChannelFactory<BusinessInterface> channelFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            tcp.SendTimeout = TimeSpan.FromMinutes(5);
            tcp.ReceiveTimeout = TimeSpan.FromMinutes(5);
            tcp.OpenTimeout = TimeSpan.FromMinutes(1);
            tcp.CloseTimeout = TimeSpan.FromMinutes(1);

            string URL = "net.tcp://localhost:8200/MortalCombatBusinessService";
            channelFactory = new DuplexChannelFactory<BusinessInterface>(callbackInstance, tcp, URL);
            duplexFoob = channelFactory.CreateChannel();
            
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is InLobbyPage inLobbyPage)
            {
                UpdateInLobbyCallbackContext(inLobbyPage);
            }
            else if (e.Content is PrivateMessagePage privateMessagePage)
            {
                UpdatePrivateCallbackContext(privateMessagePage.MessageRecipient, privateMessagePage);
            } else { 
                          
                Callbacks.UpdateInLobbyPage(null);
                foreach (var recipient in privateMessagePages.Keys)
                {
                    Callbacks.UpdatePrivatePage(recipient, null);
                }
                privateMessagePages.Clear();
            }
        }

        public void UpdateInLobbyCallbackContext(InLobbyPage lobbyPage)
        {
            Callbacks.UpdateInLobbyPage(lobbyPage);
        }

        public void UpdatePrivateCallbackContext(string recipient, PrivateMessagePage privateMessagePage)
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