using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TicTacToe.API.DTOs;
using TicTacToe.API.Models;
using TicTacToe.API.Services;

namespace TicTacToe.API.Controllers
{
    /// <summary>
    /// Контроллер для управления ходами в игре крестики-нолики
    /// </summary>
    [Route("api/games/{gameId}/[controller]")]
    [ApiController]
    public class MovesController : ControllerBase
    {
        private readonly IMoveService _moveService;
        private readonly ILogger<MovesController> _logger;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();

        /// <summary>
        /// Конструктор контроллера ходов
        /// </summary>
        /// <param name="moveService">Сервис для работы с ходами</param>
        /// <param name="logger">Логгер для записи событий</param>
        public MovesController(IMoveService moveService, ILogger<MovesController> logger)
        {
            _moveService = moveService;
            _logger = logger;
        }

        /// <summary>
        /// Совершает ход в указанной игре
        /// </summary>
        /// <param name="gameId">Идентификатор игры</param>
        /// <param name="moveDto">Данные хода (позиция и игрок)</param>
        /// <returns>
        /// Возвращает статус 200 (OK) с результатом хода, если ход успешно выполнен.
        /// Возвращает статус 404 (Not Found), если игра не найдена.
        /// Возвращает статус 409 (Conflict), если игра уже завершена.
        /// Возвращает статус 400 (Bad Request) при некорректных данных хода.
        /// Возвращает статус 500 (Internal Server Error) при внутренней ошибке сервера.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> MakeMove(int gameId, [FromBody] MoveDto moveDto)
        {
            var cacheKey = $"move_{gameId}_{moveDto.Col}_{moveDto.Row}_{moveDto.Player}";
            var semaphore = GetSemaphore(cacheKey);

            if (!await semaphore.WaitAsync(TimeSpan.FromSeconds(5)))
                return StatusCode(503, "Server busy");

            try
            {
                _logger.LogTrace("Начало обработки хода игры {GameId}", gameId);
                var existingResult = await _moveService.GetCachedMoveResultAsync(gameId, moveDto);
                if (existingResult != null)
                {
                    Response.Headers["ETag"] = $"\"{existingResult.ETag}\"";

                    return Ok(existingResult.Response);
                }

                var game = await _moveService.MakeMoveAsync(gameId, moveDto);

                if (game == null)
                {
                    _logger.LogWarning("Игра с ID:{GameId} не найдена.", gameId);
                    return NotFound("Game not found");
                }

                if (game.Status != Models.GameStatus.InProgress)
                {
                    _logger.LogWarning("Игра с ID:{GameId} уже завершена.", gameId);
                    return Conflict("Game has been finished");
                }

                var etag = GenerateETag(game, moveDto);

                var result = new CreatedMoveDto
                {
                    NewBoardState = game.BoardState,
                    Status = game.Status
                };

                await _moveService.CacheMoveResultAsync(gameId, moveDto, result, etag);

                Response.Headers.ETag = $"\"{etag}\"";
                return Ok(result);
            }
            catch (JsonException)
            {
                _logger.LogWarning("Некорректный JSON введен для игры {GameId}", gameId);
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid JSON",
                    Status = 400,
                    Detail = "The JSON body is malformed or invalid"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Игра с ID:{GameId} не найдена. {Exception}", gameId, ex);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Некорректный ход для игры с ID:{GameId}. {Exception}", gameId, ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Непредвиденная ошибка: {Exception}", ex);
                return StatusCode(500, ex.Message);
            }
            finally
            {
                ReleaseAndCleanup(cacheKey, semaphore);
            }
        }

        /// <summary>
        /// Генерирует ETag для текущего состояния игры
        /// </summary>
        /// <param name="game">Объект игры</param>
        /// <returns>Хеш-строка, представляющая текущее состояние игры</returns>
        protected virtual string GenerateETag(Game game, MoveDto moveDto)
        {
            var dataToHash = new
            {
                Board = game.BoardState,
                Move = moveDto
            };

            var json = JsonSerializer.Serialize(dataToHash);
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(hash);
        }

        private SemaphoreSlim GetSemaphore(string key)
        {
            return _semaphores.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        }

        private void ReleaseAndCleanup(string key, SemaphoreSlim semaphore)
        {
            try
            {
                semaphore.Release();
            }
            finally
            {
                if (semaphore.CurrentCount == 1)
                {
                    _semaphores.TryRemove(key, out _);
                }
            }
        }
    }
}