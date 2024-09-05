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
    public class callbacks : PlayerCallback
    {
        private inLobbyPage _inLobbyPage;
        private MessageDatabase.Message message;

        public callbacks(inLobbyPage nInLobbyPage)
        {
            _inLobbyPage = nInLobbyPage;
        }

        public void UpdateLobbyPage(inLobbyPage lobbyPage)
        {
            _inLobbyPage = lobbyPage;
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
            throw new NotImplementedException();
        }

        
    }
}
