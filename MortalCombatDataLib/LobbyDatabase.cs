/*
 * Module: LobbyDatabase
 * Description: Responsible for handling the different
 *              lobby operations
 * Authors: Ahmed, Mouktada, Jauhar
 * ID: 21467369, 20640266, , 21494299
 * Version: 1.0.0.3
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Mortal_Combat_Data_Library
{
    public class LobbyDatabase
    {
        /* Class fields:
         * lobbies -> the list of lobby rooms in the menu
         * Instance -> allows a single instance of the lobby database
         */
        public readonly List<Lobby> _lobbies;
        public static LobbyDatabase Instance { get; } = new LobbyDatabase();

        /* Method: LobbyDatabase
         * Description: Static constructor to instantiate instance
         *              of the lobby database
         */
        static LobbyDatabase() { }

        /* Method: LobbyDatabase
         * Description: Private constructor to instantiate instance
         *              of the lobby database
         */
        private LobbyDatabase()
        {
            _lobbies = new List<Lobby>();
        }

        /* Method: GetLobbyNameByIndex
         * Description: Get the lobby name by index
         * Parameters: index (int)
         * Result: string
         */
        public Lobby GetLobbyNameByIndex(int index)
        {
            return _lobbies[index];
        }

        /* Method: AddNewLobbyToServer
         * Description: Add a new lobby to the server
         * Parameters: newLobby (Lobby)
         */
        public void AddNewLobbyToServer(Lobby newLobby)
        {
            _lobbies.Add(newLobby);
        }

        public void RemoveLobbyFromServer(Lobby lobby)
        {
            _lobbies.Remove(lobby);
        }

        /* Method: AddPlayerToLobby
         * Description: Add a player to the appropriate lobby
         * Parameters: player (Player), lobbyName (string)
         */
        public void AddPlayerToLobby(Player player, string lobbyName)
        {
            Lobby lobby = GetLobby(lobbyName);

            lobby.AddPlayer(player);
        }



        /* Method: GetLobbies
         * Description: Get the list of lobbies
         * Result: List<Lobby>
         */
        public List<Lobby> GetLobbies()
        {
            return _lobbies;
        }

        /* Method: GetPlayersInLobby
         * Description: Get the list of players in a lobby
         * Parameters: lobby (Lobby)
         * Result: List<string>
         */
        public List<string> GetPlayersInLobby(Lobby lobby)
        {
            lobby = GetLobby(lobby.LobbyName);
            return lobby.GetPlayersInLobby();
        }

        /* Method: GetLobby
         * Description: Get the lobby by name
         * Parameters: lobbyName (string)
         * Result: Lobby
         */
        public Lobby GetLobby(string lobbyName)
        {
            foreach (Lobby l in _lobbies)
            {
                if (l.LobbyName.Equals(lobbyName))
                {
                    return l;
                }
            }
            return null;
        }
    }
}
