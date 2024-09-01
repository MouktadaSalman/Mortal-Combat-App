using DataServer;
using Mortal_Combat_Data_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatDataServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]

    // Add the implementation of the DataInterface
    internal class DataInterfaceImpl : DataInterface
    {

        private readonly PlayerDatabase _playerDatabase = PlayerDatabase.Instance;
        private readonly LobbyDatabase _lobbyDatabase = LobbyDatabase.Instance;
        // to add a player to the selected existing lobby
        void DataInterface.AddPlayerToServer(string pUserName) 
        {
            _playerDatabase.AddNewPlayerToServer(pUserName);
        }

        void DataInterface.CreateLobby(string lobbyName)
        {
            _lobbyDatabase.CreateLobby(lobbyName);
        }

        void DataInterface.CreateMessage(string sender, string recipent, object content, int messageType, DateTime dateTime)
        {
            Message message = new Message(sender, recipent, content, messageType, dateTime);    
            
        }

        void DataInterface.DeleteLobby(string lobbyName, Lobby lobbyToDelete)
        {
            _lobbyDatabase.RemoveLobby(lobbyToDelete);
        }

        void DataInterface.DistributeMessage(string lobbyName, string sender, string recipent, object content, int messageType, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        void DataInterface.RemovePlayerFromServer(string pUserName, Player playerToRemove)
        {
            _playerDatabase.RemovePlayerFromServer(playerToRemove);
        }

        Player DataInterface.GetPlayerUsingUsername(string username)
        {
            List<Player> players = _playerDatabase.GetPlayers();

            // loop throught all players in database and return the player that matches the imported username
            foreach (Player p in players)
            {
                if (p.Username == username)
                {
                    return p;
                }
            }
            return null; // player with imported username doesn't exist
        }
        
        Lobby DataInterface.GetLobbyUsingName(string lobbyName)
        {
            List<Lobby> lobbies = _lobbyDatabase.GetLobbies();

            // find the lobby that matches the imported lobby name and return it
            foreach (Lobby l in lobbies)
            {
                if (l.LobbyName == lobbyName)
                {
                    return l;
                }
            }
            return null; // no lobby exists with the imported lobby name
        }
    }
}
