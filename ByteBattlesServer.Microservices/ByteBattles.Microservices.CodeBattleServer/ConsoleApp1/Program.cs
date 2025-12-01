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
    private static string _jwtToken = ""; // Добавляем поле для токена

    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Code Battle Client ===");
        
        // 1. Получаем JWT токен (или используем тестовый)
        await GetJwtToken();
        
        if (string.IsNullOrEmpty(_jwtToken))
        {
            Console.WriteLine("❌ Не удалось получить JWT токен. Выход.");
            return;
        }
        
        // 2. Подключаемся к WebSocket с токеном
        Console.WriteLine("Подключение к серверу битв...");
        
        // Добавляем токен в query string
        var uri = new Uri($"ws://localhost:50312/api/battle?access_token={_jwtToken}");
        
        // Или добавляем в заголовки (зависит от сервера)
        _webSocket.Options.SetRequestHeader("Authorization", $"Bearer {_jwtToken}");
        
        await _webSocket.ConnectAsync(uri, CancellationToken.None);
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
    static async Task GetJwtToken()
    {
        Console.WriteLine("\n=== Аутентификация ===");
        Console.WriteLine("1 - Ввести существующий JWT токен");
        Console.WriteLine("2 - Использовать тестовый токен");
        Console.WriteLine("3 - Войти через API (email/password)");
        Console.Write("Ваш выбор: ");
        
        var choice = Console.ReadLine();
        
        switch (choice)
        {
            case "1":
                Console.Write("Введите JWT токен: ");
                _jwtToken = Console.ReadLine();
                if (!string.IsNullOrEmpty(_jwtToken))
                {
                    Console.WriteLine("✅ Токен получен");
                }
                break;
                
            case "2":
                // Тестовый токен (замените на реальный)
                _jwtToken = "your-test-jwt-token-here";
                Console.WriteLine($"⚠️ Используется тестовый токен: {_jwtToken.Substring(0, Math.Min(20, _jwtToken.Length))}...");
                break;
            
                
            default:
                Console.WriteLine("⚠️ Используется тестовый токен");
                _jwtToken = "test-jwt-token";
                break;
        }
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
                Console.WriteLine($" Комната создана: {GetStringProperty(json.RootElement, "roomName")}");
                Console.WriteLine($"ID комнаты: {GetStringProperty(json.RootElement, "roomId")}");
                Console.WriteLine($"Сложность: {GetStringProperty(json.RootElement, "difficulty")}");
                Console.WriteLine($"ообщение: {GetStringProperty(json.RootElement, "message")}");
                
                if (Guid.TryParse(GetStringProperty(json.RootElement, "roomId"), out var roomId))
                {
                    _currentRoomId = roomId;
                }
                break;
                
            case "joined_room":
                Console.WriteLine($"Присоединились к комнате: {GetStringProperty(json.RootElement, "roomName")}");
                Console.WriteLine($"ID комнаты: {GetStringProperty(json.RootElement, "roomId")}");
                Console.WriteLine($"Сообщение: {GetStringProperty(json.RootElement, "message")}");
                Console.WriteLine($"Участников: {GetIntProperty(json.RootElement, "participants")}");
                Console.WriteLine($"Статус: {GetStringProperty(json.RootElement, "status")}");
                Console.WriteLine($"Можно начать: {(GetBoolProperty(json.RootElement, "canStart") ? "ДА" : "НЕТ")}");
                
                if (Guid.TryParse(GetStringProperty(json.RootElement, "roomId"), out var joinedRoomId))
                {
                    _currentRoomId = joinedRoomId;
                }
                break;
                
            case "player_joined":
                Console.WriteLine($"Новый игрок: {GetStringProperty(json.RootElement, "playerId")}");
                Console.WriteLine($"Теперь участников: {GetIntProperty(json.RootElement, "participants")}");
                Console.WriteLine($"Статус комнаты: {GetStringProperty(json.RootElement, "roomStatus")}");
                break;
                
            case "player_left":
                Console.WriteLine($"Игрок вышел: {GetStringProperty(json.RootElement, "playerId")}");
                Console.WriteLine($"Осталось участников: {GetIntProperty(json.RootElement, "participants")}");
                break;
                
            case "room_status":
                Console.WriteLine($"СТАТУС КОМНАТЫ:");
                Console.WriteLine($"ID: {GetStringProperty(json.RootElement, "roomId")}");
                Console.WriteLine($"Статус: {GetStringProperty(json.RootElement, "status")}");
                Console.WriteLine($"Участников: {GetIntProperty(json.RootElement, "participantCount")}");
                Console.WriteLine($"Готово: {GetIntProperty(json.RootElement, "readyCount")}");
                Console.WriteLine($"Можно начать: {(GetBoolProperty(json.RootElement, "canStart") ? "ДА" : "НЕТ")}");
                Console.WriteLine($"Активна: {(GetBoolProperty(json.RootElement, "isActive") ? "ДА" : "НЕТ")}");
                break;
                
            case "game_can_start":
                Console.WriteLine($"КОМНАТА ГОТОВА К НАЧАЛУ!");
                Console.WriteLine($"{GetStringProperty(json.RootElement, "message")}");
                Console.WriteLine($"Подтвердите готовность в течение {GetIntProperty(json.RootElement, "countdown")} секунд");
                break;
                
            case "player_ready_changed":
                var playerId = GetStringProperty(json.RootElement, "playerId");
                var isReady = GetBoolProperty(json.RootElement, "isReady");
                var readyCount = GetIntProperty(json.RootElement, "readyCount");
                var totalPlayers = GetIntProperty(json.RootElement, "totalPlayers");
                
                if (playerId == _playerId)
                {
                    _isReady = isReady;
                }
                else
                {
                }
                Console.WriteLine($"Готовы: {readyCount}/{totalPlayers} игроков");
                break;
                
            case "player_ready_set":
                _isReady = GetBoolProperty(json.RootElement, "isReady");
                Console.WriteLine($"{(GetBoolProperty(json.RootElement, "isReady") ? "Готовы" : "Не готовы")} {GetStringProperty(json.RootElement, "message")}");
                break;
                
            case "game_started":
                Console.WriteLine($"ИГРА НАЧАЛАСЬ!");
                Console.WriteLine($"{GetStringProperty(json.RootElement, "message")}");
                Console.WriteLine($"Время начала: {GetStringProperty(json.RootElement, "startTime")}");
                Console.WriteLine($"Длительность: {GetIntProperty(json.RootElement, "duration")} секунд");
                _isReady = false; // Сбрасываем флаг готовности
                break;
                
            case "readiness_timeout":
                Console.WriteLine($"ВРЕМЯ ОЖИДАНИЯ ИСТЕКЛО");
                Console.WriteLine($"{GetStringProperty(json.RootElement, "message")}");
                Console.WriteLine($"Готово: {GetIntProperty(json.RootElement, "readyCount")}/{GetIntProperty(json.RootElement, "totalPlayers")}");
                _isReady = false;
                break;
                
            case "code_submitted":
                Console.WriteLine($"Код отправлен на проверку");
                Console.WriteLine($"Задача: {GetStringProperty(json.RootElement, "taskTitle")}");
                break;
                
            case "code_submitted_by_player":
                Console.WriteLine($"Игрок {GetStringProperty(json.RootElement, "playerId")} отправил код");
                Console.WriteLine($"Задача: {GetStringProperty(json.RootElement, "taskTitle")}");
                break;
                
            case "code_result":
                ProcessCodeResult(json.RootElement);
                break;
                
            case "battle_won":
                ProcessBattleWin(json.RootElement, true);
                break;
                
            case "battle_finished":
                ProcessBattleWin(json.RootElement, false);
                break;
                
            case "left_room":
                Console.WriteLine($"Вы покинули комнату {GetStringProperty(json.RootElement, "roomId")}");
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

// НОВЫЙ МЕТОД: Обработка результатов проверки кода
static void ProcessCodeResult(JsonElement element)
{
    Console.WriteLine($"РЕЗУЛЬТАТ ПРОВЕРКИ КОДА:");
    
    if (element.TryGetProperty("result", out var resultElement))
    {
        var status = GetStringProperty(resultElement, "status");
        var passedTests = GetIntProperty(resultElement, "passedTests");
        var totalTests = GetIntProperty(resultElement, "totalTests");
        var executionTime = GetIntProperty(resultElement, "executionTime");
        
        Console.WriteLine($"Статус: {status}");
        Console.WriteLine($"Пройдено тестов: {passedTests}/{totalTests}");
        Console.WriteLine($"Время выполнения: {executionTime}мс");
        
        // Проверяем, выиграл ли игрок
        if (status == "Passed" && passedTests == totalTests && totalTests > 0)
        {
            Console.WriteLine($"ВЫ ПРОШЛИ ВСЕ ТЕСТЫ!");
            Console.WriteLine($"ПОЗДРАВЛЯЕМ С ПОБЕДОЙ!");
        }
        else if (passedTests == totalTests && totalTests > 0)
        {
            Console.WriteLine($"ВСЕ ТЕСТЫ ПРОЙДЕНЫ!");
        }
        
        // Выводим детальную информацию о тестах
        if (resultElement.TryGetProperty("testResults", out var testResults) && 
            testResults.ValueKind == JsonValueKind.Array)
        {
            Console.WriteLine($"ДЕТАЛЬНЫЕ РЕЗУЛЬТАТЫ:");
            var testNumber = 1;
            foreach (var test in testResults.EnumerateArray())
            {
                var testStatus = GetStringProperty(test, "status");
                var input = GetStringProperty(test, "input");
                var expectedOutput = GetStringProperty(test, "expectedOutput");
                var actualOutput = GetStringProperty(test, "actualOutput");
                var testExecutionTime = GetIntProperty(test, "executionTime");
                
                var statusIcon = testStatus == "Passed" ? "Да" : "Нет";
                Console.WriteLine($"      {testNumber}. {statusIcon} Тест: {input} → {actualOutput} (ожидалось: {expectedOutput}) [{testExecutionTime}мс]");
                testNumber++;
            }
        }
    }
}

// НОВЫЙ МЕТОД: Обработка победы в битве
static void ProcessBattleWin(JsonElement element, bool isWinner)
{
    var winnerId = GetStringProperty(element, "winnerId");
    var taskTitle = GetStringProperty(element, "taskTitle");
    var message = GetStringProperty(element, "message");
    var timestamp = GetStringProperty(element, "timestamp");
    
    if (isWinner)
    {
        Console.WriteLine($" ВЫ ВЫИГРАЛИ БИТВУ!");
        Console.WriteLine($"Задача: {taskTitle}");
        Console.WriteLine($"{message}");
        Console.WriteLine($"Время победы: {timestamp}");
        Console.WriteLine($"Поздравляем с победой!");
        
        // Сбрасываем состояние комнаты
        _currentRoomId = Guid.Empty;
        _isReady = false;
    }
    else
    {
        Console.WriteLine($"БИТВА ЗАВЕРШЕНА");
        Console.WriteLine($"Победитель: {winnerId}");
        Console.WriteLine($"Задача: {taskTitle}");
        Console.WriteLine($"{message}");
        Console.WriteLine($"Время завершения: {timestamp}");
        
        if (winnerId == _playerId)
        {
            Console.WriteLine($"ПОЗДРАВЛЯЕМ С ПОБЕДОЙ!");
        }
        else
        {
            Console.WriteLine($"Поздравляем победителя!");
        }
        
        // Сбрасываем состояние комнаты
        _currentRoomId = Guid.Empty;
        _isReady = false;
    }
    
    Console.WriteLine($"Комната автоматически закрывается...");
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
        var roomName = "mew";

        Console.Write("Введите ID языка (например, 019ac01b-7977-731a-82e6-cfc2ab28e762): ");
        var languageIdInput = "0250f966-d5db-4994-8bd7-0911460ffbe9";
        //var languageIdInput = "76f0b6a1-40fc-42ca-8735-cf8ce6096789";
        
        if (!Guid.TryParse(languageIdInput, out var languageId))
        {
            Console.WriteLine("❌ ОШИБКА: Неверный формат GUID для languageId");
            return;
        }

        Console.Write("Введите сложность задачи (Easy/Medium/Hard): ");
        var difficulty = "Easy";

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
        

        Console.Write("Введите ваш код: ");
        var code = Console.ReadLine();

      
        var message = new
        {
            type = "SubmitCode",
            roomId = _currentRoomId,
           
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
            Console.WriteLine($"Отправлено: {json}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки сообщения: {ex.Message}");
        }
    }
}