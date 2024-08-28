/* 
 * Module: Lobby
 * Description: Responsible for individual lobby 
 *              functionalities.
 * Author: Ahmed, Moukhtada, Jauhar
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
    public class Lobby
    {
        private string lobbyName { get; set; }

        private List<Player> _players;

        private List<Message> _messages;

        public Lobby(string lobbyName)
        {
            this.lobbyName = lobbyName;
            _players = new List<Player>();
            _messages = new List<Message>();
        }
    }
}
