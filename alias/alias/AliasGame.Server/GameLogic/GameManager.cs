using System.Timers;
using Alias.Models;

namespace Alias.GameLogic;

public class GameManager
{
    private GameState _gameState;
    private System.Timers.Timer _timer;
    private Random _random;
    private List<string> _words;
    private const int MAX_PLAYERS = 4;
    private const int TEAM_SIZE = 2;
    private const int ROUND_TIME = 180; // 3 минуты

    public event Action<GameState> OnGameStateUpdated;

    public GameManager(List<string> words)
    {
        _gameState = new GameState();
        _random = new Random();
        _words = words;

        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (s, e) =>
        {
            _gameState.TimeLeft--;
            if (_gameState.TimeLeft <= 0) SwitchToNextTeam();
            OnGameStateUpdated?.Invoke(_gameState);
        };
    }

    public bool AddPlayer(Player player)
    {
        if (IsNicknameTaken(player.Nickname) || 
            _gameState.UnassignedPlayers.Count + GetTotalPlayersInTeams() >= MAX_PLAYERS)
        {
            return false;
        }
    
        _gameState.UnassignedPlayers.Add(player);
        OnGameStateUpdated?.Invoke(_gameState);
        return true;
    }

    private int GetTotalPlayersInTeams()
    {
        return _gameState.Teams.Sum(t => t.Players.Count);
    }

    public void AssignTeams(bool randomize)
    {
        if (randomize)
        {
            var allPlayers = _gameState.UnassignedPlayers.ToList();
            _gameState.Teams.Clear();

            if (allPlayers.Count >= 2)
            {
                // Перемешиваем игроков
                allPlayers = allPlayers.OrderBy(x => _random.Next()).ToList();

                // Создаем команды
                for (int i = 0; i < allPlayers.Count; i += TEAM_SIZE)
                {
                    if (i + 1 < allPlayers.Count)
                    {
                        var team = new Team
                        {
                            Name = $"Команда {_gameState.Teams.Count + 1}",
                            Players = allPlayers.Skip(i).Take(TEAM_SIZE).ToList()
                        };
                        _gameState.Teams.Add(team);
                    }
                }
                
                _gameState.UnassignedPlayers.Clear();
            }
        }
        
        OnGameStateUpdated?.Invoke(_gameState);
    }

    public void StartGame()
    {
        if (_gameState.Teams.Count < 2) return;

        _gameState.IsGameStarted = true;
        _gameState.CurrentTeam = _gameState.Teams[0];
        _gameState.CurrentPlayer = _gameState.CurrentTeam.Players[0];
        _gameState.TimeLeft = ROUND_TIME;
        _gameState.CurrentWord = GetRandomWord();
        _timer.Start();
        OnGameStateUpdated?.Invoke(_gameState);
    }

    public void EndGame()
    {
        _timer.Stop();
        _gameState.IsGameStarted = false;
        _gameState.CurrentPlayer = null;
        _gameState.CurrentWord = null;
        _gameState.TimeLeft = 0;
        OnGameStateUpdated?.Invoke(_gameState);
    }

    public void ProcessPlayerMove(string playerId, bool isCorrect)
    {
        if (_gameState.CurrentPlayer?.Id != playerId) return;
        
        if (isCorrect)
        {
            _gameState.CurrentTeam.Score++;
        }
        
        _gameState.CurrentWord = GetRandomWord();
        OnGameStateUpdated?.Invoke(_gameState);
    }

    private void SwitchToNextTeam()
    {
        _timer.Stop();
        
        int currentIndex = _gameState.Teams.IndexOf(_gameState.CurrentTeam);
        int nextIndex = (currentIndex + 1) % _gameState.Teams.Count;
        
        _gameState.CurrentTeam = _gameState.Teams[nextIndex];
        _gameState.CurrentPlayer = _gameState.CurrentTeam.Players[0];
        _gameState.TimeLeft = ROUND_TIME;
        _gameState.CurrentWord = GetRandomWord();
        
        _timer.Start();
        OnGameStateUpdated?.Invoke(_gameState);
    }

    public void RemovePlayer(string playerId)
    {
        var playerToRemove = _gameState.UnassignedPlayers.FirstOrDefault(p => p.Id == playerId);
        if (playerToRemove != null)
        {
            _gameState.UnassignedPlayers.Remove(playerToRemove);
            OnGameStateUpdated?.Invoke(_gameState);
            return;
        }

        foreach (var team in _gameState.Teams)
        {
            playerToRemove = team.Players.FirstOrDefault(p => p.Id == playerId);
            if (playerToRemove != null)
            {
                team.Players.Remove(playerToRemove);
                if (team.Players.Count == 0)
                {
                    _gameState.Teams.Remove(team);
                }
                break;
            }
        }

        if (_gameState.CurrentPlayer?.Id == playerId)
        {
            SwitchToNextTeam();
        }

        OnGameStateUpdated?.Invoke(_gameState);
    }

    public bool IsNicknameTaken(string nickname)
    {
        return _gameState.UnassignedPlayers.Any(p => p.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase)) ||
               _gameState.Teams.Any(t => t.Players.Any(p => p.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase)));
    }

    private string GetRandomWord() => _words[_random.Next(_words.Count)];

    public void JoinTeam(string playerId, string teamName)
    {
        var player = _gameState.UnassignedPlayers.FirstOrDefault(p => p.Id == playerId);
        if (player == null) return;

        var team = _gameState.Teams.FirstOrDefault(t => t.Name == teamName);
        if (team == null)
        {
            team = new Team { Name = teamName };
            _gameState.Teams.Add(team);
        }

        if (team.Players.Count < TEAM_SIZE)
        {
            _gameState.UnassignedPlayers.Remove(player);
            team.Players.Add(player);
            OnGameStateUpdated?.Invoke(_gameState);
        }
    }

    public GameState GetGameState()
    {
        return _gameState;
    }
}