﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Mortal_Combat_Data_Library
{

    [DataContract]
    public class Player
    {
        /* Class fields:
         * Username -> the username of the player
         * isInLobby -> To check if a player is already inside a lobby
         * DateTime -> to save the time and date of when the player joins the lobby
         */

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string JoinedLobbyName { get; set; }

        [DataMember]        
        public DateTime timeJoinedToLobby { get; set; }

        /* 
         * Method: Player
         * Description: The constructor method of the class,
         *              sets the username to the parameter and 
         *              the default values for 
         * Parameters: username (string)
         */
        public Player(string username, string lobbyName)
        {
            this.Username = username;
            JoinedLobbyName = lobbyName;
            timeJoinedToLobby = DateTime.Now;
        }

    }
}
