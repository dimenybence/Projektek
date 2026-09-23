using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeQuizGame.DataAccess.Models;
using RealtimeQuizGame.DataAccess.Services;
using RealtimeQuizGame.Shared.Models;
using RealtimeQuizGame.SignalR.Services;

namespace RealtimeQuizGame.WebApi.Controllers
{
    [ApiController]
    [Route("quizzes")]
    [Authorize]
    public class QuizzesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IQuizzesService _quizzesService;
        private readonly IQuizNotificationService _quizNotifications;

        public QuizzesController(
            IMapper mapper,
            IQuizzesService quizzesService,
            IQuizNotificationService quizNotifications)
        {
            _mapper = mapper;
            _quizzesService = quizzesService;
            _quizNotifications = quizNotifications;
        }

        private PaginatedResultDto<QuizSummaryResponseDto> MapPagedQuizzes(
            DataAccess.Models.PaginatedResult<Quiz> result)
        {
            return new PaginatedResultDto<QuizSummaryResponseDto>(
                result.Total,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                _mapper.Map<IReadOnlyList<QuizSummaryResponseDto>>(result.Items));
        }

        /// <summary>
        /// A bejelentkezett felhasználó által létrehozott kvízek lapozott listája.
        /// </summary>
        [HttpGet("mine")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResultDto<QuizSummaryResponseDto>))]
        public async Task<IActionResult> ListMine([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _quizzesService.ListMineAsync(userId, page, pageSize);
            return Ok(MapPagedQuizzes(result));
        }

        /// <summary>
        /// Új kvíz létrehozása (a kvíz indításáig szerkeszthető, ha még nem indult el).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(QuizSummaryResponseDto))]
        public async Task<IActionResult> Create([FromBody] CreateQuizRequestDto request)
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var quiz = await _quizzesService.CreateQuizAsync(userId, request);
            var summary = _mapper.Map<QuizSummaryResponseDto>(quiz);
            return CreatedAtAction(nameof(GetAdminState), new { id = quiz.Id }, summary);
        }

        /// <summary>
        /// Kvíz tartalmának szerkesztése (indítás előtt vagy befejezett kör után; futó kör alatt tiltva).
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QuizSummaryResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CreateQuizRequestDto request)
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var quiz = await _quizzesService.UpdateQuizAsync(id, userId, request);
            return Ok(_mapper.Map<QuizSummaryResponseDto>(quiz));
        }

        /// <summary>
        /// Kvíz részletes állapota és kérdések (csak a tulajdonosnak).
        /// </summary>
        [HttpGet("{id:int}/admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QuizAdminDetailResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdminState([FromRoute] int id)
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var quiz = await _quizzesService.GetOwnedQuizAsync(id, userId);
            return Ok(_mapper.Map<QuizAdminDetailResponseDto>(quiz));
        }

        /// <summary>
        /// Kvíz indítása első alkalommal, vagy befejezett kvíz új köre: lobby; új körben a korábbi válaszok törlődnek (a PIN és részvételek megmaradnak).
        /// </summary>
        [HttpPost("{id:int}/start")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Start([FromRoute] int id)
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var sessionReset = await _quizzesService.StartQuizAsync(id, userId);
            if (sessionReset)
            {
                await _quizNotifications.NotifyQuizSessionResetAsync(id);
            }

            await _quizNotifications.NotifyPlayStateUpdatedAsync(id);

            return NoContent();
        }

        /// <summary>
        /// Aktuális kérdés megnyitása időkorláttal (csak lobby fázisból).
        /// </summary>
        [HttpPost("{id:int}/questions/open")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> OpenQuestion([FromRoute] int id, [FromBody] OpenQuestionRequestDto body)
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _quizzesService.OpenQuestionAsync(id, userId, body.QuestionIndex);
            await _quizNotifications.NotifyPlayStateUpdatedAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Aktuális kérdés lezárása — eredmények megjelenítése a résztvevőknek.
        /// </summary>
        [HttpPost("{id:int}/questions/close")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CloseQuestion([FromRoute] int id)
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _quizzesService.CloseQuestionAsync(id, userId);
            await _quizNotifications.NotifyPlayStateUpdatedAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Következő kérdésre lépés az eredményfázis után, vagy kvíz befejezése.
        /// </summary>
        [HttpPost("{id:int}/questions/advance")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Advance([FromRoute] int id)
        {
            var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _quizzesService.AdvanceAfterResultsAsync(id, userId);
            await _quizNotifications.NotifyPlayStateUpdatedAsync(id);
            return NoContent();
        }
    }
}
