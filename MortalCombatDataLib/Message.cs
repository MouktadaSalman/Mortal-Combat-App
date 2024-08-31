using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mortal_Combat_Data_Library
{
    public class Message
    {


        /*
         * Class Fields:
         * sender (string) : will be assigned to whoever calls this field as who the message is going from.
         * reciever (string) : will be decided and choosen by the sender to who the message is sent.
         * content (object) : the message itself.
         * messageType(int) : will be determined, if it is a text or a file.
         * timeOfMessage(DateTime): time the message was sent. 
         * 
         * 
         */
        public string sender { get; set; }
        public string recipent { get; set; }

        //object for now as var did not work
        public object content { get; set; }
        private int messageType { get; set; }
        public DateTime timeOfMessage { get; set; }


        /*
         * Default Constructor:
         * Purpose: Creates a new instance of the Message class with default values.
         * sender, reciever, content, messageType, timeOfMessage.
         */
        public Message()
        {
            sender = "";
            recipent = "";
            content = null;
            messageType = 0;
            timeOfMessage = DateTime.Now;
        }

        /*
         * Constructor with parameters:
         * Purpose: Creates a new instance of the Message class with choosen values whenever gets called.
         * Parameters: string sender, string recipent, object content, int messageType, DateTime timeOfMessage.
         * 
         * Note: timeOfMessage is set to the current time using DateTime.Now, as it points to the time when 
         * the constructor is called (message is sent / to be send).
         * 
         * 
         */

        public Message(string sender, string recipent, object content, int messageType, DateTime timeOfMessage)
        {

            this.sender = sender;
            this.recipent = recipent;
            this.content = content;
            this.messageType = messageType;
            timeOfMessage = DateTime.Now;
            this.timeOfMessage = timeOfMessage;
        }


        /*
         * Normal overall Message getter:
         * To return a string with the entire details of the message.
         * 
         * Will we need to use something like this later on for debugging or logging or if a player wants to view a message?
         * 
         * Note: MessageType shouldn't be displayed that is for us to know the type of the message + we might remove that field.
         */
        public string GetMessageDetails()
        {
            return $"Sender: {sender} Recipent: {recipent} Content:  {content} Time of the message:  {timeOfMessage}";
        }


    }

}
