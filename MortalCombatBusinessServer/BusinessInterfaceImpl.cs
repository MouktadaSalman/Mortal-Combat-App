using DataServer;
using Mortal_Combat_Data_Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatBusinessServer
{

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, IncludeExceptionDetailInFaults = true)]
    public class BusinessInterfaceImpl : BusinessInterface
    {

        private ConcurrentDictionary<string, Lobby> allLobbies = new ConcurrentDictionary<string, Lobby>();
        private ConcurrentDictionary<string, List<Message>> messageQueue = new ConcurrentDictionary<string, List<Message>>();
        private ConcurrentDictionary<string, PlayerCallback> allPlayerCallback = new ConcurrentDictionary<string, PlayerCallback>();
    
        private DataInterface data;
        public BusinessInterfaceImpl() 
        {
            ChannelFactory<DataInterface> dataFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost:8100/MortalCombatDataService";
            dataFactory = new ChannelFactory<DataInterface>(tcp, URL);

            data = dataFactory.CreateChannel();
        }

        public void AddPlayerToServer(string pUserName)
        {
            Player player = data.GetPlayerUsingUsername(pUserName);

            // if player with the imported username doesn't exist, then create a new player with that username.
            if (player == null)
            {
                data.AddPlayerToServer(pUserName);
                Console.WriteLine($"Player created with username: {pUserName}");
            }
            else
            {
                Console.WriteLine("Username has already been taken. " +
                    "Try a different username");
            }
        }

        public void CreateLobby(string lobbyName)
        {
            Lobby lobby = data.GetLobbyUsingName(lobbyName);
            

            // create lobby with imported name only if the name doesn't already exist
            if(lobby == null)
            {
                data.CreateLobby(lobbyName);
                Console.WriteLine($"Lobby created with name: {lobbyName}");
            }
            else
            {
                Console.WriteLine("Lobby name has already been taken. " +
                    "Try a different lobby name");
            }

        }

        public void AddPlayerToLobby(string lobbyName, string username)
        {
            if (string.IsNullOrWhiteSpace(lobbyName) || string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Lobby name and username cannot be null or empty.");
            }

            data.AddPlayerToLobby(lobbyName, username);

            Console.WriteLine($"Joining lobby: {lobbyName}");
            Console.WriteLine($"Player joining: {username}");
            Console.WriteLine($"Player {username} has joined {lobbyName}");
            
        }

        public void DeleteLobby(string lobbyName)
        {
            Lobby lobby = data.GetLobbyUsingName(lobbyName);

            // if lobby with imported name exists, remove it.
            if (lobby != null)
            {
                data.DeleteLobby(lobbyName, lobby);
                Console.WriteLine($"Lobby with name \"{lobbyName}\" " +
                                      $"does not exist in server");
            }
            else
            {
                Console.WriteLine($"Lobby with name \"{lobbyName}\" " +
                    $"does not exist in server");
            }
        }


        /*
         * Method for sending a private message between 2 players (sender, recipent).
         * Parameters: (string mSender, string mRecipent, object mContent, int mMessageType, DateTime mDateTime)
         * 
         */
        public void CreateMessage(string mSender, string mRecipent, object mContent, int mMessageType, DateTime mDateTime)
        {
           
           
            // Searching through all the callbacks if it contains a recipent
           if (allPlayerCallback.ContainsKey(mRecipent))
            {
                
                var callback = allPlayerCallback[mRecipent];

                //Making the message with assigning the sender, content, timeOfmessage
                Message message = new Message();


                message.sender = mSender;
                message.content = mContent;
                message.timeOfMessage = mDateTime;
                
                // insert the message with the previous details into the recieving end.
                callback.ReceivePrivateMessage(message);
            }
            else
            {
                Console.WriteLine($"Recipient {mRecipent} not found  ");
            }
        }
    
        public void DistributeMessage(string lobbyName, string mSender, string mRecipent, object mContent, int mMessageType, DateTime mDateTime)
        {

            //Making the message with assigning the sender, content, timeOfmessage
            Message message = new Message();


            message.sender = mSender;
            message.content = mContent;
            message.timeOfMessage = mDateTime;


            //searching through allLobbies if that lobby exists.
            if (allLobbies.ContainsKey(lobbyName))
            {
                // For-loop to show the message to every player that is in the lobby.
                var lobby = allLobbies[lobbyName];
                foreach (var player in lobby._players) 
                {
                    if (allPlayerCallback.ContainsKey(player.Username))
                    {
                        var callback = allPlayerCallback[player.Username];
                        callback.ReceiveLobbyMessage(message); 
                    }
                }
            }
            else
            {
                Console.WriteLine($"Lobby {lobbyName} not found");
            }
        }

        public void RemovePlayerFromServer(string pUserName)
        {
            Player player = data.GetPlayerUsingUsername(pUserName);

            // remove player with imported username if they exist.
            if (player != null)
            {
                data.RemovePlayerFromServer(pUserName, player);
                Console.WriteLine($"Player with username \"{pUserName}\" " +
                                      $"Removed from the server");
            }
            else
            {
                Console.WriteLine("Username has already been taken. " +
                    "Try a different username");
            }
        }

        public List<Lobby> GetAllLobbies()
        {
            return data.GetAllLobbies();
        }
    }
}
