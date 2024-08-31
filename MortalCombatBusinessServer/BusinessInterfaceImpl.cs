using DataServer;
using Mortal_Combat_Data_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatBusinessServer
{
    internal class BusinessInterfaceImpl : BusinessInterface
    {

        private Dictionary<string, Lobby> allLobbies = new Dictionary<string, Lobby>();
        private Dictionary<string, List<Message>> messageQueue = new Dictionary<string, List<Message>>();
        private Dictionary<string, PlayerCallback> allPlayerCallback = new Dictionary<string, PlayerCallback>();
    
        
        private DataInterface data;
        private BusinessInterfaceImpl() 
        {
            ChannelFactory<DataInterface> dataFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            string URL = "net.tcp://localhost:8100/MortalCombatDataService";
            dataFactory = new ChannelFactory<DataInterface>(tcp, URL);

            data = dataFactory.CreateChannel();
        }

        public void AddPlayerToServer(string pUserName)
        {
            Player player = this.GetPlayerUsingUsername(pUserName);

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
            Lobby lobby = this.GetLobbyUsingName(lobbyName);

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

        public void DeleteLobby(string lobbyName)
        {
            Lobby lobby = this.GetLobbyUsingName(lobbyName);

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

        public void CreateMessage(string msender, string mrecipent, object mcontent, int mmessageType, DateTime mdateTime)
        {
           
           
           if (allPlayerCallback.ContainsKey(mrecipent))
            {
                var callback = allPlayerCallback[mrecipent];
                Message message = new Message();


                message.sender = msender;
                message.content = mcontent;
                message.timeOfMessage = mdateTime;
                
                callback.ReceivePrivateMessage(message);
            }
            else
            {
                Console.WriteLine("Recipient not found  ");
            }
        }
    
        

        

        public void DistributeMessage(Message message)
        {


            throw new NotImplementedException();
        }

        
        public void RemovePlayerFromServer(string pUserName)
        {
            Player player = this.GetPlayerUsingUsername(pUserName);

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

        public Player GetPlayerUsingUsername(string username)
        {
            return data.GetPlayerUsingUsername(username);
        }

        public Lobby GetLobbyUsingName(string lobbyName)
        {
            return data.GetLobbyUsingName(lobbyName);
        }

        public List<Lobby> GetAllLobbies()
        {
            throw new NotImplementedException();
        }
    }
}
