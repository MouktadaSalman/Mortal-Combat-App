using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortal_Combat_Data_Library
{
    public class PlayerDatabase
    {
        private readonly List<Player> _players;
        public static PlayerDatabase Instance { get; } = new PlayerDatabase();

        static PlayerDatabase() { }

        public PlayerDatabase()
        {
            _players = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            _players.Remove(player);
        }

        public string GetPlayerUserByIndex(int index)
        {
            return _players[index].Username;
        }

        //this is to test the branch

    }
}
