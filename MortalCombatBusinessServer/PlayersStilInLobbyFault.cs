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
