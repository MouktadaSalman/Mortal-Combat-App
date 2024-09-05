using Mortal_Combat_Data_Library;
using MortalCombatBusinessServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MortalCombatClient
{

    //this is the implemntation for the rest of methods in BusinessInterface, under PlayerCallBack interface
    public class callbacks : PlayerCallback
    {
        private inLobbyPage _inLobbyPage;
        private MessageDatabase.Message message;
        private privateMessagePage _privateMessagePage;
        public callbacks(inLobbyPage nInLobbyPage, privateMessagePage privateMessagePage)
        {
            _inLobbyPage = nInLobbyPage;
            _privateMessagePage = privateMessagePage;
        }

        public void UpdateLobbyPage(inLobbyPage lobbyPage)
        {
            _inLobbyPage = lobbyPage;

        }

        public void UpdatePrivatePage(privateMessagePage privateMessagePage) 
        {

            _privateMessagePage = privateMessagePage;
        }
            

        public void ReceiveLobbyMessage(string sender, string lobbyName, string content)
        {
            message.Sender = sender;
            message.Recipent = lobbyName;
            message.Content = content;

            _inLobbyPage.Dispatcher.Invoke(() =>
            {
                _inLobbyPage.showMessage(message.ToString());
            });
        }

        public void ReceivePrivateMessage(string sender, string lobbyName, string content)
        {
            message.Sender = sender;
            message.Recipent = lobbyName;
            message.Content = content;

            _privateMessagePage.Dispatcher.Invoke(() =>
            {
                _privateMessagePage.showMessage(message.ToString());
            });
        }

        
    }
}
