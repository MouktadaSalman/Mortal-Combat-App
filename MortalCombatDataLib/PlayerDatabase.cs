/*
 * Module: PlayerDatabase
 * Description: Responsible for handling the different
 *              Player operations for the entire server.
 * Authors: Ahmed, Mouktada, Jauhar
 * ID: 20640266, , 21494299
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
         * lobbies -> the list of players in the server in the menu
         */
        private readonly List<Player> _players;
        public static PlayerDatabase Instance { get; } = new PlayerDatabase();

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

        /* Method: AddNewPlayer
         * Description: Add a new player to the list of players
         * Parameter: playerUserName (string)
         * Result: none
         */
        public void AddNewPlayerToServer(string playerUserName)
        {
            //Create new lobby
            Player newPlayer = new Player(playerUserName);

            //Add new lobby
            _players.Add(newPlayer);
        }

        /* Method: RemovePlayer
         * Description: remove a player from the list of players in the server
         * Parameter: playerUserName (string)
         * Result: none
         */
        public void RemovePlayerFromServer(Player playerToRemove)
        {            
            _players.Remove(playerToRemove);                
        }

        /* Method: GetPlayer
         * Description: Return a player based on their username 
         * Parameter: playerUserName (string)
         * Result: Player
         */
        public Player GetPlayer(string playerUsername) 
        {
            foreach (Player p in _players)
            {
                if (p.Username.Equals(playerUsername))
                {
                    return p;
                }
            }
            Console.WriteLine($" Player with username ( {playerUsername} ) does not exist!!");
            return null;
        }

        public List<Player> GetPlayers() 
        {
            return _players;
        }

        public string GetPlayerUserByIndex(int index)
        {
            return _players[index].Username;
        }
    }
}
