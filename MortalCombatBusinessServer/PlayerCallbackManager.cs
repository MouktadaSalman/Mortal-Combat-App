using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MortalCombatBusinessServer
{
    public class PlayerCallbackManager
    {
        // Singleton instance
        private static readonly Lazy<PlayerCallbackManager> instance = new Lazy<PlayerCallbackManager>(() => new PlayerCallbackManager());

        
        private readonly ConcurrentDictionary<string, PlayerCallback> allPlayerCallbacks;

      
        private PlayerCallbackManager()
        {
            allPlayerCallbacks = new ConcurrentDictionary<string, PlayerCallback>();
        }

       
        public static PlayerCallbackManager Instance => instance.Value;

        
        public void AddPlayerCallback(string username, PlayerCallback callback)
        {
            allPlayerCallbacks.AddOrUpdate(username, callback, (key, oldCallback) => callback);
            Console.WriteLine($"Player {username} added/updated in the callback manager.");
        }

        
        public PlayerCallback GetPlayerCallback(string username)
        {
            allPlayerCallbacks.TryGetValue(username, out var callback);
            return callback;
        }

     
        public void ListAllPlayersInCallbacks()
        {
            if (allPlayerCallbacks.Count > 0)
            {
                Console.WriteLine("All players in allPlayerCallback:");
                foreach (var player in allPlayerCallbacks)
                {
                    Console.WriteLine($"Player Username: {player.Key}, Callback: {player.Value}");
                }
            }
            else
            {
                Console.WriteLine("No players in allPlayerCallback.");
            }
        }
    }

}
