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
        /*
         * Class Fields: 
         * duplexFOob -> the business interface server.
         * privateMessagePages -> dictionary for storing all pages that are on use between playres.
         * currentPlayer -> to define the user of each instance.
         * callbackInstance -> an instance to be used for the duplex channel.
         * _channelLock -> used to ensure thread safety when creating or modifying the communication channel.
         */
        private BusinessInterface duplexFoob;
        private Callbacks Callbacks;
        private Dictionary<string, PrivateMessagePage> privateMessagePages;
        private Player currentPlayer;
        InstanceContext callbackInstance;
        private static readonly object _channelLock = new object();

        /* Constructor: MainWindow
         * Description: The constructor of the MainWindow
         * Parameters: none
         */
        public MainWindow()
        {
            InitializeComponent();

            Callbacks = new Callbacks();
            privateMessagePages = new Dictionary<string, PrivateMessagePage>();


            /*
             * This line creates a context for the callbacks, allowing the server
             * to call back to the client.Used for setting up the duplex channel
             * for communication between client and server.
             * It will be changed between inLobbyPage and privateMessagingPage when required.
             * */
            callbackInstance = new InstanceContext(Callbacks);

            DuplexChannelFactory<BusinessInterface> channelFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            // Setting some timeouts for the communication channel
            tcp.SendTimeout = TimeSpan.FromMinutes(5);
            tcp.ReceiveTimeout = TimeSpan.FromMinutes(5);
            tcp.OpenTimeout = TimeSpan.FromMinutes(1);
            tcp.CloseTimeout = TimeSpan.FromMinutes(1);

            string URL = "net.tcp://localhost:8200/MortalCombatBusinessService";
            channelFactory = new DuplexChannelFactory<BusinessInterface>(callbackInstance, tcp, URL);
            duplexFoob = channelFactory.CreateChannel();

            MainFrame.NavigationService.Navigate(new LoginPage(duplexFoob));
        }


        /* Method: CreateChannel
         * Description: This method creates a new communication channel with the business server. 
         *              If an existing channel is open, it forcefully closes it and creates a new one.
         *              This method ensures that a new channel is always opened within a thread-safe 
         *              lock to prevent concurrency issues.
         * Parameters: none
         */
        public void CreateChannel()
        {
            
            lock (_channelLock)
            {
                try
                {
                    // Closing the current channel if it is not already closed
                    if (duplexFoob != null && ((ICommunicationObject)duplexFoob).State != CommunicationState.Closed)
                    {
                        ((ICommunicationObject)duplexFoob).Abort(); // Forcefully close the current channel
                    }

                    // Creating a new channel with the business server using a duplex connection
                    DuplexChannelFactory<BusinessInterface> channelFactory;
                    NetTcpBinding tcp = new NetTcpBinding();

                    // Setting some timeouts for the communication channel
                    tcp.SendTimeout = TimeSpan.FromMinutes(5);
                    tcp.ReceiveTimeout = TimeSpan.FromMinutes(5);
                    tcp.OpenTimeout = TimeSpan.FromMinutes(1);
                    tcp.CloseTimeout = TimeSpan.FromMinutes(1);

                    string URL = "net.tcp://localhost:8200/MortalCombatBusinessService";
                    channelFactory = new DuplexChannelFactory<BusinessInterface>(callbackInstance, tcp, URL);
                    duplexFoob = channelFactory.CreateChannel();

                    // Openning the new channel
                    ((ICommunicationObject)duplexFoob).Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to recreate the channel: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }



        /* Method: MainFrame_Navigated
         * Description: This method handles the navigation events of the main frame. When a page is navigated to,
         *              it updates the callback context accordingly for either the lobby or private messaging pages.
         *              It also clears any previous private message pages when switching between contexts.
         * Parameters: sender -> the object that triggered the event (the main frame).
         *             e -> the navigation event arguments containing the content of the page being navigated to.
         */
        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // If the navigated page is an InLobbyPage, update the lobby callback context
            if (e.Content is InLobbyPage inLobbyPage)
            {

                UpdateInLobbyCallbackContext(inLobbyPage);
            }
            // If the navigated page is a PrivateMessagePage, update the private message callback context
            else if (e.Content is PrivateMessagePage privateMessagePage)
            {
                UpdatePrivateCallbackContext(privateMessagePage.MessageRecipient, privateMessagePage);
            } else {

                // Clearing the lobby and private message callbacks when switching contexts
                Callbacks.UpdateInLobbyPage(null);
                foreach (var recipient in privateMessagePages.Keys)
                {
                    Callbacks.UpdatePrivatePage(recipient, null);
                }
                privateMessagePages.Clear();
            }
        }


        /* Method: UpdateInLobbyCallbackContext
       * Description: This method updates the callback context for the in-lobby page, ensuring that the
       *              correct page is being used for callback interactions with the server.
       * Parameters: lobbyPage -> the in-lobby page to be updated in the callback context.
       */
        public void UpdateInLobbyCallbackContext(InLobbyPage lobbyPage)
        {
            // Update the callback context with the private message page for the recipient
            Callbacks.UpdateInLobbyPage(lobbyPage);
        }


        /* Method: UpdatePrivateCallbackContext
         * Description: This method updates the callback context for private messaging between players.
         *              It either adds a private message page to the dictionary or removes it based on the given parameters.
         * Parameters: recipient -> the player who is the recipient of the private messages.
         *             privateMessagePage -> the private message page to be added or removed from the callback context.
         */
        public void UpdatePrivateCallbackContext(string recipient, PrivateMessagePage privateMessagePage)
        {
            Callbacks.UpdatePrivatePage(recipient, privateMessagePage);
            if (privateMessagePage != null)
            {

                // Add the private message page to the dictionary
                privateMessagePages[recipient] = privateMessagePage;
            }
            else
            {
                // Remove the private message page from the dictionary if it's null
                privateMessagePages.Remove(recipient);
            }
        }



        /* Method: ClosePrivateMessagePage
         * Description: This method closes the private message page for a given recipient by updating
         *              the callback context for the recipient and setting it to null.
         * Parameters: recipient -> the player for whom the private message page needs to be closed.
         */
        public void ClosePrivateMessagePage(string recipient)
        {
            UpdatePrivateCallbackContext(recipient, null);
        }


        /* Method: HandleIncomingPrivateMessage
         * Description: This method handles incoming private messages. It checks if the private message 
         *              page between the sender and recipient exists, and if it does, it forwards the 
         *              message to the correct page for display.
         * Parameters: sender -> the player sending the message.
         *             recipient -> the player receiving the message.
         *             content -> the content of the message being sent.
         */
        public void HandleIncomingPrivateMessage(string sender, string recipient, string content)
        { 
            

            
            string chatKey = GetChatKey(sender, recipient);
            if (privateMessagePages.TryGetValue(chatKey, out var privatePage))
            {
                privatePage.HandleIncomingMessage(sender, content);
            }
            
        }


        /* Method: GetChatKey
         * Description: This method generates a consistent key for the chat between two players. 
         *              The key is generated in a way that it remains the same regardless of who 
         *              the sender or recipient is.
         * Parameters: user1 -> the first player in the chat.
         *             user2 -> the second player in the chat.
         * Returns: A string representing the unique key for the chat between two players.
         */
        private string GetChatKey(string user1, string user2)
        {
            // Create a consistent key for the chat regardless of who is sender/recipient
            return string.Compare(user1, user2, StringComparison.Ordinal) < 0
                ? $"{user1}:{user2}"
                : $"{user2}:{user1}";
        }


        /* Method: SetCurrentPlayer
         * Description: This method sets the current player for the instance of the MainWindow.
         * Parameters: player -> the Player object representing the current user.
         */
        public void SetCurrentPlayer(Player player)
        {
            currentPlayer = player;
        }

        /* Method: GetCurrentPlayer
         * Description: This method retrieves the current player for the instance of the MainWindow.
         * Returns: A Player object representing the current user.
         */
        public Player GetCurrentPlayer()
        {
            return currentPlayer;
        }
    }
}