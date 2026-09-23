using Microsoft.EntityFrameworkCore;
using RealtimeQuizGame.DataAccess.Exeptions;
using RealtimeQuizGame.DataAccess.Models;
using RealtimeQuizGame.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess.Services
{
    internal class QuizzesService : IQuizzesService
    {
        private const string PinAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        /// <summary>
        /// Ha a részvételi rekord már nem létezik (pl. manuális törlés után).
        /// </summary>
        private const string ParticipationNoLongerValidMessage =
            "A részvételed már nem érvényes. Csatlakozz újra ugyanazzal a PIN-kóddal.";

        private readonly RealTimeQuizGameDbContext _db;
        private readonly IParticipantTokenService _participantTokens;

        public QuizzesService(RealTimeQuizGameDbContext db, IParticipantTokenService participantTokens)
        {
            _db = db;
            _participantTokens = participantTokens;
        }

        public async Task<Quiz> CreateQuizAsync(string ownerId, CreateQuizRequestDto request)
        {
            ValidateQuizContent(request);

            var pin = await GenerateUniquePinAsync();

            var quiz = new Quiz
            {
                Title = request.Title.Trim(),
                Pin = pin,
                OwnerId = ownerId,
                RunStatus = QuizRunStatus.NotInProgress,
                Phase = QuizPhase.Lobby,
                CurrentQuestionIndex = null,
                QuestionStartedAtUtc = null,
                DefaultSecondsPerQuestion = request.DefaultSecondsPerQuestion,
                CreatedAtUtc = DateTime.UtcNow,
            };

            foreach (var q in request.Questions)
            {
                var question = new Question
                {
                    Text = q.Text,
                    TimeLimitSeconds = q.TimeLimitSeconds,
                };

                foreach (var o in q.Options)
                {
                    question.Options.Add(new AnswerOption
                    {
                        Text = o.Text,
                        IsCorrect = o.IsCorrect,
                    });
                }

                quiz.Questions.Add(question);
            }

            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();

            return await _db.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstAsync(q => q.Id == quiz.Id);
        }

        public async Task<Quiz> UpdateQuizAsync(int quizId, string ownerId, CreateQuizRequestDto request)
        {
            ValidateQuizContent(request);

            var quiz = await GetOwnedQuizTrackedAsync(quizId, ownerId);

            if (quiz.RunStatus != QuizRunStatus.NotInProgress ||
                (quiz.Phase != QuizPhase.Lobby && quiz.Phase != QuizPhase.Completed))
            {
                throw new InvalidOperationException(
                    "A kvíz csak indítás előtt vagy befejezett állapotban szerkeszthető; futó kör nem módosítható.");
            }

            quiz.Title = request.Title.Trim();
            quiz.DefaultSecondsPerQuestion = request.DefaultSecondsPerQuestion;

            _db.QuizQuestions.RemoveRange(quiz.Questions);
            quiz.Questions.Clear();

            foreach (var q in request.Questions)
            {
                var question = new Question
                {
                    Text = q.Text,
                    TimeLimitSeconds = q.TimeLimitSeconds,
                };

                foreach (var o in q.Options)
                {
                    question.Options.Add(new AnswerOption
                    {
                        Text = o.Text,
                        IsCorrect = o.IsCorrect,
                    });
                }

                quiz.Questions.Add(question);
            }

            await _db.SaveChangesAsync();

            return await _db.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstAsync(q => q.Id == quiz.Id);
        }

        public Task<PaginatedResult<Quiz>> ListMineAsync(string ownerId, int page, int pageSize) =>
            ListQuizzesPagedAsync(
                _db.Quizzes.AsNoTracking().Where(q => q.OwnerId == ownerId),
                page,
                pageSize);

        public Task<PaginatedResult<Quiz>> ListJoinableAsync(int page, int pageSize) =>
            ListQuizzesPagedAsync(
                _db.Quizzes.AsNoTracking().Where(q => q.Phase != QuizPhase.Completed),
                page,
                pageSize);

        private async Task<PaginatedResult<Quiz>> ListQuizzesPagedAsync(
            IQueryable<Quiz> query,
            int page,
            int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 50);

            query = query.OrderByDescending(q => q.CreatedAtUtc);

            var total = await query.CountAsync();
            var items = await query
                .Include(q => q.Questions)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize);

            return new PaginatedResult<Quiz>(total, pageSize, page, totalPages, items);
        }

        public async Task<Quiz> GetOwnedQuizAsync(int quizId, string ownerId)
        {
            var quiz = await _db.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
            {
                throw new EntityNotFoundException(nameof(Quiz));
            }

            if (quiz.OwnerId != ownerId)
            {
                throw new AccessDeniedException("Nincs jogosultságod ehhez a kvízhez.");
            }

            return quiz;
        }

        public async Task<bool> StartQuizAsync(int quizId, string ownerId)
        {
            var quiz = await GetOwnedQuizTrackedAsync(quizId, ownerId);
            var sessionReset = quiz.Phase == QuizPhase.Completed;

            if (quiz.RunStatus == QuizRunStatus.InProgress)
            {
                throw new InvalidOperationException("A kvíz már fut. Befejezés után indítható új kör.");
            }

            if (quiz.Phase == QuizPhase.Lobby)
            {
                var hasParticipant = await _db.QuizParticipations.AnyAsync(p => p.QuizId == quizId);
                if (!hasParticipant)
                {
                    throw new InvalidOperationException(
                        "Legalább egy résztvevőnek csatlakoznia kell a PIN kóddal, mielőtt elindítható a kvíz.");
                }
            }

            if (quiz.Phase == QuizPhase.Completed)
            {
                var participationIds = await _db.QuizParticipations
                    .Where(p => p.QuizId == quizId)
                    .Select(p => p.Id)
                    .ToListAsync();
                if (participationIds.Count > 0)
                {
                    var answers = await _db.QuizParticipantAnswers
                        .Where(a => participationIds.Contains(a.ParticipationId))
                        .ToListAsync();
                    _db.QuizParticipantAnswers.RemoveRange(answers);
                }
            }

            quiz.RunStatus = QuizRunStatus.InProgress;
            quiz.Phase = QuizPhase.Lobby;
            quiz.CurrentQuestionIndex = null;
            quiz.QuestionStartedAtUtc = null;
            await _db.SaveChangesAsync();
            return sessionReset;
        }

        public async Task OpenQuestionAsync(int quizId, string ownerId, int questionIndex)
        {
            var quiz = await GetOwnedQuizTrackedAsync(quizId, ownerId);

            if (quiz.RunStatus != QuizRunStatus.InProgress)
            {
                throw new InvalidOperationException("A kvíz nincs folyamatban.");
            }

            if (quiz.Phase != QuizPhase.Lobby)
            {
                throw new InvalidOperationException("Új kérdés csak a várakozási (lobby) fázisból nyitható meg.");
            }

            var ordered = OrderedQuestions(quiz);
            if (questionIndex < 0 || questionIndex >= ordered.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(questionIndex), "Érvénytelen kérdésindex.");
            }

            quiz.CurrentQuestionIndex = questionIndex;
            quiz.Phase = QuizPhase.QuestionOpen;
            quiz.QuestionStartedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task CloseQuestionAsync(int quizId, string ownerId)
        {
            var quiz = await GetOwnedQuizTrackedAsync(quizId, ownerId);

            if (quiz.Phase != QuizPhase.QuestionOpen)
            {
                throw new InvalidOperationException("Nincs megnyitott kérdés, amit le lehetne zárni.");
            }

            quiz.Phase = QuizPhase.QuestionResults;
            await _db.SaveChangesAsync();
        }

        public async Task AdvanceAfterResultsAsync(int quizId, string ownerId)
        {
            var quiz = await GetOwnedQuizTrackedAsync(quizId, ownerId);

            if (quiz.Phase != QuizPhase.QuestionResults)
            {
                throw new InvalidOperationException("Csak eredményfázisból léphető a következő kérdésre.");
            }

            var idx = quiz.CurrentQuestionIndex ?? throw new InvalidOperationException("Hiányzó aktuális kérdés.");
            var ordered = OrderedQuestions(quiz);
            var next = idx + 1;

            if (next >= ordered.Count)
            {
                quiz.RunStatus = QuizRunStatus.NotInProgress;
                quiz.Phase = QuizPhase.Completed;
                quiz.CurrentQuestionIndex = null;
                quiz.QuestionStartedAtUtc = null;
            }
            else
            {
                quiz.CurrentQuestionIndex = next;
                quiz.Phase = QuizPhase.QuestionOpen;
                quiz.QuestionStartedAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        public async Task<(QuizParticipation participation, string token)> JoinByPinAsync(
            string pin,
            string displayName,
            string? appUserId)
        {
            var normalizedPin = pin.Trim().ToUpperInvariant();
            var quiz = await _db.Quizzes
                .FirstOrDefaultAsync(q => q.Pin == normalizedPin);

            if (quiz == null)
            {
                throw new EntityNotFoundException(nameof(Quiz));
            }

            if (quiz.Phase == QuizPhase.Completed)
            {
                throw new InvalidOperationException("Ez a kvíz már befejeződött.");
            }

            var participation = new QuizParticipation
            {
                Id = Guid.NewGuid(),
                QuizId = quiz.Id,
                DisplayName = displayName.Trim(),
                AppUserId = appUserId,
                JoinedAtUtc = DateTime.UtcNow,
            };

            _db.QuizParticipations.Add(participation);
            await _db.SaveChangesAsync();

            var token = _participantTokens.CreateToken(participation.Id, quiz.Id);
            return (participation, token);
        }

        public async Task<ParticipantPlayStateDto> GetParticipantPlayStateAsync(Guid participationId)
        {
            var participation = await _db.QuizParticipations
                .Include(p => p.Quiz)
                .ThenInclude(q => q!.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(p => p.Id == participationId);

            if (participation == null)
            {
                throw new InvalidOperationException(ParticipationNoLongerValidMessage);
            }

            var quiz = participation.Quiz;
            var now = DateTime.UtcNow;
            var ordered = OrderedQuestions(quiz);

            if (quiz.RunStatus == QuizRunStatus.NotInProgress && quiz.Phase == QuizPhase.Completed)
            {
                var allQuestionIds = ordered.Select(q => q.Id).ToList();
                var finalLeaderboard = allQuestionIds.Count > 0
                    ? await BuildLeaderboardAsync(quiz.Id, allQuestionIds, participationId)
                    : null;

                return new ParticipantPlayStateDto
                {
                    QuizId = quiz.Id,
                    QuizTitle = quiz.Title,
                    RunStatus = MapRunStatus(quiz.RunStatus),
                    Phase = MapPhase(quiz.Phase),
                    CurrentQuestionIndex = quiz.CurrentQuestionIndex,
                    ServerUtcNow = now,
                    CurrentQuestion = null,
                    ResultBreakdown = null,
                    RemainingSeconds = null,
                    SelectedAnswerOptionId = null,
                    HasAnsweredCurrentQuestion = false,
                    Leaderboard = finalLeaderboard,
                };
            }

            if ((quiz.RunStatus == QuizRunStatus.NotInProgress && quiz.Phase == QuizPhase.Lobby)
                || quiz.CurrentQuestionIndex == null
                || ordered.Count == 0)
            {
                return new ParticipantPlayStateDto
                {
                    QuizId = quiz.Id,
                    QuizTitle = quiz.Title,
                    RunStatus = MapRunStatus(quiz.RunStatus),
                    Phase = MapPhase(quiz.Phase),
                    CurrentQuestionIndex = quiz.CurrentQuestionIndex,
                    ServerUtcNow = now,
                    CurrentQuestion = null,
                    ResultBreakdown = null,
                    RemainingSeconds = null,
                    SelectedAnswerOptionId = null,
                    HasAnsweredCurrentQuestion = false,
                    Leaderboard = null,
                };
            }

            var current = ordered[quiz.CurrentQuestionIndex.Value];
            int? remainingSeconds = null;
            ParticipantQuestionDto? currentQuestion = null;
            IList<QuestionResultOptionDto>? resultBreakdown = null;
            IList<LeaderboardEntryDto>? leaderboard = null;

            if (quiz.Phase == QuizPhase.QuestionOpen)
            {
                var limit = current.TimeLimitSeconds ?? quiz.DefaultSecondsPerQuestion;
                var deadline = quiz.QuestionStartedAtUtc!.Value.AddSeconds(limit);
                remainingSeconds = (int)Math.Max(0, (deadline - now).TotalSeconds);
                currentQuestion = new ParticipantQuestionDto
                {
                    Id = current.Id,
                    Text = current.Text,
                    Options = current.Options
                        .OrderBy(o => o.Id)
                        .Select(o => new ParticipantOptionDto { Id = o.Id, Text = o.Text })
                        .ToList(),
                };
            }
            else if (quiz.Phase == QuizPhase.QuestionResults)
            {
                resultBreakdown = await BuildResultBreakdownAsync(current);
                var questionIdsSoFar = ordered
                    .Take(quiz.CurrentQuestionIndex.Value + 1)
                    .Select(q => q.Id)
                    .ToList();
                leaderboard = await BuildLeaderboardAsync(quiz.Id, questionIdsSoFar, participationId);
            }

            var existing = await _db.QuizParticipantAnswers
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ParticipationId == participationId && a.QuestionId == current.Id);

            return new ParticipantPlayStateDto
            {
                QuizId = quiz.Id,
                QuizTitle = quiz.Title,
                RunStatus = MapRunStatus(quiz.RunStatus),
                Phase = MapPhase(quiz.Phase),
                CurrentQuestionIndex = quiz.CurrentQuestionIndex,
                ServerUtcNow = now,
                CurrentQuestion = currentQuestion,
                ResultBreakdown = resultBreakdown,
                RemainingSeconds = remainingSeconds,
                SelectedAnswerOptionId = existing?.AnswerOptionId,
                HasAnsweredCurrentQuestion = existing != null,
                Leaderboard = leaderboard,
            };
        }

        public async Task SubmitAnswerAsync(Guid participationId, int answerOptionId)
        {
            var participation = await _db.QuizParticipations
                .Include(p => p.Quiz)
                .ThenInclude(q => q!.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(p => p.Id == participationId);

            if (participation == null)
            {
                throw new InvalidOperationException(ParticipationNoLongerValidMessage);
            }

            var quiz = participation.Quiz;

            if (quiz.RunStatus != QuizRunStatus.InProgress || quiz.Phase != QuizPhase.QuestionOpen)
            {
                throw new InvalidOperationException("Jelenleg nem adhatsz le választ erre a kérdésre.");
            }

            if (quiz.CurrentQuestionIndex == null)
            {
                throw new InvalidOperationException("Nincs aktív kérdés.");
            }

            var ordered = OrderedQuestions(quiz);
            var current = ordered[quiz.CurrentQuestionIndex.Value];
            var option = current.Options.FirstOrDefault(o => o.Id == answerOptionId);
            if (option == null)
            {
                throw new ArgumentException("A választás nem tartozik ehhez a kérdéshez.");
            }

            var limit = current.TimeLimitSeconds ?? quiz.DefaultSecondsPerQuestion;
            var deadline = quiz.QuestionStartedAtUtc!.Value.AddSeconds(limit);
            if (DateTime.UtcNow > deadline)
            {
                throw new InvalidOperationException("Az időkeret lejárt — válasz nem rögzíthető.");
            }

            var existing = await _db.QuizParticipantAnswers
                .FirstOrDefaultAsync(a => a.ParticipationId == participationId && a.QuestionId == current.Id);

            if (existing != null)
            {
                throw new InvalidOperationException("Erre a kérdésre már válaszoltál.");
            }

            _db.QuizParticipantAnswers.Add(new QuizParticipantAnswer
            {
                ParticipationId = participationId,
                QuestionId = current.Id,
                AnswerOptionId = answerOptionId,
                SubmittedAtUtc = DateTime.UtcNow,
            });

            await _db.SaveChangesAsync();
        }

        private async Task<IList<QuestionResultOptionDto>> BuildResultBreakdownAsync(Question current)
        {
            var counts = await _db.QuizParticipantAnswers
                .AsNoTracking()
                .Where(a => a.QuestionId == current.Id)
                .GroupBy(a => a.AnswerOptionId)
                .Select(g => new { OptionId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.OptionId, x => x.Count);

            return current.Options
                .OrderBy(o => o.Id)
                .Select(o => new QuestionResultOptionDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect,
                    AnswerCount = counts.TryGetValue(o.Id, out var c) ? c : 0,
                })
                .ToList();
        }

        private async Task<IList<LeaderboardEntryDto>> BuildLeaderboardAsync(
            int quizId,
            IReadOnlyList<int> questionIds,
            Guid currentParticipationId)
        {
            if (questionIds.Count == 0)
            {
                return Array.Empty<LeaderboardEntryDto>();
            }

            var participations = await _db.QuizParticipations
                .AsNoTracking()
                .Where(p => p.QuizId == quizId)
                .Select(p => new { p.Id, p.DisplayName })
                .ToListAsync();

            var correctCounts = await _db.QuizParticipantAnswers
                .AsNoTracking()
                .Where(a => questionIds.Contains(a.QuestionId))
                .Where(a => a.AnswerOption.IsCorrect)
                .Where(a => a.Participation.QuizId == quizId)
                .GroupBy(a => a.ParticipationId)
                .Select(g => new { ParticipationId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ParticipationId, x => x.Count);

            var sorted = participations
                .Select(p => new
                {
                    p.Id,
                    p.DisplayName,
                    CorrectCount = correctCounts.TryGetValue(p.Id, out var c) ? c : 0,
                })
                .OrderByDescending(x => x.CorrectCount)
                .ThenBy(x => x.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var result = new List<LeaderboardEntryDto>(sorted.Count);
            var rank = 0;
            var itemsProcessed = 0;
            int? previousCount = null;

            foreach (var entry in sorted)
            {
                itemsProcessed++;
                if (previousCount != entry.CorrectCount)
                {
                    rank = itemsProcessed;
                    previousCount = entry.CorrectCount;
                }

                result.Add(new LeaderboardEntryDto
                {
                    Rank = rank,
                    DisplayName = entry.DisplayName,
                    CorrectAnswerCount = entry.CorrectCount,
                    IsCurrentParticipant = entry.Id == currentParticipationId,
                });
            }

            return result;
        }

        private async Task<Quiz> GetOwnedQuizTrackedAsync(int quizId, string ownerId)
        {
            var quiz = await _db.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
            {
                throw new EntityNotFoundException(nameof(Quiz));
            }

            if (quiz.OwnerId != ownerId)
            {
                throw new AccessDeniedException("Nincs jogosultságod ehhez a kvízhez.");
            }

            return quiz;
        }

        private static List<Question> OrderedQuestions(Quiz quiz)
        {
            return quiz.Questions.OrderBy(q => q.Id).ToList();
        }

        private static void ValidateQuizContent(CreateQuizRequestDto request)
        {
            if (request.Questions.Count < 1)
            {
                throw new ArgumentException("Legalább egy kérdés szükséges.");
            }

            foreach (var q in request.Questions)
            {
                if (q.Options.Count < 2)
                {
                    throw new ArgumentException("Minden kérdéshez legalább két válaszlehetőség tartozzon.");
                }

                var correct = q.Options.Count(o => o.IsCorrect);
                if (correct != 1)
                {
                    throw new ArgumentException("Minden kérdésnél pontosan egy helyes választ kell megjelölni.");
                }
            }
        }

        private async Task<string> GenerateUniquePinAsync()
        {
            for (var attempt = 0; attempt < 40; attempt++)
            {
                var pin = new string(Enumerable.Range(0, 6).Select(_ => PinAlphabet[Random.Shared.Next(PinAlphabet.Length)]).ToArray());
                var exists = await _db.Quizzes.AnyAsync(q => q.Pin == pin);
                if (!exists)
                {
                    return pin;
                }
            }

            throw new InvalidOperationException("Nem sikerült egyedi PIN kódot generálni.");
        }

        private static QuizRunStatusDto MapRunStatus(QuizRunStatus s) => s switch
        {
            QuizRunStatus.NotInProgress => QuizRunStatusDto.NotInProgress,
            QuizRunStatus.InProgress => QuizRunStatusDto.InProgress,
            _ => throw new ArgumentOutOfRangeException(nameof(s)),
        };

        private static QuizPhaseDto MapPhase(QuizPhase p) => p switch
        {
            QuizPhase.Lobby => QuizPhaseDto.Lobby,
            QuizPhase.QuestionOpen => QuizPhaseDto.QuestionOpen,
            QuizPhase.QuestionResults => QuizPhaseDto.QuestionResults,
            QuizPhase.Completed => QuizPhaseDto.Completed,
            _ => throw new ArgumentOutOfRangeException(nameof(p)),
        };
    }
}
