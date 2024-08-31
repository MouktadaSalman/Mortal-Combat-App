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
using System.Text;
using System.Threading.Tasks;

namespace Mortal_Combat_Data_Library
{
    public class LobbyDatabase
    {
        /* Class fields:
         * lobbies -> the list of lobby rooms in the menu
         */
        private readonly List<Lobby> _lobbies;

        public static LobbyDatabase Instance { get; } = new LobbyDatabase();

        static LobbyDatabase() { }


        /* Method: LobbyDatabase
         * Description: Private constructor to instantiate instance
         *              of the lobby database
         * Parameters: none
         */
        private LobbyDatabase()
        {
            _lobbies = new List<Lobby>();
        }

        /* Method: CreateLobby
         * Description: Create a lobby
         * Parameter: lobbyName (string)
         * Result: none
         */
        public void CreateLobby(string lobbyName)
        {

            //Create new lobby
            Lobby newLobby = new Lobby(lobbyName);

            //Add new lobby
            _lobbies.Add(newLobby);
        }

        /* Method: RemoveLobby
         * Description: Remove a lobby
         * Parameter: lobbyName (string)
         * Result: none
         */
        public void RemoveLobby(Lobby lobbyToRemove)
        {
            _lobbies.Remove(lobbyToRemove);
        }

        /* Method: AddPlayer
         * Description: Add a player to the appropriate lobby
         * Parameters: player (Player), lobbyName (string)
         * Result: none
         */
        public void AddPlayer(Player player, string lobbyName)
        {
            Lobby correctLobby = null;

            foreach (Lobby lob in _lobbies)
            {
                if(lob.lobbyName == lobbyName)
                {
                    correctLobby = lob;
                    break;
                }
            }

            //Check if lobby exists in database
            if (correctLobby == null)
            {
                Console.WriteLine("MissingLobbyError:: No matching lobby found");
            }
            else
            {
                correctLobby.AddPlayer(player);
            }
        }

        /* Method: RemovePlayer
         * Description: Remove player from lobby
         * Parameters: playerName (string), lobbyName (string)
         * Result none
         */
        public void RemovePlayer(string playerName,  string lobbyName)
        {
            Lobby correctLobby = null;

            foreach(Lobby lob in _lobbies)
            {
                if(lob.lobbyName == lobbyName)
                {
                    correctLobby= lob;
                    break;
                }
            }

            //Check if lobby exists in database
            if (correctLobby == null)
            {
                Console.WriteLine("MissingLobbyError:: No matching lobby found");
            }
            else
            {
                correctLobby.RemovePlayer(playerName);
            }
        }

        public List<Lobby> GetLobbies()
        {
            return _lobbies;
        }
    }
}
