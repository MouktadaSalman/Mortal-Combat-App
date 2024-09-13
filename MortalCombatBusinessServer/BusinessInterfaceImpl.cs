/* 
 * Module: BusinessInterfaceImpl
 * Description: This module is responsible for the business logic of the game.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, , 21494299
 * Version: 1.0.0.2
 */
using DataServer;
using Mortal_Combat_Data_Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Threading.Tasks;

namespace MortalCombatBusinessServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    public class BusinessInterfaceImpl : BusinessInterface
    {

        /* ConcurrentDictionary allows multiple threads to read and write concurrently.*/

        /*This dictionary stores a mapping of player usernames to their associated callback interfaces. 
         * It allows the server to send messages directly to specific players. 
         * Used with both lobby messaging and private messaging.
         */
        private ConcurrentDictionary<string, PlayerCallback> allPlayerCallback = new ConcurrentDictionary<string, PlayerCallback>();

        /*This dictionary stores a mapping of lobby names to lists of player callbacks in each lobby.
         * It's used to manage which players are in which lobbies and to send messages to all players
         * in a specific lobby. 
         */
        private ConcurrentDictionary<string, List<PlayerCallback>> allLobbies = new ConcurrentDictionary<string, List<PlayerCallback>>();


        /*This dictionary stores messages so when the recipent enters the chat they get notified, key is the recipient's username.
         * It allows the server to store messages for  players and deliver them when they come in the chat.
         */

        private ConcurrentDictionary<string, List<MessageDatabase.Message>> pendingMessages = new ConcurrentDictionary<string, List<MessageDatabase.Message>>();

        public string downloadPath;

        private DataInterface data;

        /* Method: BusinessInterfaceImpl
         * Description: Constructor for the BusinessInterfaceImpl class
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

        /* Method: AddPlayerToServer
         * Description: Adds a player to the server
         * Parameters: player (Player)
         */
        public void AddPlayerToServer(Player player)
        {
            data.AddPlayerToServer(player);
        }

        /* Method: AddLobbyToServer
         * Description: Adds a lobby to the server
         * Parameters: lobbyName (string)
         */
        public void AddLobbyToServer(string lobbyName)
        {
            Console.WriteLine($"trying to add lobby = {lobbyName}");
            data.AddLobbyToServer(lobbyName);

            // Check if the lobby name already exists by comparing it with all the lobby names in the callback dictionary
            if (!allLobbies.ContainsKey(lobbyName))
            {
                allLobbies[lobbyName] = new List<PlayerCallback>();
                Console.WriteLine($"{lobbyName} created in dictionary");
            }
        }

        /* Method: CheckUsernameValidity
         * Description: Checks if the username is valid
         * Parameters: username (string)
         */
        public void CheckUsernameValidity(string username)
        {
            data.GetNumOfPlayers(out int numOfPlayers); 
            
            int i = GetIndexForPlayer(username); // gets the index of the player username, return -1 if it doesn't exist

            // if the lobby name already exists, throw an exception
            if (i != -1)
            {
                Console.WriteLine($"username: {username}, already exists, try a different username!!");
                throw new FaultException<PlayerNameAlreadyEsistsFault>(new PlayerNameAlreadyEsistsFault()
                { Issue = "Player name already exists" });
            }            
        }


        /* Method: CheckLobbyNameValidity
         * Description: Checks if the lobby name is valid
         * Parameters: lobbyName (string)
         */
        public void CheckLobbyNameValidity(string lobbyName)
        {
            data.GetNumOfLobbies(out int numOfLobbies);

            // Check if the lobby name already exists by comparing it with all the lobby names in the database

            int i = GetIndexForLobby(lobbyName); // gets the index of the lobby name return, -1 if it doesn't exist


            // if the lobby name already exists, throw an exception
            if (i != -1)
            {
                Console.WriteLine($"Lobby name: {lobbyName}, already exists, try a different name for the lobby!!");
                throw new FaultException<LobbyNameAlreadyExistsFault>(new LobbyNameAlreadyExistsFault()
                { Issue = "Lobby name already taken \nTry a different name" });
            }
        }

        /* Method: DeleteLobby
         * Description: Deletes a lobby from the callback dictionary and the data server
         * Parameters: lobbyName (string)
         */
        public void DeleteLobby(string lobbyName)
        {
            Console.WriteLine($"trying to delete lobby = {lobbyName}");

            // retrieve the list of player callbacks in a lobby
            List<PlayerCallback> playerCallBacks = allLobbies[lobbyName];                    

            Console.WriteLine($"players in lobby {playerCallBacks.Count} for lobby {lobbyName}");

            // Check if the list is empty
            if (playerCallBacks.Count == 0)
            {
                // Delete the lobby form the database
                int i = GetIndexForLobby(lobbyName);
                data.DeleteLobby(i); 

                // Remove the key from the ConcurrentDictionary
                allLobbies.TryRemove(lobbyName, out playerCallBacks);
            }
            else
            {
                throw new FaultException<PlayersStilInLobbyFault>(new PlayersStilInLobbyFault()
                { Issue = "Lobby still has player inside it\nTry again later" });
            }
        }

        /* Method: AddPlayertoLobby
         * Description: Adds a player to a lobby
         * Parameters: player (Player), lobbyName (string)
         */
        public void AddPlayertoLobby(Player player, string lobbyName)
        {
            if (string.IsNullOrEmpty(lobbyName))
            {
                throw new ArgumentException("Lobby name cannot be null or empty", nameof(lobbyName));
            }

            // check if the lobby callback already exists in the alllobbies callback dictionary
            if (!allLobbies.ContainsKey(lobbyName))
            {
                allLobbies[lobbyName] = new List<PlayerCallback>();
                Console.WriteLine($"{lobbyName} created in dictionary");
            }

            data.AddPlayerToLobby(player, lobbyName); // Add the player to the lobby in the database

            PlayerCallback callback = OperationContext.Current.GetCallbackChannel<PlayerCallback>(); // Get the callback channel for the player

            var playerCallbacks = allLobbies[lobbyName]; // Get the list of player callbacks in the lobby


            // Check if the player is already in the lobby, if not add them
            if (!playerCallbacks.Contains(callback)) 
            {
                playerCallbacks.Add(callback);
            }

            // Add the player to the global player list
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

            Console.WriteLine($"Players in  this lobby are {playerCallbacks.Count} ");
        }

        /* Method: RemovePlayerFromLobby
         * Description: Removes a player from a lobby
         * Parameters: playerUsername (string), lobbyName (string)
         */
        public void RemovePlayerFromLobby(string playerUsername, string lobbyName)
        {
            List<PlayerCallback> playerCallBacks = allLobbies[lobbyName]; // Get the list of player callbacks in the lobby
           
            PlayerCallback plCallback = allPlayerCallback[playerUsername]; // Get the player callback for the passed in player

            // Loop through the list of player callbacks in the lobby
            foreach (var pCB in playerCallBacks) 
            {
                // If the player callback is found, remove it
                if (pCB == plCallback) 
                {
                    playerCallBacks.Remove(pCB);

                    int i = GetIndexForLobby(lobbyName);   
                    int j = GetIndexForPlayerInLobby(playerUsername, lobbyName);

                    Console.WriteLine($"returned index for player is ({j}), and for lobby is ({i}).");

                    data.RemovePlayerFromLobby(i, j); // Remove the player from the lobby in the database
             
                    break;
                }
            }
        }

        /* Method: RemovePlayerFromServer
         * Description: Removes a player from the server
         * Parameters: pUsername (string)
         */
        public void RemovePlayerFromServer(string pUserName)
        {
            PlayerCallback plCallback = allPlayerCallback[pUserName];
            
            // Remove the player from the global player list
            allPlayerCallback.TryRemove(pUserName, out plCallback);

            int i = GetIndexForPlayer(pUserName);
            Console.WriteLine($"Player {pUserName} is in index {i}.");            

            data.RemovePlayerFromServer(i);           
        }

        /* Method: GetIndexForPlayer
         * Description: Gets the index of a player in the database
         * Parameters: playerToFind (string)
         */
        public int GetIndexForPlayer(string playerToFind)
        {
            data.GetNumOfPlayers(out int numOfPlayers);

            // Check if the lobby name already exists by comparing it with all the lobby names in the database
            for (int i = 0; i < numOfPlayers; i++)
            {
                data.GetPlayerForIndex(i, out Player foundPlayer);

                if (playerToFind.Equals(foundPlayer.Username)) 
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetIndexForPlayerInLobby(string playerToFind, string lobbyName)
        {
            Lobby lobby = GetLobbyByName(lobbyName);
            int numOfPlayers = lobby._playerInLobby.Count;

            Console.WriteLine($"Count of players in lobby is: {numOfPlayers}");

            // Check if the lobby name already exists by comparing it with all the lobby names in the database
            for (int i = 0; i < numOfPlayers; i++)
            {
                Player player = lobby._playerInLobby[i];
                if (playerToFind.Equals(player.Username))
                {
                    return i;
                }
            }
            return -1;
        }

        /* Method: GetIndexForLobby
         * Description: Gets the index of a lobby in the database
         * Parameters: lobbyToFind (string)
         */
        public int GetIndexForLobby(string lobbyToFind)
        {
            data.GetNumOfLobbies(out int numOfLobbies);

            // Check if the lobby name already exists by comparing it with all the lobby names in the database
            for (int i = 0; i < numOfLobbies; i++)
            {
                data.GetLobbyForIndex(i, out Lobby foundLobby);

                if (lobbyToFind.Equals(foundLobby.LobbyName))
                {
                    return i;
                }
            }
            return -1;
        }

        /* Method: SendPrivateMessage
         * Description: Sends a private message to a player
         * Parameters: sender (string), recipient (string), content (string)
         */
        public void SendPrivateMessage(string sender, string recipient, string content)
        {
            data.CreateMessage(sender, recipient, content, 1);

            if (allPlayerCallback.TryGetValue(recipient, out PlayerCallback callback)) 
            {
                NotifyPrivatePlayer(sender, recipient, content); // Notify the recipient
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

        /* Method: DeliverPendingMessages
         * Description: Delivers pending messages to a player
         * Parameters: username (string)
         */
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

        /* Method: NotifyPrivatePlayer
         * Description: Notifies a player of a private message
         * Parameters: sender (string), recipient (string), content (string)
         */
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

        /* Method: StorePrivateMessage
         * Description: Stores a private message in the database
         * Parameters: sender (string), recipient (string), content (string)
         */
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

        /* Method: GetPrivateMessages
         * Description: Retrieves private messages between two players
         * Parameters: user1 (string), user2 (string)
         * Result: List<MessageDatabase.Message>
         */
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

        /* Method: GetLobbyByName
         * Description: Retrieves a lobby by name
         * Parameters: lobbyName (string)
         * Result: Lobby
         */
        public Lobby GetLobbyByName(string lobbyName)
        {
            data.GetNumOfLobbies(out int numOfLobbies); // Get the number of lobbies in the database
            for (int i = 0; i < numOfLobbies; i++)
            {
                data.GetLobbyForIndex(i, out Lobby foundLobby);
                // Check if the passed in lobby name exists by comparing it with all the lobby names in the database
                if (lobbyName.Equals(foundLobby.LobbyName))
                {
                    return foundLobby;                  
                }                
            }
            return null;
        }
        
        /* Method: GetPlayerByUsername
         * Description: Retrieves a player by username
         * Parameters: username (string)
         * Result: Player
         */
        public Player GetPlayerByUsername(string username)
        {
            data.GetNumOfPlayers(out int numOfPlayers); // Get the number of lobbies in the database
            for (int i = 0; i < numOfPlayers; i++)
            {
                data.GetPlayerForIndex(i, out Player foundPlayer);
                // Check if the passed in lobby name exists by comparing it with all the lobby names in the database
                if (username.Equals(foundPlayer.Username))
                {
                    return foundPlayer;                  
                }                
            }
            return null;
        }

        /* Method: DistributeMessageToLobby
         * Description: Distributes a message to all players in a lobby
         * Parameters: lobbyName (string), sender (string), content (string)
         */
        public void DistributeMessageToLobby(string lobbyName, string sender, string content)
        {
            data.CreateMessage(sender, lobbyName, content, 1);
            NotifyDistributedMessages(lobbyName, sender, content.ToString());
        }

        /* Method: DistributeMessageToLobbyF
         * Description: Distributes a hyper-link message to all players in a lobby
         * Parameters: lobbyName (string), sender (string), content (MessageDatabase.FileLinkBlock)
         */
        public void DistributeMessageToLobbyF(string lobbyName, string sender, MessageDatabase.FileLinkBlock content)
        {
            data.CreateMessageF(sender, lobbyName, content, 2);
            NotifyDistributedMessagesF(lobbyName, sender, content);
        }

        /* Method: GetDistributedMessages
         * Description: Retrieves messages distributed to all players in a lobby
         * Parameters: sender (string), recipent (string)
         * Result: List<MessageDatabase.Message>
         */
        public List<MessageDatabase.Message> GetDistributedMessages(string sender, string recipent)
        {
            return data.GetMessagesForLobby(sender, recipent);
        }

        /* Method: NotifyDistributedMessages
         * Description: Notifies all players in a lobby of a message
         * Parameters: lobbyName (string), sender (string), content (string)
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
         * Description: Notifies all players in a lobby of a hyper-link message
         * Parameters: lobbyName (string), sender (string), content (MessageDatabase.FileLinkBlock)
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

        /* Method: GetAllLobbyNames
         * Description: Retrieves all lobby names
         * Result: List<string>
         */
        public List<string> GetAllLobbyNames()
        {
            return data.GetAllLobbyNames();
        }

        /* Method: UploadFile
         * Description: Uploads a file to the server
         * Parameters: filePath (string)
         */
        public void UploadFile(string filePath)
        {
            data.UploadFile(filePath);
        }

        /* Method: DownloadFile
         * Description: Downloads a file from the server
         * Parameters: fileName (string)
         */
        public void DownloadFile(string fileName)
        {
            //Initialize values
            byte[] fData = null;
            int fType = 0;
            
            //Get the path to the apps downloads folder
            downloadPath = @"" + Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.FullName,
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