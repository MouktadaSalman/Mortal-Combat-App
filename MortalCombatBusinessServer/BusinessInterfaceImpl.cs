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
        // A ConcurrentDictionary to manage all player callbacks per instance
        private ConcurrentDictionary<string, PlayerCallback> allPlayerCallback = new ConcurrentDictionary<string, PlayerCallback>();
        private ConcurrentDictionary<string, List<PlayerCallback>> allLobbies = new ConcurrentDictionary<string, List<PlayerCallback>>();
        private ConcurrentDictionary<string, List<MessageDatabase.Message>> pendingMessages = new ConcurrentDictionary<string, List<MessageDatabase.Message>>();

        private DataInterface data;

        public BusinessInterfaceImpl()
        {
            // Set up communication with the Data layer
            ChannelFactory<DataInterface> dataFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/MortalCombatDataService";
            dataFactory = new ChannelFactory<DataInterface>(tcp, URL);
            data = dataFactory.CreateChannel();
        }

        // Add a player to the server and their callback to the allPlayerCallback dictionary
        public void AddPlayerToServer(Player player)
        {
            data.AddPlayerToServer(player);
        }

        // Add lobby to the server and ensure it's in the allLobbies dictionary
        public void AddLobbyToServer(Lobby lobby)
        {
            data.AddLobbyToServer(lobby);

            if (!allLobbies.ContainsKey(lobby.LobbyName))
            {
                allLobbies[lobby.LobbyName] = new List<PlayerCallback>();
                Console.WriteLine($"{lobby.LobbyName} created in dictionary");
            }
        }



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



        public void CheckUsernameValidity(string username, out bool isValid)
        {
            int numOfPlayers;
            string foundUsername;
            isValid = true;

            data.GetNumOfPlayers(out numOfPlayers);
            for (int i = 0; i < numOfPlayers; i++)
            {
                data.GetPlayerForIndex(i, out foundUsername);

                if (username.Equals(foundUsername))
                {
                    Console.WriteLine($"username: {foundUsername}, already exists, try a different username!!");
                    isValid = false;
                    return;
                }
            }
        }


        /* Method: CheckLobbyNameValidity
         * Description: Checks if the lobby name is valid
         * Parameters: lobbyName (string), isValid (bool)
         * Result: isValid (bool)
         */
        public void CheckLobbyNameValidity(string lobbyName)
        {
            data.GetNumOfLobbies(out int numOfLobbies);

            // Check if the lobby name already exists by comparing it with all the lobby names in the database
            for (int i = 0; i < numOfLobbies; i++)
            {
                data.GetLobbyForIndex(i, out Lobby foundLobby);
                
                if (lobbyName.Equals(foundLobby.LobbyName))
                {
                    Console.WriteLine($"Lobby name: {foundLobby.LobbyName}, already exists, try a different name for the lobby!!");
                    throw new FaultException<LobbyNameAlreadyExistsFault>(new LobbyNameAlreadyExistsFault()
                    { Issue = "Lobby name already exists" });
                }
            }
        }

        public void DeleteLobby(string lobbyName, out bool doesHavePlayers)
        {

            bool lobbyHasPlayers = false;
            data.GetNumOfLobbies(out int numOfLobbies);

            for (int i = 0; i < numOfLobbies; i++)
            {
                data.GetLobbyForIndex(i, out foundLobbyName);
                if (lobbyName.Equals(foundLobbyName.LobbyName))
                {

                    data.DeleteLobby(foundLobbyName, out lobbyHasPlayers); // Delete the lobby                    
                    break;
                }

            }
            if (lobbyHasPlayers)
            {
                throw new FaultException<PlayersStilInLobbyFault>(new PlayersStilInLobbyFault()
                { Issue = "Players still in lobby" });
            }
        }


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

            // Ensure the lobby exists
            if (!allLobbies.ContainsKey(lobbyName))
            {
                allLobbies[lobbyName] = new List<PlayerCallback>();
                Console.WriteLine($"{lobbyName} created in dictionary");
            }

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
        public void DistributeMessageToLobby(string lobbyName, string sender, string content)
        {
            data.CreateMessage(sender, lobbyName, content, 1);
            NotifyDistributedMessages(lobbyName, sender, content.ToString());
        }

        public void DistributeMessageToLobbyF(string lobbyName, string sender, MessageDatabase.FileLinkBlock content)
        {
            data.CreateMessageF(sender, lobbyName, content, 2);
            NotifyDistributedMessagesF(lobbyName, sender, content);
        }
        //-----------------------------------------------------------------------//

        public List<MessageDatabase.Message> GetDistributedMessages(string sender, string recipent)
        {
            return data.GetMessagesForLobby(sender, recipent);
        }

        //----------------Notify both text + hyper-links messages----------------//
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

        // Remove player from the server
        public void RemovePlayerFromServer(string pUserName)
        {
            if (allPlayerCallback.ContainsKey(pUserName))
            {
                allPlayerCallback.TryRemove(pUserName, out _);
                Console.WriteLine($"Player {pUserName} removed from allPlayerCallback.");
            }
        }

        public List<string> GetAllLobbyNames()
        {
            return data.GetAllLobbyNames();
        }

        public List<string> GetPlayersInLobby(Lobby lobby)
        {
            return data.GetAllPlayersInlobby(lobby);
        }

        //File sharing functionalities
        public void UploadFile(string filePath)
        {
            data.UploadFile(filePath);
        }

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

        public void DeleteLobby(string lobbyName)
        {
            throw new NotImplementedException();
        }
    }
}