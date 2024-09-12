using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatBusinessServer
{
    [DataContract]
    public class LobbyNameAlreadyExistsFault
    {
        [DataMember]
        public string Issue { get; set; }
    }
}
