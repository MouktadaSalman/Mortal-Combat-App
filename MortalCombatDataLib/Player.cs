using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortal_Combat_Data_Library
{
    public class Player
    {
        public string Username { get; set; }
        public bool isInLobby{ get; set; } // do we actually need this? we can just have the list of player in each lobby through the lobby class
        public DateTime timeJoinedToLobby { get; set; }

        public Player(string username)
        {
            this.Username = username;
            isInLobby = false;
            timeJoinedToLobby = DateTime.Now;
        }

    }
}
