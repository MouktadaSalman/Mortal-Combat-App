/* 
 * Module: Lobby
 * Description: Responsible for individual lobby 
 *              functionalities.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, 21494299
 * Version: 1.0.0.2
 */

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Mortal_Combat_Data_Library
{
    [DataContract]
    public class Lobby
    {
        /* Class fields:
         * LobbyName -> the name of the lobby
         * _playerInLobby -> list of players in the current lobby
         * _messages -> the messages of the current lobby
         */
        [DataMember]
        public string LobbyName { get; set; }
        [DataMember]
        private List<MessageDatabase.Message> _messages;
        [DataMember]
        public List<string> _playerInLobby;

        /* 
         * Method: Lobby
         * Description: The constructor method of the class
         *              also creates a new empty list for
         *              players + messages (for history)
         * Parameters: lobbyID (string), lobbyName (string)
         */
        public Lobby(string lobbyName)
        {
            this.LobbyName = lobbyName;
            _messages = new List<MessageDatabase.Message>();
            _playerInLobby = new List<string>();
        }

        /* 
         * Method: AddPlayer
         * Description: Add a player to the current lobby
         * Parameters: player (Player)
         */
        public void AddPlayer(string player)
        {
            _playerInLobby.Add(player);
        }

        /* 
         * Method: AddMessage
         * Description: Add message to lobby
         * Parameters: message (Message)
         */
        public void AddMessage(MessageDatabase.Message message) 
        {
            _messages.Add(message);
        }

        /* 
         * Method: GetPlayersInLobby
         * Description: Get all players in lobby
         * Result: List of players in lobby
         */
        public List<string> GetPlayersInLobby()
        {
            return _playerInLobby;
        }
    }
}
