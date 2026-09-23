using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RealtimeQuizGame.DataAccess;
using RealtimeQuizGame.DataAccess.Config;
using RealtimeQuizGame.DataAccess.Services;
using RealtimeQuizGame.Shared.Models;

namespace RealtimeQuizGame.Tests.UnitTests;

public class QuizzesServiceUnitTests : IDisposable
{
    private readonly RealTimeQuizGameDbContext _context;
    private readonly QuizzesService _service;

    public QuizzesServiceUnitTests()
    {
        var options = new DbContextOptionsBuilder<RealTimeQuizGameDbContext>()
            .UseInMemoryDatabase($"UnitTest_{Guid.NewGuid():N}")
            .Options;

        _context = new RealTimeQuizGameDbContext(options);
        _context.Database.EnsureCreated();

        var jwtSettings = Options.Create(new JwtSettings
        {
            SecretKey = "UnitTestSecretKeyForParticipantTokens123456",
            Audience = "elte.hu",
            Issuer = "elte.hu",
            AccessTokenExpirationMinutes = 20,
            ParticipantAudience = "quiz-participant.elte.hu",
            ParticipantTokenExpirationMinutes = 120,
        });

        _service = new QuizzesService(_context, new ParticipantTokenService(jwtSettings));

        _context.Users.Add(new DataAccess.Models.User
        {
            Id = "owner-1",
            UserName = "owner@test.quiz",
            Email = "owner@test.quiz",
            EmailConfirmed = true,
            Name = "Owner",
            RefreshToken = Guid.NewGuid(),
        });
        _context.SaveChanges();
    }

    // Kérdés nélküli kvíz létrehozása ArgumentException-t dob.
    [Fact]
    public async Task CreateQuiz_NoQuestions_ThrowsArgumentException()
    {
        var request = new CreateQuizRequestDto
        {
            Title = "Invalid",
            DefaultSecondsPerQuestion = 30,
            Questions = [],
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateQuizAsync("owner-1", request));
    }

    // Helyes válasz nélküli kérdés ArgumentException-t dob.
    [Fact]
    public async Task CreateQuiz_NoCorrectOption_ThrowsArgumentException()
    {
        var request = new CreateQuizRequestDto
        {
            Title = "Invalid",
            DefaultSecondsPerQuestion = 30,
            Questions =
            [
                new CreateQuestionDto
                {
                    Text = "Q?",
                    Options =
                    [
                        new CreateAnswerOptionDto { Text = "A", IsCorrect = false },
                        new CreateAnswerOptionDto { Text = "B", IsCorrect = false },
                    ],
                },
            ],
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateQuizAsync("owner-1", request));
    }

    // Több helyes válasz esetén ArgumentException a validációban.
    [Fact]
    public async Task CreateQuiz_MultipleCorrect_ThrowsArgumentException()
    {
        var request = new CreateQuizRequestDto
        {
            Title = "Invalid",
            DefaultSecondsPerQuestion = 30,
            Questions =
            [
                new CreateQuestionDto
                {
                    Text = "Q?",
                    Options =
                    [
                        new CreateAnswerOptionDto { Text = "A", IsCorrect = true },
                        new CreateAnswerOptionDto { Text = "B", IsCorrect = true },
                    ],
                },
            ],
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateQuizAsync("owner-1", request));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
