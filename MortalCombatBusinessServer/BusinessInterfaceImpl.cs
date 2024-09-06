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

        private ConcurrentDictionary<string, List<PlayerCallback>> allLobbies = new ConcurrentDictionary<string,List<PlayerCallback>>();
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


            if (!allLobbies.ContainsKey(lobby.LobbyName))
            {
                allLobbies[lobby.LobbyName] = new List<PlayerCallback>();
                Console.WriteLine($" {lobby.LobbyName} created  in dictionary ");
            }
            

        }
        public void AddPlayertoLobby(Player player, string lobbyName)
        {
            data.AddPlayerToLobby(player, lobbyName);
            PlayerCallback callback = OperationContext.Current.GetCallbackChannel<PlayerCallback>();


            if( allLobbies.ContainsKey(lobbyName))
            {
                var playerCallbacks = allLobbies[lobbyName];


                if (!playerCallbacks.Contains(callback))
                {
                    playerCallbacks.Add(callback);
                }



            }

           PlayerCallbackManager.Instance.AddPlayerCallback(player.Username, callback);


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
         * Parameters: (string mSender, string mRecipent, object mContent, int mMessageType)
         * 
         */



        public void SendPrivateMessage(string sender, string recipent, object content)
        {
            data.CreateMessage(sender, recipent, content, 1);
            
            NotifyPrivatePlayer(sender, recipent, content.ToString());
        }

        public List<MessageDatabase.Message> GetPrivateMessages(string sender, string recipent)
        {
            return data.GetPrivateMessages(sender, recipent);
        }

        public void NotifyPrivatePlayer(string sender, string recipent, string content)
        {

            PlayerCallbackManager.Instance.ListAllPlayersInCallbacks();

            var callback = PlayerCallbackManager.Instance.GetPlayerCallback(recipent);

            if (callback != null)
            {
                MessageDatabase.Message message = new MessageDatabase.Message(sender, recipent, content, 1);
                

                callback.ReceivePrivateMessage(sender, recipent, content);
            } else
            {
                Console.WriteLine("error notifying");
            }

        }


        //lobby messages
        public void DistributeMessageToLobby(string lobbyName, string sender, object content)
        {
            data.CreateMessage(sender, lobbyName, content, 1);
            NotifyDistributedMessages(lobbyName, sender, content.ToString());
        }

        public void DistributeHyperlinkToLobby(string lobbyName, string sender, object content)
        {
            data.CreateMessage(sender, lobbyName, content, 2);
            NotifyDistributedMessages(lobbyName, sender, content);
        }

        public List<MessageDatabase.Message> GetDistributedMessages(string sender, string recipent)
        {
            return data.GetMessagesForLobby(sender, recipent);
        }

        public void NotifyDistributedMessages(string lobbyName, string sender, string content)
        {

            if (allLobbies.ContainsKey(lobbyName))
            {
                var playerCallbacks = allLobbies[lobbyName];
                MessageDatabase.Message message = new MessageDatabase.Message(sender, lobbyName, content, 1);

                //Notify all players in the lobby
                foreach (var callback in playerCallbacks)
                {
                    try
                    {
                        callback.ReceiveLobbyMessage(message.Sender, message.Recipent, message.Content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending message to client: {ex.Message}");
                    }
                }
            }
        }



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
