﻿using DataServer;
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

        /*
         * ConcurrentDictionary allows multiple threads to read and write concurrently.
         *
         * 
         * /


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

        public void CheckLobbyNameValidity(string lobbyName, out bool isValid)
        {
            int numOfLobbies;
            string foundLobbyName;
            isValid = true;

            data.GetNumOfLobbies(out numOfLobbies);
            for (int i = 0; i < numOfLobbies; i++)
            {
                data.GetLobbyForIndex(i, out foundLobbyName);

                if (lobbyName.Equals(foundLobbyName))
                {
                    Console.WriteLine($"Lobby name: {foundLobbyName}, already exists, try a different name for the lobby!!");
                    isValid = false;
                    return;
                }
            }
        }

        public void DeleteLobby(string lobbyName, out bool doesHavePlayers)
        {
            int numOfLobbies;
            string foundLobbyName;
            doesHavePlayers = false;

            data.GetNumOfLobbies(out numOfLobbies);
            for (int i = 0; i < numOfLobbies; i++)
            {
                data.GetLobbyForIndex(i, out foundLobbyName);
                if (lobbyName.Equals(foundLobbyName))
                {
                    int lobbyCount;
                    data.GetPlayersInLobbyCount(i, out lobbyCount);
                    if (lobbyCount > 0)
                    {
                        doesHavePlayers = true;
                        return;
                    }
                    else
                    {
                        data.DeleteLobby(i);
                    }
                }
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
    }
}