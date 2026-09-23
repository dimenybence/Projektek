using System.Net;
using System.Net.Http.Json;
using RealtimeQuizGame.Shared.Models;
using RealtimeQuizGame.Tests.Infrastructure;

namespace RealtimeQuizGame.Tests.IntegrationTests;

public class QuizParticipantControllerIntegrationTests : IDisposable
{
    private readonly QuizWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public QuizParticipantControllerIntegrationTests()
    {
        _factory = new QuizWebApplicationFactory(nameof(QuizParticipantControllerIntegrationTests));
        _client = _factory.CreateClient();
    }

    // Érvényes PIN-nel csatlakozás participation tokent ad.
    [Fact]
    public async Task Join_ValidPin_ReturnsParticipationToken()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var quiz = await CreateQuizAsync();

        var response = await _client.PostAsJsonAsync(
            "/quizzes/participant/join",
            new JoinQuizRequestDto { Pin = quiz.Pin, DisplayName = "Player One" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var join = await response.Content.ReadFromJsonAsync<JoinQuizResponseDto>();
        Assert.NotNull(join);
        Assert.False(string.IsNullOrEmpty(join.ParticipationToken));
        Assert.Equal(quiz.Id, join.QuizId);
    }

    // Ismeretlen PIN 404 Not Found.
    [Fact]
    public async Task Join_InvalidPin_ReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync(
            "/quizzes/participant/join",
            new JoinQuizRequestDto { Pin = "ZZZZZZ", DisplayName = "Player" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // Befejezett kvízhez csatlakozás 409 Conflict.
    [Fact]
    public async Task Join_CompletedQuiz_ReturnsConflict()
    {
        var response = await _client.PostAsJsonAsync(
            "/quizzes/participant/join",
            new JoinQuizRequestDto { Pin = TestDataSeeder.CompletedQuizPin, DisplayName = "Late Player" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // Csatlakozás után a játékállapot Lobby fázist mutat.
    [Fact]
    public async Task GetPlayState_AfterJoin_InLobby()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var quiz = await CreateQuizAsync();
        var (_, token) = await _factory.JoinQuizAsync(_client, quiz.Pin);
        using var participantClient = _factory.CreateParticipantClient(token);

        var state = await participantClient.GetFromJsonAsync<ParticipantPlayStateDto>(
            "/quizzes/participant/play-state");

        Assert.NotNull(state);
        Assert.Equal(QuizPhaseDto.Lobby, state.Phase);
    }

    // Participation token nélkül a play-state 401.
    [Fact]
    public async Task GetPlayState_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/quizzes/participant/play-state");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Nyitott kérdésre beküldött válasz 204 No Content.
    [Fact]
    public async Task SubmitAnswer_WhenQuestionOpen_ReturnsNoContent()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var quiz = await CreateQuizAsync();
        var (_, token) = await _factory.JoinQuizAsync(_client, quiz.Pin);
        using var participantClient = _factory.CreateParticipantClient(token);

        await _client.PostAsync($"/quizzes/{quiz.Id}/start", null);
        await _client.PostAsJsonAsync(
            $"/quizzes/{quiz.Id}/questions/open",
            new OpenQuestionRequestDto { QuestionIndex = 0 });

        var playState = await participantClient.GetFromJsonAsync<ParticipantPlayStateDto>(
            "/quizzes/participant/play-state");
        var optionId = playState!.CurrentQuestion!.Options.First().Id;

        var response = await participantClient.PostAsJsonAsync(
            "/quizzes/participant/answer",
            new SubmitAnswerRequestDto { AnswerOptionId = optionId });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    // Ugyanarra a kérdésre második válasz 409 Conflict.
    [Fact]
    public async Task SubmitAnswer_Twice_ReturnsConflict()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var quiz = await CreateQuizAsync();
        var (_, token) = await _factory.JoinQuizAsync(_client, quiz.Pin);
        using var participantClient = _factory.CreateParticipantClient(token);

        await _client.PostAsync($"/quizzes/{quiz.Id}/start", null);
        await _client.PostAsJsonAsync(
            $"/quizzes/{quiz.Id}/questions/open",
            new OpenQuestionRequestDto { QuestionIndex = 0 });

        var playState = await participantClient.GetFromJsonAsync<ParticipantPlayStateDto>(
            "/quizzes/participant/play-state");
        var optionId = playState!.CurrentQuestion!.Options.First().Id;

        await participantClient.PostAsJsonAsync(
            "/quizzes/participant/answer",
            new SubmitAnswerRequestDto { AnswerOptionId = optionId });

        var response = await participantClient.PostAsJsonAsync(
            "/quizzes/participant/answer",
            new SubmitAnswerRequestDto { AnswerOptionId = optionId });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // Nincs nyitott kérdés — válasz beküldése 409.
    [Fact]
    public async Task SubmitAnswer_WhenNoOpenQuestion_ReturnsConflict()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var quiz = await CreateQuizAsync();
        var (_, token) = await _factory.JoinQuizAsync(_client, quiz.Pin);
        using var participantClient = _factory.CreateParticipantClient(token);

        var response = await participantClient.PostAsJsonAsync(
            "/quizzes/participant/answer",
            new SubmitAnswerRequestDto { AnswerOptionId = 1 });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // A joinable lista nem tartalmazza a befejezett kvízeket.
    [Fact]
    public async Task ListJoinable_ExcludesCompletedQuizzes()
    {
        var response = await _client.GetAsync("/quizzes/participant/joinable?page=1&pageSize=50");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PaginatedResultDto<QuizSummaryResponseDto>>();
        Assert.NotNull(page);
        Assert.DoesNotContain(page.Items, q => q.Pin == TestDataSeeder.CompletedQuizPin);
    }

    private async Task<QuizSummaryResponseDto> CreateQuizAsync()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var response = await _client.PostAsJsonAsync(
            "/quizzes",
            TestDataSeeder.CreateValidQuizRequest());
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<QuizSummaryResponseDto>())!;
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
