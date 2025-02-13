namespace Alias.Models;

public class GameState
{
    public List<Team> Teams { get; set; } = new List<Team>();
    public List<Player> UnassignedPlayers { get; set; } = new List<Player>();
    public string CurrentWord { get; set; }
    public Player CurrentPlayer { get; set; }
    public Team CurrentTeam { get; set; }
    public int TimeLeft { get; set; }
    public bool IsGameStarted { get; set; }
}