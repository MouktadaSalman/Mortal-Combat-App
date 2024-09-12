/* 
 * Module: PlayersStilInLobbyFault
 * Description: Exception for when players are still in the lobby, preventing it from being deleted
 * Author:  Moukhtada
 * ID: 20640266
 * Version: 1.0.0.2
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MortalCombatBusinessServer
{
    [DataContract]
    public class PlayersStilInLobbyFault
    {
        [DataMember]
        public string Issue { get; set; }
    }
}
