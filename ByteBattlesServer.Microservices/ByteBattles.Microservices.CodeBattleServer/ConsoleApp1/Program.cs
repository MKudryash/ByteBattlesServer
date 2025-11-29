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
    private static Guid _currentRoomId;
    private static bool _isReady = false;

    static async Task Main(string[] args)
    {
        Console.WriteLine("Подключение к серверу битв...");
        
        await _webSocket.ConnectAsync(new Uri("ws://localhost:50312/api/battle"), CancellationToken.None);
        Console.WriteLine("Подключено!");

        // Запускаем задачу для получения сообщений
        var receiveTask = ReceiveMessages();

        // Основной цикл взаимодействия
        while (_webSocket.State == WebSocketState.Open)
        {
            await ShowMainMenu();
        }

        await receiveTask;
    }

    static async Task ShowMainMenu()
    {
        Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
        Console.WriteLine("1 - Создать комнату");
        Console.WriteLine("2 - Присоединиться к комнате");
        Console.WriteLine("3 - Показать статус комнаты");
        Console.WriteLine("4 - Подтвердить готовность");
        Console.WriteLine("5 - Отправить код");
        Console.WriteLine("6 - Покинуть комнату");
        Console.WriteLine("7 - Выйти");

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
                await GetRoomStatus();
                break;
            case "4":
                await ToggleReady();
                break;
            case "5":
                await SubmitCode();
                break;
            case "6":
                await LeaveRoom();
                break;
            case "7":
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Выход", CancellationToken.None);
                break;
            default:
                Console.WriteLine("Неизвестная команда");
                break;
        }
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
                    Console.WriteLine($"✅ Комната создана: {GetStringProperty(json.RootElement, "roomName")}");
                    Console.WriteLine($"🆔 ID комнаты: {GetStringProperty(json.RootElement, "roomId")}");
                    Console.WriteLine($"📊 Сложность: {GetStringProperty(json.RootElement, "difficulty")}");
                    Console.WriteLine($"📝 Сообщение: {GetStringProperty(json.RootElement, "message")}");
                    
                    if (Guid.TryParse(GetStringProperty(json.RootElement, "roomId"), out var roomId))
                    {
                        _currentRoomId = roomId;
                    }
                    break;
                    
                case "joined_room":
                    Console.WriteLine($"✅ Присоединились к комнате: {GetStringProperty(json.RootElement, "roomName")}");
                    Console.WriteLine($"🆔 ID комнаты: {GetStringProperty(json.RootElement, "roomId")}");
                    Console.WriteLine($"📝 Сообщение: {GetStringProperty(json.RootElement, "message")}");
                    Console.WriteLine($"👥 Участников: {GetIntProperty(json.RootElement, "participants")}");
                    Console.WriteLine($"📊 Статус: {GetStringProperty(json.RootElement, "status")}");
                    Console.WriteLine($"🚀 Можно начать: {(GetBoolProperty(json.RootElement, "canStart") ? "ДА" : "НЕТ")}");
                    
                    if (Guid.TryParse(GetStringProperty(json.RootElement, "roomId"), out var joinedRoomId))
                    {
                        _currentRoomId = joinedRoomId;
                    }
                    break;
                    
                case "player_joined":
                    Console.WriteLine($"🎮 Новый игрок: {GetStringProperty(json.RootElement, "playerId")}");
                    Console.WriteLine($"👥 Теперь участников: {GetIntProperty(json.RootElement, "participants")}");
                    Console.WriteLine($"📊 Статус комнаты: {GetStringProperty(json.RootElement, "roomStatus")}");
                    break;
                    
                case "player_left":
                    Console.WriteLine($"👋 Игрок вышел: {GetStringProperty(json.RootElement, "playerId")}");
                    Console.WriteLine($"👥 Осталось участников: {GetIntProperty(json.RootElement, "participants")}");
                    break;
                    
                case "room_status":
                    Console.WriteLine($"📊 СТАТУС КОМНАТЫ:");
                    Console.WriteLine($"   🆔 ID: {GetStringProperty(json.RootElement, "roomId")}");
                    Console.WriteLine($"   📊 Статус: {GetStringProperty(json.RootElement, "status")}");
                    Console.WriteLine($"   👥 Участников: {GetIntProperty(json.RootElement, "participantCount")}");
                    Console.WriteLine($"   ✅ Готово: {GetIntProperty(json.RootElement, "readyCount")}");
                    Console.WriteLine($"   🚀 Можно начать: {(GetBoolProperty(json.RootElement, "canStart") ? "ДА" : "НЕТ")}");
                    Console.WriteLine($"   🔥 Активна: {(GetBoolProperty(json.RootElement, "isActive") ? "ДА" : "НЕТ")}");
                    break;
                    
                case "game_can_start":
                    Console.WriteLine($"🚀 КОМНАТА ГОТОВА К НАЧАЛУ!");
                    Console.WriteLine($"📝 {GetStringProperty(json.RootElement, "message")}");
                    Console.WriteLine($"⏰ Подтвердите готовность в течение {GetIntProperty(json.RootElement, "countdown")} секунд");
                    Console.WriteLine("💡 Используйте команду '4' для подтверждения готовности");
                    break;
                    
                case "player_ready_changed":
                    var playerId = GetStringProperty(json.RootElement, "playerId");
                    var isReady = GetBoolProperty(json.RootElement, "isReady");
                    var readyCount = GetIntProperty(json.RootElement, "readyCount");
                    var totalPlayers = GetIntProperty(json.RootElement, "totalPlayers");
                    
                    if (playerId == _playerId)
                    {
                        _isReady = isReady;
                        Console.WriteLine($"{(isReady ? "✅" : "❌")} Вы {(isReady ? "подтвердили" : "отменили")} готовность");
                    }
                    else
                    {
                        Console.WriteLine($"🎮 Игрок {playerId} {(isReady ? "✅ готов" : "❌ не готов")}");
                    }
                    Console.WriteLine($"📊 Готовы: {readyCount}/{totalPlayers} игроков");
                    break;
                    
                case "player_ready_set":
                    _isReady = GetBoolProperty(json.RootElement, "isReady");
                    Console.WriteLine($"{(GetBoolProperty(json.RootElement, "isReady") ? "✅" : "❌")} {GetStringProperty(json.RootElement, "message")}");
                    break;
                    
                case "game_started":
                    Console.WriteLine($"🎉 ИГРА НАЧАЛАСЬ!");
                    Console.WriteLine($"📝 {GetStringProperty(json.RootElement, "message")}");
                    Console.WriteLine($"⏰ Время начала: {GetStringProperty(json.RootElement, "startTime")}");
                    Console.WriteLine($"⏳ Длительность: {GetIntProperty(json.RootElement, "duration")} секунд");
                    _isReady = false; // Сбрасываем флаг готовности
                    break;
                    
                case "readiness_timeout":
                    Console.WriteLine($"⏰ ВРЕМЯ ОЖИДАНИЯ ИСТЕКЛО");
                    Console.WriteLine($"📝 {GetStringProperty(json.RootElement, "message")}");
                    Console.WriteLine($"✅ Готово: {GetIntProperty(json.RootElement, "readyCount")}/{GetIntProperty(json.RootElement, "totalPlayers")}");
                    _isReady = false;
                    break;
                    
                case "code_submitted":
                    Console.WriteLine($"📤 Код отправлен на проверку");
                    Console.WriteLine($"📋 Задача: {GetStringProperty(json.RootElement, "problemId")}");
                    break;
                    
                case "code_submitted_by_player":
                    Console.WriteLine($"🎮 Игрок {GetStringProperty(json.RootElement, "playerId")} отправил код");
                    Console.WriteLine($"📋 Задача: {GetStringProperty(json.RootElement, "problemId")}");
                    break;
                    
                case "code_result":
                    Console.WriteLine($"📊 РЕЗУЛЬТАТ ПРОВЕРКИ КОДА:");
                    if (json.RootElement.TryGetProperty("result", out var resultElement))
                    {
                        Console.WriteLine($"   📋 Статус: {GetStringProperty(resultElement, "status")}");
                        Console.WriteLine($"   ✅ Пройдено тестов: {GetIntProperty(resultElement, "passedTests")}/{GetIntProperty(resultElement, "totalTests")}");
                        Console.WriteLine($"   ⏱️ Время выполнения: {GetIntProperty(resultElement, "executionTime")}мс");
                    }
                    break;
                    
                case "left_room":
                    Console.WriteLine($"👋 Вы покинули комнату {GetStringProperty(json.RootElement, "roomId")}");
                    _currentRoomId = Guid.Empty;
                    _isReady = false;
                    break;
                    
                case "player_disconnected":
                    Console.WriteLine($"🔌 Игрок отключился: {GetStringProperty(json.RootElement, "playerId")}");
                    if (json.RootElement.TryGetProperty("participants", out var disconnectedParticipants) && 
                        disconnectedParticipants.ValueKind == JsonValueKind.Number)
                    {
                        Console.WriteLine($"👥 Осталось участников: {disconnectedParticipants.GetInt32()}");
                    }
                    break;
                    
                case "error":
                    Console.WriteLine($"❌ ОШИБКА: {GetStringProperty(json.RootElement, "message")}");
                    break;
                    
                default:
                    Console.WriteLine($"📨 Неизвестный тип сообщения: {jsonMessage}");
                    break;
            }
            Console.WriteLine("---");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка обработки сообщения: {ex.Message}");
            Console.WriteLine($"📨 Полученное сообщение: {jsonMessage}");
        }
    }

    // Вспомогательные методы для безопасного получения свойств
    private static string GetStringProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            return property.ValueKind switch
            {
                JsonValueKind.String => property.GetString(),
                JsonValueKind.Number => property.GetInt32().ToString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => property.ToString()
            };
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

    private static bool GetBoolProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            return property.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.String => bool.TryParse(property.GetString(), out var result) && result,
                _ => false
            };
        }
        return false;
    }

    static async Task CreateRoom()
    {
        Console.Write("Введите название комнаты: ");
        var roomName = Console.ReadLine();

        Console.Write("Введите ID языка (например, 019ac01b-7977-731a-82e6-cfc2ab28e762): ");
        var languageIdInput = Console.ReadLine();
        
        if (!Guid.TryParse(languageIdInput, out var languageId))
        {
            Console.WriteLine("❌ ОШИБКА: Неверный формат GUID для languageId");
            return;
        }

        Console.Write("Введите сложность задачи (Easy/Medium/Hard): ");
        var difficulty = Console.ReadLine();

        if (string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(difficulty))
        {
            Console.WriteLine("❌ ОШИБКА: Название комнаты и сложность не могут быть пустыми");
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
            Console.WriteLine("❌ ОШИБКА: Неверный формат GUID для roomId");
            return;
        }

        var message = new
        {
            type = "JoinRoom",
            roomId = roomId
        };

        await SendMessage(message);
    }

    static async Task GetRoomStatus()
    {
        if (_currentRoomId == Guid.Empty)
        {
            Console.WriteLine("❌ Вы не находитесь в комнате. Присоединитесь к комнате сначала.");
            return;
        }

        // Статус комнаты автоматически присылается сервером при изменениях
        Console.WriteLine("📊 Запрос статуса комнаты...");
    }

    static async Task ToggleReady()
    {
        if (_currentRoomId == Guid.Empty)
        {
            Console.WriteLine("❌ Вы не находитесь в комнате. Присоединитесь к комнате сначала.");
            return;
        }

        var newReadyState = !_isReady;
        
        var message = new
        {
            type = "PlayerReady",
            roomId = _currentRoomId,
            isReady = newReadyState
        };

        await SendMessage(message);
    }

    static async Task SubmitCode()
    {
        if (_currentRoomId == Guid.Empty)
        {
            Console.WriteLine("❌ Вы не находитесь в комнате. Присоединитесь к комнате сначала.");
            return;
        }

        Console.Write("Введите ID задачи: ");
        var problemId = Console.ReadLine();

        Console.Write("Введите ваш код: ");
        var code = Console.ReadLine();

        if (string.IsNullOrEmpty(problemId) || string.IsNullOrEmpty(code))
        {
            Console.WriteLine("❌ ОШИБКА: ID задачи и код не могут быть пустыми");
            return;
        }

        var message = new
        {
            type = "SubmitCode",
            roomId = _currentRoomId,
            problemId = problemId,
            code = code
        };

        await SendMessage(message);
    }

    static async Task LeaveRoom()
    {
        if (_currentRoomId == Guid.Empty)
        {
            Console.WriteLine("❌ Вы не находитесь в комнате.");
            return;
        }

        var message = new
        {
            type = "LeaveRoom",
            roomId = _currentRoomId
        };

        await SendMessage(message);
        _currentRoomId = Guid.Empty;
        _isReady = false;
    }

    static async Task SendMessage(object message)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), 
                WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine($"📤 Отправлено: {json}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка отправки сообщения: {ex.Message}");
        }
    }
}