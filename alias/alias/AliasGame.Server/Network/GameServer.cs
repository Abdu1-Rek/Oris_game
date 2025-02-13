using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Alias.GameLogic;
using Alias.Models;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Alias.Network;

public class GameServer
{
    private TcpListener _listener;
    private GameManager _gameManager;
    private List<TcpClient> _clients = new List<TcpClient>();
    private Dictionary<string, TcpClient> _clientsMap = new Dictionary<string, TcpClient>();
    
    public GameServer(string ip, int port, List<string> words)
    {
        _listener = new TcpListener(IPAddress.Parse(ip), port);
        _gameManager = new GameManager(words);
        _gameManager.OnGameStateUpdated += OnGameStateUpdated;
    }


    public void Start()
    {
        _listener.Start();

        Console.WriteLine("Server started");


        new Thread(() =>
        {
            while (true)
            {
                var client = _listener.AcceptTcpClient();
                _clients.Add(client);
                new Thread(() => HandleClient(client)).Start();
            }
        }).Start();
    }


    private void HandleClient(TcpClient client)
    {
        var stream = client.GetStream();
        byte[] buffer = new byte[1024];
        string disconnectedPlayerId = null;
        
        try
        {
            while (client.Connected)
            {
                int bytesRead;

                try
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0) break;
                }
                catch
                {
                    break;
                }
                
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Сохраняем ID игрока при получении команды PlayerLeave
                if (message.StartsWith(GameCommands.PlayerLeave))

                {
                    disconnectedPlayerId = message.Split(':')[1];
                }

                ProcessClientMessage(message);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке клиента: {ex.Message}");
        }

        finally
        {
            // Если ID не был получен через PlayerLeave, ищем его в маппинге
            if (disconnectedPlayerId == null)
            {
                disconnectedPlayerId = _clientsMap.FirstOrDefault(x => x.Value == client).Key;
            }
            
            if (disconnectedPlayerId != null)
            {
                _clientsMap.Remove(disconnectedPlayerId);

                _gameManager.RemovePlayer(disconnectedPlayerId);
            }
            
            _clients.Remove(client);

            try
            {
                client.Close();
            }
            catch
            {
            }
        }
    }


    private void ProcessClientMessage(string message)
    {
        var parts = message.Split(':');
        string command = parts[0];
        
        switch (command)
        {
            case GameCommands.PlayerJoin:
                var player = new Player { Id = parts[1], Nickname = parts[2] };
                if (_gameManager.AddPlayer(player))
                {
                    _clientsMap[player.Id] = _clients[_clients.Count - 1];
                    SendToClient(_clientsMap[player.Id], $"{GameCommands.JoinResponse}:success");
                }
                else
                {
                    SendToClient(_clients[_clients.Count - 1], $"{GameCommands.JoinResponse}:nickname_taken");
                    _clients.RemoveAt(_clients.Count - 1);
                }
                break;

            case GameCommands.RandomizeTeams:
                _gameManager.AssignTeams(true);
                break;

            case GameCommands.JoinTeam:
                if (parts.Length >= 3)
                {
                    _gameManager.JoinTeam(parts[1], parts[2]);
                }
                break;

            case GameCommands.PlayerMove:
                if (parts.Length >= 3)
                {
                    _gameManager.ProcessPlayerMove(parts[1], bool.Parse(parts[2]));
                }
                break;
            
            case GameCommands.StartGame:
                _gameManager.StartGame();
                break;
            
            case GameCommands.PlayerLeave:
                if (parts.Length >= 2)
                {
                    string playerId = parts[1];
                    if (_clientsMap.ContainsKey(playerId))
                    {
                        var clientToRemove = _clientsMap[playerId];
                        _clients.Remove(clientToRemove);
                        _clientsMap.Remove(playerId);
                        _gameManager.RemovePlayer(playerId);
                        try { clientToRemove.Close(); } catch { }
                    }
                }
                break;

            case GameCommands.EndGame:
                _gameManager.EndGame();
                break;
        }

        // Отправляем обновлённое состояние игры всем клиентам
        BroadcastGameState(_gameManager.GetGameState());
    }

    private void SendToClient(TcpClient client, string message)
    {
        try
        {
            var data = Encoding.UTF8.GetBytes(message);
            client.GetStream().Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке данных клиенту: {ex.Message}");
        }
    }

    private void OnGameStateUpdated(GameState gameState)
    {
        BroadcastGameState(gameState);
    }

    private void BroadcastGameState(GameState gameState)
    {
        var connectedClients = _clients.Where(c => c.Connected).ToList();
        var serializedState = JsonConvert.SerializeObject(gameState);
        var message = $"{GameCommands.GameState}:{serializedState}";

        foreach (var client in connectedClients)
        {
            try
            {
                SendToClient(client, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке состояния игры: {ex.Message}");
            }
        }
    }
}