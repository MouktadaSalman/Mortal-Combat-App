using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/* 
 * Module: FileLinkBlock
 * Description: To hold all the info of a hyper-linked
 *              message (file-sharing)
 * Author: Jauhar
 * ID: 21494299
 * Version: 1.0.1.1
 */

namespace MortalCombatBusinessServer
{
    [DataContract]
    public class FileLinkBlock
    {
        /* Class fields:
         * Sender -> the sender of the file-sharing
         * FileName -> name of the file to be shared
         * Uri -> the URI link of hyper-link
         */
        [DataMember]
        public string Sender { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string Uri { get; set; }
    }
}
