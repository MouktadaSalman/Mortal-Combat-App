/* 
 * Module: Lobby
 * Description: Responsible for individual lobby 
 *              functionalities.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 20640266, , 21494299
 * Version: 1.0.0.2
 */

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortal_Combat_Data_Library
{
    public class Lobby
    {
        /* Class fields:
         * lobbyID -> the id of the lobby
         * lobbyName -> the name of the lobby
         * _players -> list of players in the current lobby
         * _messages -> the messages of the current lobby
         */
        private string lobbyID { get; set; }
        private string lobbyName { get; set; }

        private List<Player> _players;

        private List<Message> _messages;

        /* 
         * Method: Lobby
         * Description: The constructor method of the class
         * Parameters: lobbyID (string), lobbyName (string)
         */
        public Lobby(string lobbyID, string lobbyName)
        {
            this.lobbyID = lobbyID;
            this.lobbyName = lobbyName;
            _players = new List<Player>();
            _messages = new List<Message>();
        }

        /* 
         * Method: AddPlayer
         * Description: Add a player to the current lobby
         * Parameters: player (Player)
         * Result: none
         */
        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        /* 
         * Method: RemovePlayer
         * Description: Remove a player from current lobby
         * Parameters: playerName (string)
         * Result: none
         */
        public void RemovePlayer(string playerName)
        {
            //Create a temp as placeholder for player
            Player temp = null;
            foreach (Player player in _players)
            {
                //If there is a matching player currently in lobby
                if(player.Username == playerName) 
                {            
                    temp = player; 
                    break; 
                }
            }

            if (temp == null)
            {
                //Going to throw an actual custom exception
                Console.WriteLine("MissingPlayerError:: Player not found in lobby");
            }
            else
            {
                _players.Remove(temp);
            }
        }

        /* 
         * Method: AddMessage
         * Description: Add message to lobby
         * Parameters: message (Message)
         * Result: none
         */
        public void AddMessage(Message message) 
        {
            _messages.Add(message);
        }
    }
}
