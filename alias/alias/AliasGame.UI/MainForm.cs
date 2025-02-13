using AliasGame.Client.Network;
using AliasGame.UI.Models;
using Newtonsoft.Json;

namespace AliasGame.UI;

public partial class MainForm : Form
{
    private GameClient _gameClient;
    private string _playerId;
    private GameState _currentState;
    private bool _isClosing = false;

    public MainForm()
    {
        InitializeComponent();
        _playerId = Guid.NewGuid().ToString();
        SetupEventHandlers();
        
        this.FormClosing += MainForm_FormClosing;
    }

    private void SetupEventHandlers()
    {
        btnJoin.Click += (s, e) => ConnectToServer();
        btnStart.Click += (s, e) => _gameClient.SendMessage(GameCommands.StartGame);
        btnCorrect.Click += (s, e) => SendMove(true);
        btnIncorrect.Click += (s, e) => SendMove(false);
        btnRandomTeams.Click += (s, e) => _gameClient.SendMessage(GameCommands.RandomizeTeams);
        // btnJoinTeam.Click += (s, e) => JoinTeam();
        // Добавьте обработчик для завершения игры, если это необходимо
        // btnEndGame.Click += (s, e) => _gameClient.SendMessage(GameCommands.EndGame);
    }

    private void ConnectToServer()
    {
        if (string.IsNullOrWhiteSpace(txtNickname.Text))
        {
            MessageBox.Show("Пожалуйста, введите имя игрока.", 
                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _gameClient = new GameClient();
            _gameClient.OnMessageReceived += HandleServerMessage;
            _gameClient.Connect("127.0.0.1", 12345);
            _gameClient.SendMessage($"{GameCommands.PlayerJoin}:{_playerId}:{txtNickname.Text.Trim()}");

            EnableJoinControls(false);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка подключения к серверу: {ex.Message}", 
                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            EnableJoinControls(true);
        }
    }

    private void SendMove(bool isCorrect)
    {
        _gameClient.SendMessage($"{GameCommands.PlayerMove}:{_playerId}:{isCorrect}");
    }

    private void HandleServerMessage(string message)
    {
        if (InvokeRequired)
        {
            Invoke(new Action<string>(HandleServerMessage), message);
            return;
        }

        try
        {
            var parts = message.Split(new[] { ':' }, 2);
            
            if (parts.Length != 2) return;

            switch (parts[0])
            {
                case GameCommands.JoinResponse:
                    HandleJoinResponse(parts[1]);
                    break;
                
                case GameCommands.GameState:
                    try
                    {
                        _currentState = JsonConvert.DeserializeObject<GameState>(parts[1]);
                        UpdateUI();
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Ошибка десериализации: {ex.Message}");
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки сообщения: {ex.Message}");
        }
    }

    private void HandleJoinResponse(string response)
    {
        switch (response)
        {
            case "success":
                EnableJoinControls(false);
                break;
            
            case "nickname_taken":
                MessageBox.Show("Игрок с таким именем уже присутствует в игре. Пожалуйста, выберите другое имя.",
                    "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                EnableJoinControls(true);
                txtNickname.SelectAll();
                txtNickname.Focus();
                break;
        }
    }

    private void EnableJoinControls(bool enabled)
    {
        if (InvokeRequired)
        {
            Invoke(new Action<bool>(EnableJoinControls), enabled);
            return;
        }

        txtNickname.Enabled = enabled;
        btnJoin.Enabled = enabled;
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!_isClosing && _gameClient?.IsConnected == true)
        {
            _isClosing = true;
            e.Cancel = true;

            try
            {
                _gameClient.SendMessage($"{GameCommands.PlayerLeave}:{_playerId}");
                
                Task.Run(async () =>
                {
                    await Task.Delay(300);
                    
                    this.Invoke(() =>
                    {
                        _gameClient?.Disconnect();
                        this.FormClosing -= MainForm_FormClosing;
                        this.Close();
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке сообщения о выходе: {ex.Message}");
                this.Close();
            }
        }
    }

    private void UpdateUI()
    {
        if (InvokeRequired)
        {
            Invoke(new Action(UpdateUI));
            return;
        }

        bool isCurrentPlayer = _currentState.CurrentPlayer?.Id == _playerId;
        
        lblCurrentWord.Text = isCurrentPlayer 
            ? _currentState.CurrentWord 
            : $"Объясняет: {_currentState.CurrentPlayer?.Nickname ?? ""}";
        
        btnCorrect.Enabled = isCurrentPlayer;
        btnIncorrect.Enabled = isCurrentPlayer;
        
        TimeSpan time = TimeSpan.FromSeconds(_currentState.TimeLeft);
        lblTimer.Text = $"{time.Minutes:D2}:{time.Seconds:D2}";
        
        lstPlayers.Items.Clear();
        foreach (var team in _currentState.Teams)
        {
            lstPlayers.Items.Add($"=== {team.Name} (Очки: {team.Score}) ===");
            foreach (var player in team.Players)
            {
                lstPlayers.Items.Add($"  {player.Nickname}");
            }
        }

        /*if (!_currentState.IsGameStarted)
        {
            cmbTeams.Items.Clear();
            foreach (var team in _currentState.Teams)
            {
                if (team.Players.Count < 2)
                {
                    cmbTeams.Items.Add(team.Name);
                }
            }
        }*/
        
        btnRandomTeams.Enabled = !_currentState.IsGameStarted;
        // btnJoinTeam.Enabled = !_currentState.IsGameStarted;
        // cmbTeams.Enabled = !_currentState.IsGameStarted;
        btnStart.Enabled = !_currentState.IsGameStarted && _currentState.Teams.Count >= 2;
    }

    /*private void JoinTeam()
    {
        if (cmbTeams.SelectedItem != null)
        {
            string teamName = cmbTeams.SelectedItem.ToString();
            _gameClient.SendMessage($"{GameCommands.JoinTeam}:{_playerId}:{teamName}");
        }
    }*/
}