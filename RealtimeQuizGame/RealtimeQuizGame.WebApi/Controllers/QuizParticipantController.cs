using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeQuizGame.DataAccess.Config;
using RealtimeQuizGame.DataAccess.Services;
using RealtimeQuizGame.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RealtimeQuizGame.WebApi.Controllers
{
    [ApiController]
    [Route("quizzes/participant")]
    public class QuizParticipantController : ControllerBase
    {
        public const string ParticipantJwtScheme = AuthSchemes.ParticipantJwt;

        private readonly IMapper _mapper;
        private readonly IQuizzesService _quizzesService;

        public QuizParticipantController(IMapper mapper, IQuizzesService quizzesService)
        {
            _mapper = mapper;
            _quizzesService = quizzesService;
        }

        /// <summary>
        /// Csatlakozható kvízek lapozott listája (befejezett körök nélkül).
        /// </summary>
        [HttpGet("joinable")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResultDto<QuizSummaryResponseDto>))]
        public async Task<IActionResult> ListJoinable([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _quizzesService.ListJoinableAsync(page, pageSize);
            return Ok(new PaginatedResultDto<QuizSummaryResponseDto>(
                result.Total,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                _mapper.Map<IReadOnlyList<QuizSummaryResponseDto>>(result.Items)));
        }

        /// <summary>
        /// Csatlakozás PIN kóddal (bejelentkezés nélkül is). Bejelentkezett felhasználónál a fiók opcionálisan összekapcsolódik.
        /// </summary>
        [HttpPost("join")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JoinQuizResponseDto))]
        public async Task<IActionResult> Join([FromBody] JoinQuizRequestDto request)
        {
            string? appUserId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                appUserId = User.FindFirst("id")?.Value;
            }

            var (participation, token) = await _quizzesService.JoinByPinAsync(request.Pin, request.DisplayName, appUserId);
            var quiz = await _quizzesService.GetParticipantPlayStateAsync(participation.Id);
            return Ok(new JoinQuizResponseDto
            {
                ParticipationToken = token,
                QuizId = quiz.QuizId,
                QuizTitle = quiz.QuizTitle,
            });
        }

        /// <summary>
        /// Aktuális játékállapot (kérdés / eredmény / várakozás) — SignalR esemény után, vagy kezdeti betöltéskor hívandó.
        /// </summary>
        [HttpGet("play-state")]
        [Authorize(AuthenticationSchemes = ParticipantJwtScheme)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ParticipantPlayStateDto))]
        public async Task<IActionResult> GetPlayState()
        {
            var participationId = GetParticipationId();
            if (participationId == null)
            {
                return Unauthorized();
            }

            var state = await _quizzesService.GetParticipantPlayStateAsync(participationId.Value);
            return Ok(state);
        }

        /// <summary>
        /// Válasz beküldése az aktuális, nyitott kérdésre (egyszer, az időkorláton belül).
        /// </summary>
        [HttpPost("answer")]
        [Authorize(AuthenticationSchemes = ParticipantJwtScheme)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerRequestDto request)
        {
            var participationId = GetParticipationId();
            if (participationId == null)
            {
                return Unauthorized();
            }

            await _quizzesService.SubmitAnswerAsync(participationId.Value, request.AnswerOptionId);
            return NoContent();
        }

        private Guid? GetParticipationId()
        {
            var raw = User.FindFirst(AuthClaimTypes.ParticipationId)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }
}
