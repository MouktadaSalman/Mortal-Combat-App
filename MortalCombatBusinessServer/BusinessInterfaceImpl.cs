/* 
 * Module: File
 * Description: The business logic for the Mortal Combat game
 * Author: Jauhar, Mouktada, Ahmed
 * ID: 21494299, 20640266, 21467369
 * Version: 1.0.0.1
 */
using DataServer;
using Mortal_Combat_Data_Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace MortalCombatBusinessServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    public class BusinessInterfaceImpl : BusinessInterface
    {
        /* Class Fields:
         * allPlayerCallbacks ->   
         * allLobbies -> 
         * data -> 
         */
        private ConcurrentDictionary<string, PlayerCallback> allPlayerCallback = new ConcurrentDictionary<string, PlayerCallback>();
        private ConcurrentDictionary<string, List<PlayerCallback>> allLobbies = new ConcurrentDictionary<string, List<PlayerCallback>>();
        private ConcurrentDictionary<string, List<MessageDatabase.Message>> pendingMessages = new ConcurrentDictionary<string, List<MessageDatabase.Message>>();

        private DataInterface data;

        /* Constructor: BusinessInterfaceImpl
         * Description: The constructor of the business interface
         */
        public BusinessInterfaceImpl()
        {
            // Set up communication with the Data layer
            ChannelFactory<DataInterface> dataFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/MortalCombatDataService";
            dataFactory = new ChannelFactory<DataInterface>(tcp, URL);
            data = dataFactory.CreateChannel();
        }

        /* Method: CreatePlayer
         * Description: Calls the data layer to create a player
         * Parameters: pUserName (string)
         */
        public void AddPlayerToServer(Player player)
        {
            data.AddPlayerToServer(player);
        }

        /* Method: AddLobbyToServer
         * Description: Calls the data layer to add a lobby, also adds the lobby to the dictionary of callbacks
         * Parameters: lobby (Lobby)
         */
        public void AddLobbyToServer(Lobby lobby)
        {
            data.AddLobbyToServer(lobby);

            if (!allLobbies.ContainsKey(lobby.LobbyName))
            {
                allLobbies[lobby.LobbyName] = new List<PlayerCallback>();
                Console.WriteLine($"{lobby.LobbyName} created in dictionary");
            }
        }

        /* Method: AddPlayertoLobby
         * Description: Calls the data layer to add a player to a lobby then adds the player to the dictionary of callbacks
         * Parameters: player (Player), lobbyName (string)
         */
        public void AddPlayertoLobby(Player player, string lobbyName)
        {
            data.AddPlayerToLobby(player, lobbyName);

            PlayerCallback callback = OperationContext.Current.GetCallbackChannel<PlayerCallback>();
        

            if (allLobbies.ContainsKey(lobbyName))
            {
                var playerCallbacks = allLobbies[lobbyName];

                if (!playerCallbacks.Contains(callback))
                {
                    playerCallbacks.Add(callback);
                }
            }

            if (!allPlayerCallback.ContainsKey(player.Username))
            {
                allPlayerCallback.TryAdd(player.Username, callback);
                Console.WriteLine($"Player {player.Username} added to allPlayerCallback.");
            }
            else
            {
                Console.WriteLine($"Player {player.Username} already exists in allPlayerCallback.");
            }
        }

        /* Method: ListAllPlayersInCallbacks
        * Description: lists all players in the dictionary of callbacks
        */
        // List all players in the local instance's allPlayerCallback dictionary
        public void ListAllPlayersInCallbacks()
        {
            if (allPlayerCallback.Count > 0)
            {
                Console.WriteLine("All players in allPlayerCallback:");
                foreach (var player in allPlayerCallback)
                {
                    Console.WriteLine($"Player Username: {player.Key}, Callback: {player.Value}");
                }
            }
            else
            {
                Console.WriteLine("No players in allPlayerCallback.");
            }
        }

        /* Method: CheckUsernameValidity
         * Description: Checks if the username is valid
         * Parameters: username (string), isValid (bool)
         * Result: isValid (bool)
         */
        public void CheckUsernameValidity(string username, out bool isValid)
        {
            int numOfPlayers;
            string foundUsername;
            isValid = true; // Username is valid by default

            data.GetNumOfPlayers(out numOfPlayers);

            // Check if the username already exists by comparing it with all the usernames in the database
            for (int i = 0; i < numOfPlayers; i++)
            {
                data.GetPlayerForIndex(i, out foundUsername);

                if (username.Equals(foundUsername))
                {
                    Console.WriteLine($"username: {foundUsername}, already exists, try a different username!!");
                    isValid = false; // Username is not valid, it already exists
                    return;
                }
            }
        }

        /* Method: CheckLobbyNameValidity
         * Description: Checks if the lobby name is valid
         * Parameters: lobbyName (string), isValid (bool)
         * Result: isValid (bool)
         */
        public void CheckLobbyNameValidity(string lobbyName, out bool isValid)
        {
            int numOfLobbies;
            Lobby foundLobbyName;
            isValid = true; // Lobby name is valid by default

            data.GetNumOfLobbies(out numOfLobbies);

            // Check if the lobby name already exists by comparing it with all the lobby names in the database
            for (int i = 0; i < numOfLobbies; i++)
            {
                data.GetLobbyForIndex(i, out foundLobbyName);

                if (lobbyName.Equals(foundLobbyName.LobbyName))
                {
                    Console.WriteLine($"Lobby name: {foundLobbyName.LobbyName}, already exists, try a different name for the lobby!!");
                    isValid = false; // Lobby name is not valid, it already exists
                    return;
                }
            }
        }

        /* Method: DeleteLobby
        * Description: Deletes a lobby if it has no players
        * Parameters: lobbyName (string), doesHavePlayers (bool)
        * Result: doesHavePlayers (bool)
        */
        public void DeleteLobby(string lobbyName)
        {
            data.GetNumOfLobbies(out int numOfLobbies);
            for (int i = 0; i < numOfLobbies; i++)
            {
                data.GetLobbyForIndex(i, out Lobby foundLobbyName);

                // Check if the passed in lobby name still exists by comparing it with all the lobby names in the database
                if (lobbyName.Equals(foundLobbyName.LobbyName))
                {
                    data.DeleteLobby(i); // Delete the lobby                    
                }
            }
        }

        /* Method: SendPrivateMessage
        * Description: Calls the data layer to create a private message then notifies the recipient
        * Parameters: sender (string), recipent (string), content (string)
        */
        public void SendPrivateMessage(string sender, string recipent, string content)

        // Handle private messages
        public void AddPlayertoLobby(Player player, string lobbyName)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (string.IsNullOrEmpty(lobbyName))
            {
                throw new ArgumentException("Lobby name cannot be null or empty", nameof(lobbyName));
            }

        /* Method: GetPrivateMessages
        * Description: Calls the data layer to get private messages
        * Parameters: sender (string), recipent (string)
        * Result: List of private messages
        */
        public List<MessageDatabase.Message> GetPrivateMessages(string sender, string recipent)
        {
            return data.GetPrivateMessages(sender, recipent);
        }
            // Ensure the lobby exists
            if (!allLobbies.ContainsKey(lobbyName))
            {
                allLobbies[lobbyName] = new List<PlayerCallback>();
                Console.WriteLine($"{lobbyName} created in dictionary");
            }

        /* Method: NotifyPrivatePlayer
        * Description: Notifies the recipient of a private message
        * Parameters: sender (string), recipent (string), content (string)
        */
        public void NotifyPrivatePlayer(string sender, string recipent, string content)
        {
            ListAllPlayersInCallbacks();

            if (allPlayerCallback.ContainsKey(recipent))
            {
                var callback = allPlayerCallback[recipent];
                MessageDatabase.Message message = new MessageDatabase.Message(sender, recipent, content, 1);
            data.AddPlayerToLobby(player, lobbyName);

            PlayerCallback callback = OperationContext.Current.GetCallbackChannel<PlayerCallback>();

            var playerCallbacks = allLobbies[lobbyName];

            if (!playerCallbacks.Contains(callback))
            {
                playerCallbacks.Add(callback);
            }
            

            if (allPlayerCallback.TryAdd(player.Username, callback))
            {
                Console.WriteLine($"Player {player.Username} added to allPlayerCallback.");
                // Deliver any pending messages
                DeliverPendingMessages(player.Username);
            }
            else
            {
                Console.WriteLine($"Player {player.Username} already exists in allPlayerCallback.");
            }
        }

        public void SendPrivateMessage(string sender, string recipient, string content)
        {
            data.CreateMessage(sender, recipient, content, 1);

            if (allPlayerCallback.TryGetValue(recipient, out PlayerCallback callback))
            {
                NotifyPrivatePlayer(sender, recipient, content);
            }
            else
            {
                // Store the message for later delivery
                pendingMessages.AddOrUpdate(recipient,
                    new List<MessageDatabase.Message> { new MessageDatabase.Message(sender, recipient, content, 1) },
                    (key, existingList) =>
                    {
                        existingList.Add(new MessageDatabase.Message(sender, recipient, content, 1));
                        return existingList;
                    });
                Console.WriteLine($"Message from {sender} to {recipient} stored for later delivery.");
            }
        }

        private void DeliverPendingMessages(string username)
        {
            if (pendingMessages.TryRemove(username, out List<MessageDatabase.Message> messages))
            {
                foreach (var message in messages)
                {
                    NotifyPrivatePlayer(message.Sender, message.Recipent, message.Content.ToString());
                }
                Console.WriteLine($"Delivered {messages.Count} pending messages to {username}");
            }
        }

        public void NotifyPrivatePlayer(string sender, string recipient, string content)
        {
            if (allPlayerCallback.TryGetValue(recipient, out PlayerCallback callback))
            {
                try
                {
                    callback.ReceivePrivateMessage(sender, recipient, content);
                    Console.WriteLine($"Private message sent from {sender} to {recipient}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending private message to {recipient}: {ex.Message}");
                    // Store the message for later delivery
                    pendingMessages.AddOrUpdate(recipient,
                        new List<MessageDatabase.Message> { new MessageDatabase.Message(sender, recipient, content, 1) },
                        (key, existingList) =>
                        {
                            existingList.Add(new MessageDatabase.Message(sender, recipient, content, 1));
                            return existingList;
                        });
                }
            }
            else
            {
                Console.WriteLine($"Recipient {recipient} not found in active callbacks. Message from {sender} stored for later delivery.");
                // Store the message for later delivery
                pendingMessages.AddOrUpdate(recipient,
                    new List<MessageDatabase.Message> { new MessageDatabase.Message(sender, recipient, content, 1) },
                    (key, existingList) =>
                    {
                        existingList.Add(new MessageDatabase.Message(sender, recipient, content, 1));
                        return existingList;
                    });
            }
        }


        public void StorePrivateMessage(string sender, string recipient, string content)
        {
            try
            {
                // Store the message in the database
                data.CreateMessage(sender, recipient, content, 1); // Assuming 1 is the type for private messages

                Console.WriteLine($"Stored private message from {sender} to {recipient}");

                // If the recipient is exists, notify them
                if (allPlayerCallback.TryGetValue(recipient, out PlayerCallback callback))
                {
                    try
                    {
                        callback.ReceivePrivateMessage(sender, recipient, content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error notifying recipient {recipient}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error storing private message: {ex.Message}");
                throw;
            }
        }

        public List<MessageDatabase.Message> GetPrivateMessages(string user1, string user2)
        {
            try
            {
                // Retrieve messages where user1 is sender and user2 is recipient
                var messages1 = data.GetPrivateMessages(user1, user2);

                // Retrieve messages where user2 is sender and user1 is recipient
                var messages2 = data.GetPrivateMessages(user2, user1);

                // Combine and sort the messages by timestamp
                var allMessages = messages1.Concat(messages2)
                    
                    .ToList();

                return allMessages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving private messages: {ex.Message}");
                throw;
            }
        }

        //------Distribute messages to the lobby (Both text + hyper-links)-------//
        /* Method: DistributeMessageToLobby
        * Description: Calls the data layer to create a text message then notifies all players in the lobby
        * Parameters: sender (string), recipent (string), content (string)
        */
        public void DistributeMessageToLobby(string lobbyName, string sender, string content)
        {
            data.CreateMessage(sender, lobbyName, content, 1);
            NotifyDistributedMessages(lobbyName, sender, content.ToString());
        }

        /* Method: DistributeMessageToLobbyF
        * Description: Calls the data layer to create a hyper-link message then notifies all players in the lobby
        * Parameters: sender (string), recipent (string), content (MessageDatabase.FileLinkBlock)
        */
        public void DistributeMessageToLobbyF(string lobbyName, string sender, MessageDatabase.FileLinkBlock content)
        {
            data.CreateMessageF(sender, lobbyName, content, 2);
            NotifyDistributedMessagesF(lobbyName, sender, content);
        }
        //-----------------------------------------------------------------------//

        /* Method: GetDistributedMessages
        * Description: Calls the data layer to get messages for the lobby
        * Parameters: sender (string), recipent (string)
        */
        public List<MessageDatabase.Message> GetDistributedMessages(string sender, string recipent)
        {
            return data.GetMessagesForLobby(sender, recipent);
        }

        //----------------Notify both text + hyper-links messages----------------//
        /* Method: NotifyDistributedMessages
        * Description: Notifies all players in the lobby of a text message
        * Parameters: sender (string), recipent (string), content (string)
        */
        public void NotifyDistributedMessages(string lobbyName, string sender, string content)
        {
            if (allLobbies.ContainsKey(lobbyName))
            {
                var playerCallbacks = allLobbies[lobbyName];

                // Notify all players in the lobby
                foreach (var callback in playerCallbacks)
                {
                    try
                    {
                        callback.ReceiveLobbyMessage(sender, lobbyName, content.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending message to client: {ex.Message}");
                    }
                }
            }
        }

        /* Method: NotifyDistributedMessagesF
        * Description: Notifies all players in the lobby of a hyper-link message
        * Parameters: sender (string), recipent (string), content (MessageDatabase.FileLinkBlock)
        */
        public void NotifyDistributedMessagesF(string lobbyName, string sender, MessageDatabase.FileLinkBlock content)
        {
            if (allLobbies.ContainsKey(lobbyName))
            {
                var playerCallbacks = allLobbies[lobbyName];

                // Notify all players in the lobby
                foreach (var callback in playerCallbacks)
                {
                    try
                    {
                        callback.ReceiveLobbyMessageF(sender, lobbyName, content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending message to client: {ex.Message}");
                    }
                }
            }
        }
        //----------------------------------------------------------------------//

        /* Method: RemovePlayerFromServer
        * Description: Removes a player from the dictionary of callbacks
        * Parameters: pUsername (string)
        */
        public void RemovePlayerFromServer(string pUsername)
        {
            if (allPlayerCallback.ContainsKey(pUsername))
            {
                allPlayerCallback.TryRemove(pUsername, out _);
                Console.WriteLine($"Player {pUsername} removed from allPlayerCallback.");
            }
        }

        /* Method: GetAllLobbyNames
        * Description: Calls the data layer to get all lobby names
        * Returns: List of all lobby names
        */
        public List<string> GetAllLobbyNames()
        {
            return data.GetAllLobbyNames();
        }

        /* Method: GetPlayersInLobby
        * Description: Calls the data layer to get all players in a lobby
        * Returns: List of all lobby names
        */
        public List<string> GetPlayersInLobby(Lobby lobby)
        {
            return data.GetAllPlayersInlobby(lobby);
        }

        /* Method: UploadFile
        * Description: Calls the data layer to upload a file using it's path
        * Parameters: filePath (string)
        */
        public void UploadFile(string filePath)
        {
            data.UploadFile(filePath);
        }

        /* Method: DownloadFile
        * Description: Downloads a file using it's name
        * Parameters: fileName (string)
        */
        public void DownloadFile(string fileName)
        {
            //Initialize values
            byte[] fData = null;
            int fType = 0;

            //Get the path to the apps downloads folder
            string downloadPath = @"" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                                        "MortalCombatDownloads");

            //If the app downloads folder doesn't exist yet... create it
            if (!Directory.Exists(downloadPath)) { Directory.CreateDirectory(downloadPath); }

            //Check iff app still encounters issues in pathing the custom downloads folder
            if (Directory.Exists(downloadPath))
            {
                //Set the final path
                string finalPath = Path.Combine(downloadPath, fileName);

                //Extract file details
                data.RetrieveFile(fileName, out fData, out fType);

                //If its an image
                if (fType == 1)
                {
                    // Using MemoryStream for image conversion
                    using (MemoryStream ms = new MemoryStream(fData))
                    {
                        using (Image image = Image.FromStream(ms))
                        {
                            image.Save(finalPath); // Save using the provided file name (already has extension in its name)
                            Console.WriteLine($"Image saved to: {finalPath}");
                        }
                    }
                }
                //If it is a text file
                else if (fType == 2)
                {
                    File.WriteAllBytes(finalPath, fData);
                    Console.WriteLine($"File saved to: {finalPath}");
                }
                //If encountered unknown filetype
                else { Console.WriteLine("Encountered unkown file type"); }

                //Open the file explorer to show it has been downloaded
                Process.Start("explorer.exe", finalPath);
            }
            else { Console.WriteLine("DirectoryNotFound:: Failed to path towards the downloads folder"); }
        }
    }
}