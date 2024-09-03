using DataServer;
using Mortal_Combat_Data_Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.ServiceModel.Security;
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

        public void AddPlayerToServer(Player player)
        {
            data.AddPlayerToServer(player);
        }

        public void AddLobbyToServer(Lobby lobby)
        {
            data.AddLobbyToServer(lobby);
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

        //public void DeleteLobby(string lobbyName)
        //{
        //    Lobby lobby = data.GetLobbyUsingName(lobbyName);

        //    // if lobby with imported name exists, remove it.
        //    if (lobby != null)
        //    {
        //        data.DeleteLobby(lobbyName, lobby);
        //        Console.WriteLine($"Lobby with name \"{lobbyName}\" " +
        //                              $"does not exist in server");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Lobby with name \"{lobbyName}\" " +
        //            $"does not exist in server");
        //    }
        //}


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
    
        //public void DistributeMessage(string lobbyName, string mSender, string mRecipent, object mContent, int mMessageType, DateTime mDateTime)
        //{

        //    //Making the message with assigning the sender, content, timeOfmessage
        //    Message message = new Message();


        //    message.sender = mSender;
        //    message.content = mContent;
        //    message.timeOfMessage = mDateTime;


        //    //searching through allLobbies if that lobby exists.
        //    if (allLobbies.ContainsKey(lobbyName))
        //    {
        //        // For-loop to show the message to every player that is in the lobby.
        //        var lobby = allLobbies[lobbyName];
        //        foreach (var player in lobby._players) 
        //        {
        //            if (allPlayerCallback.ContainsKey(player.Username))
        //            {
        //                var callback = allPlayerCallback[player.Username];
        //                callback.ReceiveLobbyMessage(message); 
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Lobby {lobbyName} not found");
        //    }
        //}

        //public void RemovePlayerFromServer(string pUserName)
        //{
        //    Player player = data.GetPlayerUsingUsername(pUserName);

        //    // remove player with imported username if they exist.
        //    if (player != null)
        //    {
        //        data.RemovePlayerFromServer(pUserName, player);
        //        Console.WriteLine($"Player with username \"{pUserName}\" " +
        //                              $"Removed from the server");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Username has already been taken. " +
        //            "Try a different username");
        //    }
        //}

        public List<string> GetAllLobbyNames()
        {
            return data.GetAllLobbyNames();
        }
    }
}
