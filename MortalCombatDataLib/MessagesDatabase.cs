using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Mortal_Combat_Data_Library

{
    [DataContract]
    public class MessageDatabase
    {
        /*
         * Class fields:
         * _messages: contains all the messages
         * Instance: allows a single instance of the message database
         */
        [DataMember]
        private readonly List<Message> _messages;

        
        public static readonly MessageDatabase Instance = new MessageDatabase();

        /*
         * Constructor: MessageDatabase
         * Description: The private constructor of the database.
         * 
         */
        private MessageDatabase()
        {
            _messages = new List<Message>();
        }




        /*
         * Method: SaveMessage
         * Description: just stores the message in the db.
         * Parameters: sender (string), recipient (string), content (string), messageType (int)
         * 
         */
        public void SaveMessage(string sender, string recipient, string content, int messageType)
        {
            Message newMessage = new Message(sender, recipient, content, messageType, DateTime.Now);
            _messages.Add(newMessage);
        }

        /*
         * Method: GetMessagesForRecipient
         * Description: Retrieves all messages sent to a specific recipient.
         * Parameters: recipient (string)
         * Result: List of messages for the recipient.
         */
        public List<Message> GetMessagesForRecipient(string recipient)
        {
            List<Message> recipientMessages = new List<Message>();
            foreach (Message message in _messages)
            {
                if (message.Recipient == recipient)
                {
                    recipientMessages.Add(message);
                }
            }
            return recipientMessages;
        }

        /*
         * Method: GetAllMessages
         * Description: Retrieves all messages in the database.
         * Parameters: none
         * Result: List of all messages.
         */
        public List<Message> GetAllMessages()
        {
            return new List<Message>(_messages);
        }

        /*
         * Inner Class: Message
         * Description: Represents a message entity in the database.
         */
        public class Message
        {
            public string Sender { get; }
            public string Recipient { get; }
            public string Content { get; }
            public int MessageType { get; }  // maybe we could have this set 1 in default for normal messages,
                                             // however if it is a file we set it to 2, in the message chat
            public DateTime Timestamp { get; }

            /*
             * Constructor: Message
             * Description: Creates a new message instance.
             * Parameters: sender (string), recipient (string), content (string), messageType (int), timestamp (DateTime)
             */
            public Message(string sender, string recipient, string content, int messageType, DateTime timestamp)
            {
                Sender = sender;
                Recipient = recipient;
                Content = content;
                MessageType = messageType;
                Timestamp = timestamp;
            }


            // IDK just maybe we will use it for debugging later on similar to the one in message.cs
            public override string ToString()
            {
                return $"Message{{sender='{Sender}', recipient='{Recipient}', content='{Content}', messageType={MessageType}, timestamp={Timestamp}}}";
            }
        }
    }
}