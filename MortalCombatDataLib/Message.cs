using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortal_Combat_Data_Library
{
    public class Message
    {
        private string sender { get; set; }
        private string recipent { get; set; }

        //object for now as var did not work
        private object content { get; set; }
        private int messageType { get; set; }
        private DateTime timeOfMessage { get; set; }

        public Message()
        {
            sender = "";
            recipent = "";
            content = null;
            messageType = 0;
            timeOfMessage = DateTime.Now;
        }

        public Message(string sender, string recipent, object content, int messageType, DateTime timeOfMessage)
        {

            this.sender = sender;
            this.recipent = recipent;
            this.content = content;
            this.messageType = messageType;
            timeOfMessage = DateTime.Now;
            this.timeOfMessage = timeOfMessage;
        }

    }
}
