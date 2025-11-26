using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static ClientWebSocket _webSocket = new ClientWebSocket();
    private static string _playerId;

    static async Task Main(string[] args)
    {
        Console.WriteLine("Подключение к серверу битв...");
        
        // Исправленный URL - используйте /battlehub вместо /battle
        await _webSocket.ConnectAsync(new Uri("ws://localhost:5065/api/battle"), CancellationToken.None);
        Console.WriteLine("Подключено!");

        // Запускаем задачу для получения сообщений
        var receiveTask = ReceiveMessages();

        // Основной цикл взаимодействия
        while (_webSocket.State == WebSocketState.Open)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1 - Создать комнату");
            Console.WriteLine("2 - Присоединиться к комнате");
            Console.WriteLine("3 - Отправить код");
            Console.WriteLine("4 - Выйти");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await CreateRoom();
                    break;
                case "2":
                    await JoinRoom();
                    break;
                case "3":
                    await SubmitCode();
                    break;
                case "4":
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Выход", CancellationToken.None);
                    break;
                default:
                    Console.WriteLine("Неизвестная команда");
                    break;
            }
        }

        await receiveTask;
    }

    static async Task ReceiveMessages()
    {
        var buffer = new byte[1024 * 4];
        
        while (_webSocket.State == WebSocketState.Open)
        {
            try
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    ProcessMessage(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("Соединение закрыто сервером");
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении сообщения: {ex.Message}");
                break;
            }
        }
    }

    static void ProcessMessage(string jsonMessage)
    {
        try
        {
            using var json = JsonDocument.Parse(jsonMessage);
            var typeProperty = json.RootElement.TryGetProperty("type", out var typeElement) 
                ? typeElement.GetString() 
                : "unknown";

            Console.WriteLine($"\n[СЕРВЕР] {DateTime.Now:T}");
            Console.WriteLine($"Тип: {typeProperty}");

            switch (typeProperty)
            {
                case "Connected":
                    if (json.RootElement.TryGetProperty("playerId", out var playerIdElement))
                        _playerId = playerIdElement.GetString();
                    Console.WriteLine($"ID игрока: {_playerId}");
                    Console.WriteLine($"Сообщение: {json.RootElement.GetProperty("message").GetString()}");
                    break;
                case "room_created":
                    Console.WriteLine($"Комната создана: {json.RootElement.GetProperty("roomName").GetString()}");
                    Console.WriteLine($"ID комнаты: {json.RootElement.GetProperty("roomId").GetString()}");
                    break;
                case "joined_room":
                    Console.WriteLine($"Присоединились к комнате: {json.RootElement.GetProperty("roomName").GetString()}");
                    if (json.RootElement.TryGetProperty("players", out var playersElement))
                    {
                        Console.WriteLine("Игроки в комнате:");
                        foreach (var player in playersElement.EnumerateArray())
                        {
                            Console.WriteLine($"  - {player}");
                        }
                    }
                    break;
                case "player_joined":
                    Console.WriteLine($"Новый игрок: {json.RootElement.GetProperty("playerId").GetString()}");
                    break;
                case "code_result":
                    Console.WriteLine($"Результат проверки: {json.RootElement.GetProperty("result").GetString()}");
                    break;
                case "error":
                    Console.WriteLine($"Ошибка: {json.RootElement.GetProperty("message").GetString()}");
                    break;
                default:
                    Console.WriteLine($"Необработанное сообщение: {jsonMessage}");
                    break;
            }
            Console.WriteLine("---");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки сообщения: {ex.Message}");
            Console.WriteLine($"Полученное сообщение: {jsonMessage}");
        }
    }

    static async Task CreateRoom()
    {
        Console.Write("Введите название комнаты: ");
        var roomName = Console.ReadLine();

        // Для тестирования - используем фиксированный languageId
        var languageId = Guid.NewGuid();

        var message = new
        {
            type = "CreateRoom", // Имя метода в Hub
            roomName,
            languageId
        };

        await SendMessage(message);
    }

    static async Task JoinRoom()
    {
        Console.Write("Введите ID комнаты: ");
        var roomId = Console.ReadLine();

        var message = new
        {
            type = "JoinRoom", // Имя метода в Hub
            roomId = Guid.Parse(roomId)
        };

        await SendMessage(message);
    }

    static async Task SubmitCode()
    {
        Console.Write("Введите ID комнаты: ");
        var roomId = Console.ReadLine();

        Console.Write("Введите название задачи: ");
        var problem = Console.ReadLine();

        Console.Write("Введите ваш код: ");
        var code = Console.ReadLine();

        var message = new
        {
            type = "SubmitCode", // Имя метода в Hub
            roomId = Guid.Parse(roomId),
            problemId = problem,
            code = code
        };

        await SendMessage(message);
    }

    static async Task SendMessage(object message)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), 
                WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine($"Отправлено: {json}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
        }
    }
}