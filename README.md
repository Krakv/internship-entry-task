# Tic-Tac-Toe REST API

REST API для игры в крестики-нолики NxN.

## Технологии
- **Платформа**: .NET 9
- **База данных**: PostgreSQL (основная), Redis (кэширование)
- **Библиотеки**:
  - Entity Framework Core (ORM)
  - Swashbuckle.AspNetCore (Swagger)
  - StackExchange.Redis (Redis клиент)
  - Npgsql (PostgreSQL провайдер)

## Архитектура
### Структура проекта

1. Controllers - только HTTP-запросы/ответы
2. Services - правила игры, проверка ходов, определение победителя
3. Repositories - работа с БД: CRUD операций, EF Core
4. Models - сущности БД: Game, Move...
5. DTOs - передача данных

### Описание 
- Слоистая архитектура (Controllers, Services, Repositories)
- Repository для работы с БД
- Dependency Injection для управления зависимостями

## Endpoints

### 1. Создание новой игры
`POST /api/games`

**Ответ (201 Created):**
```json
{
  "Id": "1",
  "BoardSize": 3,
  "Status": "InProgress"
}
```

### 2. Получение состояния игры
`GET /api/games/{id}`

**Ответ (200 OK):**
```json
{
  "BoardState": "X-O-XO--X",
  "WinnerLineLength": 3,
  "CurrentPlayer": "O",
  "Status": "InProgress"
}
```

### 3. Совершение хода
`POST /api/games/{id}/moves`

**Запрос:**
```json
{
  "Player": "X",
  "Row": 1,
  "Col": 1
}
```

**Ответ (200 OK):**
```json
{
  "NewBoardState": "X-OXXO--X",
  "Status": "XWon"
}
```

## Коды ошибок
- `400 Bad Request` - Некорректный ход или некорректный JSON
- `404 Not Found` - Игра не найдена

# Запуск проекта
1. Запустите через Docker `docker-compose up -d --build`.
2. API будет доступно на http://localhost:8080.

# Тестирование
- Unit-тесты: Проверка игровой логики (победа/ничья/случайные ходы).
- Интеграционные тесты: Проверка API endpoints.

Запуск тестов: `dotnet test`

# Особенности
1. Concurrency & идемпотентность: ходы кэшируются в Redis.
2. Crash-safe: все изменения сохраняются в PostgreSQL перед ответом.
3. Случайные ходы: реализованы через Random.Next(0, 100) < 10 на каждом 3-м ходе.

# Документация API

Swagger UI доступен по адресу: `http://localhost:8080/swagger`
