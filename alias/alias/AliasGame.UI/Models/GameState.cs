using Alias.Models;
using Player = AliasGame.Client.Models.Player;

namespace AliasGame.UI.Models
{
    public class GameState
    {
        // Убрано свойство Players, так как игроки находятся внутри команд
        public string CurrentWord { get; set; }
        public Player CurrentPlayer { get; set; }
        public int TimeLeft { get; set; }
        public bool IsGameStarted { get; set; }
        public List<Team> Teams { get; set; } = new List<Team>();
        public Team CurrentTeam { get; set; }
    }
}