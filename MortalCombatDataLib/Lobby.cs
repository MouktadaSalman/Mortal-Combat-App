﻿/* 
 * Module: Lobby
 * Description: Responsible for individual lobby 
 *              functionalities.
 * Author: Ahmed, Moukhtada, Jauhar
 * ID: 21467369, 20640266, , 21494299
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
         * lobbyName -> the name of the lobby
         * _players -> list of players in the current lobby
         * _messages -> the messages of the current lobby
         */

        [DataMember]
        public string LobbyName { get; set; }
        public int PlayerCount { get; set; }

        //[DataMember]       
        //public List<Player> _players;

        [DataMember]
        private List<Message> _messages;


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
            //_players = new List<Player>();
            _messages = new List<Message>();
        }

        //public string GetUsernameByIndex(int index)
        //{
        //    return _players[index].Username;
        //}

        /* 
         * Method: AddPlayer
         * Description: Add a player to the current lobby
         * Parameters: player (Player)
         * Result: none
         */
        //public void AddPlayer(Player player)
        //{

        //    _players.Add(player);
        //}

        /* 
         * Method: RemovePlayer
         * Description: Remove a player from current lobby
         * Parameters: playerName (string)
         * Result: none
         */
        //public void RemovePlayer(string playerName)
        //{
        //    //Create a temp as placeholder for player
        //    Player temp = null;
        //    foreach (Player player in _players)
        //    {
        //        //If there is a matching player currently in lobby
        //        if(player.Username.Equals(playerName)) 
        //        {            
        //            temp = player; 
        //            break; 
        //        }
        //    }

        //    if (temp == null)
        //    {
        //        //Going to throw an actual custom exception
        //        Console.WriteLine("MissingPlayerError:: Player not found in lobby");
        //    }
        //    else
        //    {
        //        _players.Remove(temp);
        //    }
        //}

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

        /* 
         * Method: GetPlayerCount
         * Description: Get player count of lobby
         * Parameters: none
         * Result: count (int)
         */
        //public int GetPlayerCount()
        //{
        //    return _players.Count; 
        //}
    }
}
