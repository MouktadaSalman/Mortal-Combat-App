/*
 * Module: LobbyDatabase
 * Description: Responsible for handling the different
 *              lobby operations
 * Authors: Ahmed, Mouktada, Jauhar
 * ID: 20640266, , 21494299
 * Version: 1.0.0.2
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
        private readonly List<Lobby> lobbies;

        public static LobbyDatabase Instance { get; } = new LobbyDatabase();

        /* 
         * Method: LobbyDatabase
         * Description: Private constructor to instantiate instance
         *              of the lobby database
         * Parameters: none
         */
        private LobbyDatabase()
        {
            lobbies = new List<Lobby>();
        }

        /* 
         * Method: CreateLobby
         */
    }
}
