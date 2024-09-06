using DataServer;
using Mortal_Combat_Data_Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;

namespace MortalCombatBusinessServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    public class BusinessInterfaceImpl : BusinessInterface
    {
        // A ConcurrentDictionary to manage all player callbacks per instance
        private ConcurrentDictionary<string, PlayerCallback> allPlayerCallback = new ConcurrentDictionary<string, PlayerCallback>();
        private ConcurrentDictionary<string, List<PlayerCallback>> allLobbies = new ConcurrentDictionary<string, List<PlayerCallback>>();

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

        // Add player to a lobby and store their callback
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
        public void SendPrivateMessage(string sender, string recipent, object content)
        {
            data.CreateMessage(sender, recipent, content, 1);

            NotifyPrivatePlayer(sender, recipent, content.ToString());
        }

        public List<MessageDatabase.Message> GetPrivateMessages(string sender, string recipent)
        {
            return data.GetPrivateMessages(sender, recipent);
        }

        // Notify private players using their callback
        public void NotifyPrivatePlayer(string sender, string recipent, string content)
        {
            ListAllPlayersInCallbacks();

            if (allPlayerCallback.ContainsKey(recipent))
            {
                var callback = allPlayerCallback[recipent];
                MessageDatabase.Message message = new MessageDatabase.Message(sender, recipent, content, 1);

                callback.ReceivePrivateMessage(message.Sender, message.Recipent, message.Content.ToString());
            }
            else
            {
                Console.WriteLine($"Error: not notifying private player {sender} |{recipent}|{content}");
            }
        }

        // Distribute messages to the lobby
        public void DistributeMessageToLobby(string lobbyName, string sender, object content, int type)
        {
            if(type == 1)
            {
                data.CreateMessage(sender, lobbyName, content.ToString(), type);
            }
            else if(type == 2)
            {
                data.CreateMessage(sender, lobbyName, content, type);
            }
            else
            {
                Console.WriteLine("Unrecognisable messsage type");
            }

            NotifyDistributedMessages(lobbyName, sender, content, type);
        }

        public List<MessageDatabase.Message> GetDistributedMessages(string sender, string recipent)
        {
            return data.GetMessagesForLobby(sender, recipent);
        }

        public void NotifyDistributedMessages(string lobbyName, string sender, object content, int type)
        {
            if (allLobbies.ContainsKey(lobbyName))
            {
                var playerCallbacks = allLobbies[lobbyName];

                MessageDatabase.Message message = null;

                //General string message
                if(type == 1)
                {
                    message = new MessageDatabase.Message(sender, lobbyName, content.ToString(), 1);
                }
                //File link message
                else if(type == 2)
                {
                    message = new MessageDatabase.Message(sender, lobbyName, content, 2);
                }

                // Notify all players in the lobby
                foreach (var callback in playerCallbacks)
                {
                    try
                    {
                        callback.ReceiveLobbyMessage(message.Sender, message.Recipent, message.Content, message.MessageType);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending message to client: {ex.Message}");
                    }
                }
            }
        }

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
            data.DownloadFile(fileName);
        }
    }
}









