using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortal_Combat_Data_Library
{
    public class Player
    {
        /* Class fields:
         * Username -> the username of the player
         * isInLobby -> To check if a player is already inside a lobby
         * DateTime -> to save the time and date of when the player joins the lobby
         */
        public string Username { get; set; }
        public bool isInLobby{ get; set; } // do we actually need this? we can just have the list of player in each lobby through the lobby class
        public DateTime timeJoinedToLobby { get; }

        /* 
         * Method: Player
         * Description: The constructor method of the class,
         *              sets the username to the parameter and 
         *              the default values for 
         * Parameters: username (string)
         */
        public Player(string username)
        {
            this.Username = username;
            isInLobby = false;
            timeJoinedToLobby = DateTime.Now;
        }

    }
}
