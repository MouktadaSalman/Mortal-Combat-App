﻿using System;
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

        /* Constructor: MessageDatabase
         * Description: The private constructor of the database.
         */
        private MessageDatabase()
        {
            _messages = new List<Message>();
        }

        /* Method: SaveMessage
         * Description: just stores the message in the db.
         * Parameters: sender (string), recipient (string), content (string), messageType (int)
         */
        public void SaveMessage(string sender, string recipent, string content, int messageType)
        {
            Message newMessage = new Message(sender, recipent, content, messageType);
            _messages.Add(newMessage);
        }

        /* Method: SaveMessage
         * Description: just stores the message in the db.
         * Parameters: sender (string), recipient (string), content (FileLinkBlock), messageType (int)
         */
        public void SaveMessage(string sender, string recipent, FileLinkBlock content, int messageType)
        {
            Message newMessage = new Message(sender, recipent, content, messageType);
            _messages.Add(newMessage);
        }

        /* Method: GetMessagesForRecipient
         * Description: Retrieves all messages sent to a specific recipient.
         * Parameters: recipient (string)
         * Result: List of messages for the recipient.
         */
        public List<Message> GetMessagesForRecipient(string recipent)
        {
            List<Message> recipientMessages = new List<Message>();
            foreach (Message message in _messages)
            {
                if (message.Recipent.Equals(recipent))
                {
                    recipientMessages.Add(message);
                }
            }
            return recipientMessages;
        }


        public List<Message> GetPrivateMessagesForRecipient(string sender, string recipent)
        {
            List<Message> pRecipientMessages = new List<Message>();
            foreach (Message message in _messages)
            {
                if (message.Recipent.Equals(recipent) && message.Sender.Equals(sender))
                {
                    pRecipientMessages.Add(message);
                }
            }
            return pRecipientMessages;
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
        [DataContract]
        public class Message
        {
            [DataMember]
            public string Sender { get; set; }

            [DataMember]
            public string Recipent { get; set; }

            [DataMember]
            public string Content { get; set; }

            [DataMember]
            public FileLinkBlock ContentF { get; set; }

            [DataMember]
            public int MessageType { get; set; }  // maybe we could have this set 1 in default for normal messages,
                                             // however if it is a file we set it to 2, in the message chat
          

            /*
             * Constructor: Message
             * Description: Creates a new message instance.
             * Parameters: sender (string), recipient (string), content (string), messageType (int), timestamp (DateTime)
             */
            public Message(string sender, string recipient, string content, int messageType)
            {
                Sender = sender;
                Recipent = recipient;
                Content = content;
                MessageType = messageType;
                
            }

            /*
             * Constructor: Message
             * Description: Creates a new message instance.
             * Parameters: sender (string), recipient (string), content (FileLinkBlock), messageType (int), timestamp (DateTime)
             */
            public Message(string sender, string recipient, FileLinkBlock content, int messageType)
            {
                Sender = sender;
                Recipent = recipient;
                ContentF = content;
                MessageType = messageType;

            }


            // IDK just maybe we will use it for debugging later on similar to the one in message.cs
            public override string ToString()
            {
                return $"{Sender}: {Content}";
            }
        }

        /* 
         * Inner Class: FileLinkBlock
         * Description: To hold all the info of a hyper-linked
         *              message (file-sharing)
         * Author: Jauhar
         * ID: 21494299
         * Version: 1.0.1.1
         */
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
}