/* 
 * Module: Player
 * Description: The operations to store and retrieve player data
 * Author: Mouktada
 * ID: 20640266
 * Version: 1.0.0.1
 */
using System;
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
         */

        [DataMember]
        public string Username { get; set; }


        /* 
         * Method: Player
         * Description: The constructor method of the class,
         *              sets the username to the parameter and 
         *              the default values for 
         * Parameters: username (string), lobbyName (string)
         */
        public Player(string username)
        {
            this.Username = username;
        }

    }
}
