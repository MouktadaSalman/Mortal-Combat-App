/* 
 * Module: PlayerNameAlreadyEsistsFault
 * Description: Exception for when a player name already exists, preventing player from taking the name
 * Author:  Moukhtada
 * ID: 20640266
 * Version: 1.0.0.2
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatBusinessServer
{
    [DataContract]
    public class PlayerNameAlreadyEsistsFault
    {
        [DataMember]
        public string Issue { get; set; }
    }
}
