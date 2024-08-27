using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortal_Combat_Data_Library
{
    public class Lobby
    {
        public string LobbyName { get; set; }

        public List<Player> _players;

        public List<Message> messeges;

        public Lobby(string lobbyName)
        {
            this.LobbyName = lobbyName;
            _players = new List<Player>();
            messeges = new List<Message>();
        }
    }
}
