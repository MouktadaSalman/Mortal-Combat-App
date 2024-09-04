using Mortal_Combat_Data_Library;
using MortalCombatBusinessServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatClient
{
    public class callbacks : PlayerCallback
    {
        private inLobbyPage _inLobbyPage;


        public callbacks(inLobbyPage nInLobbyPage)
        {
            _inLobbyPage = nInLobbyPage;
        }
        public void ReceiveLobbyMessage(MessageDatabase.Message message)
        {
            _inLobbyPage.Dispatcher.Invoke(() =>
            {
                _inLobbyPage.showMessage(message.ToString());
            });
        }

        public void ReceivePrivateMessage(MessageDatabase.Message message)
        {
            throw new NotImplementedException();
        }
    }
}
