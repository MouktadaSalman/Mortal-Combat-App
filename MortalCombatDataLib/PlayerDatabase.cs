/*
 * Module: PlayerDatabase
 * Description: The operations to store and retrieve player data
 * Authors: Mouktada
 * ID: 20640266
 * Version: 1.0.0.3
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortal_Combat_Data_Library
{
    public class PlayerDatabase
    {
        /* Class fields:
         * _players -> the list of players in the entire server
         * Instance -> allows a single instance of the player database
         */
        public readonly List<Player> _players;
        public static PlayerDatabase Instance { get; } = new PlayerDatabase();

        /* Method: PlayerDatabase
         * Description: Static constructor to instantiate instance
         *              of the player database
         */
        static PlayerDatabase() { }

        /* Method: PlayerDatabase
         * Description: Private constructor to instantiate instance
         *              of the player database
         * Parameters: none
         */
        public PlayerDatabase()
        {
            _players = new List<Player>();
        }

        /* Method: AddPlayerToServer
         * Description: Add a player to the server
         * Parameters: player (Player)
         */
        public void AddPlayerToServer(Player player)
        {
            _players.Add(player);
        }

        /* Method: RemovePlayerFromServer
         * Description: Remove a player from the server
         * Parameters: player (Player)
         */
        public void RemovePlayerFromServer(int index)
        {
            _players.RemoveAt(index);
        }

        public Player GetPlayerByIndex(int index)
        {
            return _players[index];
        }
    }
}
