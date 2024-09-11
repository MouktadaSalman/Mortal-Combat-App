using Mortal_Combat_Data_Library;
using MortalCombatBusinessServer;
using MortalCombatClient;
using System.Collections.Generic;
using System.Windows;

namespace MortalCombatClient
{
    public class callbacks : PlayerCallback
    {
        private inLobbyPage _inLobbyPage;
        private Dictionary<string, privateMessagePage> _privateMessagePages;
        private MainWindow _mainWindow;

        public callbacks()
        {
            _mainWindow = (MainWindow)Application.Current.MainWindow;
            _privateMessagePages = new Dictionary<string, privateMessagePage>();
        }

        public void UpdateLobbyPage(inLobbyPage lobbyPage)
        {
            _inLobbyPage = lobbyPage;
        }

        public void UpdatePrivatePage(string chatPartner, privateMessagePage page)
        {
            if (page == null)
            {
                _privateMessagePages.Remove(chatPartner);
            }
            else
            {
                _privateMessagePages[chatPartner] = page;
            }
        }

        public void ReceiveLobbyMessage(string sender, string lobbyName, string content)
        {
            if (_inLobbyPage != null)
            {
                _inLobbyPage.Dispatcher.Invoke(() =>
                {
                    _inLobbyPage.showMessage($"{sender}: {content}");
                });
            }
        }

        public void ReceiveLobbyMessageF(string sender, string lobbyName, MessageDatabase.FileLinkBlock content)
        {
            if (_inLobbyPage != null)
            {
                _inLobbyPage.Dispatcher.Invoke(() =>
                {
                    _inLobbyPage.showLink(content);
                });
            }
        }

        public void ReceivePrivateMessage(string sender, string recipient, string content)
        {
            _mainWindow.Dispatcher.Invoke(() =>
            {
                _mainWindow.HandleIncomingPrivateMessage(sender, recipient, content);
            });
        }
    }
}