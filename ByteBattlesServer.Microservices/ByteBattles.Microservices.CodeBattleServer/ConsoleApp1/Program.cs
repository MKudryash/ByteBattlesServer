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
                case "connected":
                    if (json.RootElement.TryGetProperty("playerId", out var playerIdElement))
                        _playerId = playerIdElement.GetString();
                    Console.WriteLine($"ID игрока: {_playerId}");
                    Console.WriteLine($"Сообщение: {GetStringProperty(json.RootElement, "message")}");
                    break;
                    
                case "room_created":
                    Console.WriteLine($"Комната создана: {GetStringProperty(json.RootElement, "roomName")}");
                    Console.WriteLine($"ID комнаты: {GetStringProperty(json.RootElement, "roomId")}");
                    Console.WriteLine($"Сложность: {GetStringProperty(json.RootElement, "difficulty")}");
                    Console.WriteLine($"Сообщение: {GetStringProperty(json.RootElement, "message")}");
                    break;
                    
                case "joined_room":
                    Console.WriteLine($"Присоединились к комнате: {GetStringProperty(json.RootElement, "roomId")}");
                    Console.WriteLine($"Сообщение: {GetStringProperty(json.RootElement, "message")}");
                    if (json.RootElement.TryGetProperty("participants", out var participantsElement) && 
                        participantsElement.ValueKind == JsonValueKind.Number)
                    {
                        Console.WriteLine($"Участников в комнате: {participantsElement.GetInt32()}");
                    }
                    break;
                    
                case "player_joined":
                    Console.WriteLine($"Новый игрок присоединился: {GetStringProperty(json.RootElement, "playerId")}");
                    if (json.RootElement.TryGetProperty("participants", out var joinedParticipants) && 
                        joinedParticipants.ValueKind == JsonValueKind.Number)
                    {
                        Console.WriteLine($"Теперь участников: {joinedParticipants.GetInt32()}");
                    }
                    break;
                    
                case "player_left":
                    Console.WriteLine($"Игрок вышел: {GetStringProperty(json.RootElement, "playerId")}");
                    if (json.RootElement.TryGetProperty("participants", out var leftParticipants) && 
                        leftParticipants.ValueKind == JsonValueKind.Number)
                    {
                        Console.WriteLine($"Осталось участников: {leftParticipants.GetInt32()}");
                    }
                    break;
                    
                case "code_submitted":
                    Console.WriteLine($"Код отправлен на проверку");
                    Console.WriteLine($"Задача: {GetStringProperty(json.RootElement, "problemId")}");
                    break;
                    
                case "code_submitted_by_player":
                    Console.WriteLine($"Игрок {GetStringProperty(json.RootElement, "playerId")} отправил код");
                    Console.WriteLine($"Задача: {GetStringProperty(json.RootElement, "problemId")}");
                    break;
                    
                case "code_result":
                    Console.WriteLine($"Результат проверки кода:");
                    if (json.RootElement.TryGetProperty("result", out var resultElement))
                    {
                        Console.WriteLine($"  Статус: {GetStringProperty(resultElement, "status")}");
                        Console.WriteLine($"  Пройдено тестов: {GetIntProperty(resultElement, "passedTests")}/{GetIntProperty(resultElement, "totalTests")}");
                        Console.WriteLine($"  Время выполнения: {GetIntProperty(resultElement, "executionTime")}мс");
                    }
                    break;
                    
                case "error":
                    Console.WriteLine($"ОШИБКА: {GetStringProperty(json.RootElement, "message")}");
                    break;
                    
                default:
                    Console.WriteLine($"Неизвестный тип сообщения: {jsonMessage}");
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

    // Вспомогательные методы для безопасного получения свойств
    private static string GetStringProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property) && 
            property.ValueKind == JsonValueKind.String)
        {
            return property.GetString();
        }
        return "N/A";
    }

    private static int GetIntProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property) && 
            property.ValueKind == JsonValueKind.Number)
        {
            return property.GetInt32();
        }
        return 0;
    }

    static async Task CreateRoom()
    {
        Console.Write("Введите название комнаты: ");
        var roomName = Console.ReadLine();

        Console.Write("Введите ID языка (например, 019ac01b-7977-731a-82e6-cfc2ab28e762): ");
        var languageIdInput = Console.ReadLine();
        
        if (!Guid.TryParse(languageIdInput, out var languageId))
        {
            Console.WriteLine("ОШИБКА: Неверный формат GUID для languageId");
            return;
        }

        Console.Write("Введите сложность задачи (Easy/Medium/Hard): ");
        var difficulty = Console.ReadLine();

        if (string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(difficulty))
        {
            Console.WriteLine("ОШИБКА: Название комнаты и сложность не могут быть пустыми");
            return;
        }

        var message = new
        {
            type = "CreateRoom",
            roomName = roomName,
            languageId = languageId,
            difficulty = difficulty
        };

        await SendMessage(message);
    }

    static async Task JoinRoom()
    {
        Console.Write("Введите ID комнаты: ");
        var roomIdInput = Console.ReadLine();

        if (!Guid.TryParse(roomIdInput, out var roomId))
        {
            Console.WriteLine("ОШИБКА: Неверный формат GUID для roomId");
            return;
        }

        var message = new
        {
            type = "JoinRoom",
            roomId = roomId
        };

        await SendMessage(message);
    }

    static async Task SubmitCode()
    {
        Console.Write("Введите ID комнаты: ");
        var roomIdInput = Console.ReadLine();

        if (!Guid.TryParse(roomIdInput, out var roomId))
        {
            Console.WriteLine("ОШИБКА: Неверный формат GUID для roomId");
            return;
        }

        Console.Write("Введите ID задачи: ");
        var problemId = Console.ReadLine();

        Console.Write("Введите ваш код: ");
        var code = Console.ReadLine();

        if (string.IsNullOrEmpty(problemId) || string.IsNullOrEmpty(code))
        {
            Console.WriteLine("ОШИБКА: ID задачи и код не могут быть пустыми");
            return;
        }

        var message = new
        {
            type = "SubmitCode",
            roomId = roomId,
            problemId = problemId,
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