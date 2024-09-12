using System;
using System.Collections.Generic;
using System.Linq;
/* 
 * Module: LobbyNameAlreadyExistsFault
 * Description: Exception for when a lobby name already exists, preventing player from making a new lobby with the same name
 * Author:  Moukhtada
 * ID: 20640266
 * Version: 1.0.0.2
 */
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
