using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using RealtimeQuizGame.Shared.Models;
using RealtimeQuizGame.Tests.Infrastructure;

namespace RealtimeQuizGame.Tests.IntegrationTests;

public class QuizzesControllerIntegrationTests : IDisposable
{
    private readonly QuizWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public QuizzesControllerIntegrationTests()
    {
        _factory = new QuizWebApplicationFactory(nameof(QuizzesControllerIntegrationTests));
        _client = _factory.CreateClient();
    }

    // Bejelentkezés nélkül a saját kvízek listázása 401.
    [Fact]
    public async Task ListMine_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/quizzes/mine");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Érvényes kvíz létrehozása 201 és PIN + kérdésszám ellenőrzése.
    [Fact]
    public async Task CreateQuiz_Valid_ReturnsCreated()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);

        var response = await _client.PostAsJsonAsync(
            "/quizzes",
            TestDataSeeder.CreateValidQuizRequest());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var quiz = await response.Content.ReadFromJsonAsync<QuizSummaryResponseDto>();
        Assert.NotNull(quiz);
        Assert.False(string.IsNullOrWhiteSpace(quiz.Pin));
        Assert.Equal(2, quiz.QuestionCount);
    }

    // A tulajdonos lekérheti a kvíz admin állapotát és kérdéseit.
    [Fact]
    public async Task GetAdminState_OwnQuiz_ReturnsQuestions()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync();

        var response = await _client.GetAsync($"/quizzes/{created.Id}/admin");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var detail = await response.Content.ReadFromJsonAsync<QuizAdminDetailResponseDto>();
        Assert.NotNull(detail);
        Assert.Equal(2, detail.Questions.Count);
    }

    // Más felhasználó kvíze 403 Forbidden a tulajdonos nélkül.
    [Fact]
    public async Task GetAdminState_OtherUsersQuiz_ReturnsForbidden()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync();

        await _factory.LoginAsync(_client, TestDataSeeder.OtherEmail, TestDataSeeder.OtherPassword);
        var response = await _client.GetAsync($"/quizzes/{created.Id}/admin");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // Nem futó kvíz tartalma szerkeszthető (200 OK).
    [Fact]
    public async Task UpdateQuiz_NotStarted_ReturnsOk()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync();

        var update = TestDataSeeder.CreateValidQuizRequest("Updated title");
        var response = await _client.PutAsJsonAsync($"/quizzes/{created.Id}", update);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<QuizSummaryResponseDto>();
        Assert.NotNull(updated);
        Assert.Equal("Updated title", updated.Title);
    }

    // Futó kvíz szerkesztése 409 Conflict.
    [Fact]
    public async Task UpdateQuiz_InProgress_ReturnsConflict()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync();
        await _factory.JoinQuizAsync(_client, created.Pin);
        await _client.PostAsync($"/quizzes/{created.Id}/start", null);

        var response = await _client.PutAsJsonAsync(
            $"/quizzes/{created.Id}",
            TestDataSeeder.CreateValidQuizRequest("Should fail"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // Befejezett kvíz szerkesztése 200 OK (indítás előtt és befejezés után szerkeszthető).
    [Fact]
    public async Task UpdateQuiz_Completed_ReturnsOk()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync();
        await _factory.JoinQuizAsync(_client, created.Pin);
        await _client.PostAsync($"/quizzes/{created.Id}/start", null);
        await _client.PostAsJsonAsync(
            $"/quizzes/{created.Id}/questions/open",
            new OpenQuestionRequestDto { QuestionIndex = 0 });
        await _client.PostAsync($"/quizzes/{created.Id}/questions/close", null);
        await _client.PostAsync($"/quizzes/{created.Id}/questions/advance", null);
        await _client.PostAsync($"/quizzes/{created.Id}/questions/close", null);
        await _client.PostAsync($"/quizzes/{created.Id}/questions/advance", null);

        var update = TestDataSeeder.CreateValidQuizRequest("Updated after complete");
        var response = await _client.PutAsJsonAsync($"/quizzes/{created.Id}", update);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<QuizSummaryResponseDto>();
        Assert.NotNull(updated);
        Assert.Equal("Updated after complete", updated.Title);
    }

    // A /quizzes/mine lapozott eredményt ad (Total, CurrentPage).
    [Fact]
    public async Task ListMine_ReturnsPagedResult()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        await CreateQuizAsync("Paged quiz A");
        await CreateQuizAsync("Paged quiz B");

        var response = await _client.GetAsync("/quizzes/mine?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PaginatedResultDto<QuizSummaryResponseDto>>();
        Assert.NotNull(page);
        Assert.True(page.Total >= 2);
        Assert.Equal(1, page.CurrentPage);
    }

    // Résztvevő után a kvíz indítható lobbyból (InProgress állapot).
    [Fact]
    public async Task StartQuiz_FromLobby_ReturnsNoContent()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync();
        await _factory.JoinQuizAsync(_client, created.Pin);

        var response = await _client.PostAsync($"/quizzes/{created.Id}/start", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var admin = await GetAdminStateAsync(created.Id);
        Assert.Equal(QuizRunStatusDto.InProgress, admin.RunStatus);
    }

    // Lobby fázisból kérdés megnyitása QuestionOpen fázisba visz.
    [Fact]
    public async Task OpenQuestion_FromLobby_ReturnsNoContent()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync();
        await _factory.JoinQuizAsync(_client, created.Pin);
        await _client.PostAsync($"/quizzes/{created.Id}/start", null);

        var response = await _client.PostAsJsonAsync(
            $"/quizzes/{created.Id}/questions/open",
            new OpenQuestionRequestDto { QuestionIndex = 0 });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var admin = await GetAdminStateAsync(created.Id);
        Assert.Equal(QuizPhaseDto.QuestionOpen, admin.Phase);
        Assert.Equal(0, admin.CurrentQuestionIndex);
    }

    // Nyitott kérdés mellett újabb megnyitás 409 Conflict.
    [Fact]
    public async Task OpenQuestion_WhenNotInLobby_ReturnsConflict()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync();
        await _factory.JoinQuizAsync(_client, created.Pin);
        await _client.PostAsync($"/quizzes/{created.Id}/start", null);
        await _client.PostAsJsonAsync(
            $"/quizzes/{created.Id}/questions/open",
            new OpenQuestionRequestDto { QuestionIndex = 0 });

        var response = await _client.PostAsJsonAsync(
            $"/quizzes/{created.Id}/questions/open",
            new OpenQuestionRequestDto { QuestionIndex = 1 });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // Megnyitott kérdés lezárása QuestionResults fázisba lép.
    [Fact]
    public async Task CloseQuestion_WhenOpen_ReturnsNoContent()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync();
        await _factory.JoinQuizAsync(_client, created.Pin);
        await _client.PostAsync($"/quizzes/{created.Id}/start", null);
        await _client.PostAsJsonAsync(
            $"/quizzes/{created.Id}/questions/open",
            new OpenQuestionRequestDto { QuestionIndex = 0 });

        var response = await _client.PostAsync($"/quizzes/{created.Id}/questions/close", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var admin = await GetAdminStateAsync(created.Id);
        Assert.Equal(QuizPhaseDto.QuestionResults, admin.Phase);
    }

    // Eredményfázis után advance a következő kérdésre ugrik.
    [Fact]
    public async Task Advance_AfterResults_MovesToNextQuestion()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync();
        await _factory.JoinQuizAsync(_client, created.Pin);
        await _client.PostAsync($"/quizzes/{created.Id}/start", null);
        await _client.PostAsJsonAsync(
            $"/quizzes/{created.Id}/questions/open",
            new OpenQuestionRequestDto { QuestionIndex = 0 });
        await _client.PostAsync($"/quizzes/{created.Id}/questions/close", null);

        var response = await _client.PostAsync($"/quizzes/{created.Id}/questions/advance", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var admin = await GetAdminStateAsync(created.Id);
        Assert.Equal(QuizPhaseDto.QuestionOpen, admin.Phase);
        Assert.Equal(1, admin.CurrentQuestionIndex);
    }

    // Utolsó kérdés után advance Completed fázisra zárja a kvízt.
    [Fact]
    public async Task Advance_AfterLastQuestion_CompletesQuiz()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync();
        await _factory.JoinQuizAsync(_client, created.Pin);
        await _client.PostAsync($"/quizzes/{created.Id}/start", null);

        await _client.PostAsJsonAsync(
            $"/quizzes/{created.Id}/questions/open",
            new OpenQuestionRequestDto { QuestionIndex = 0 });
        await _client.PostAsync($"/quizzes/{created.Id}/questions/close", null);
        await _client.PostAsync($"/quizzes/{created.Id}/questions/advance", null);

        await _client.PostAsync($"/quizzes/{created.Id}/questions/close", null);
        var response = await _client.PostAsync($"/quizzes/{created.Id}/questions/advance", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var admin = await GetAdminStateAsync(created.Id);
        Assert.Equal(QuizPhaseDto.Completed, admin.Phase);
        Assert.Equal(QuizRunStatusDto.NotInProgress, admin.RunStatus);
    }

    // Teljes játékmenet: join, válasz, lezárás, ranglista a végén.
    [Fact]
    public async Task FullQuizFlow_CompletesSuccessfully()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var created = await CreateQuizAsync("Full flow quiz");
        var (_, participantToken) = await _factory.JoinQuizAsync(_client, created.Pin);
        using var participantClient = _factory.CreateParticipantClient(participantToken);

        await _client.PostAsync($"/quizzes/{created.Id}/start", null);
        await _client.PostAsJsonAsync(
            $"/quizzes/{created.Id}/questions/open",
            new OpenQuestionRequestDto { QuestionIndex = 0 });

        var playState = await participantClient.GetFromJsonAsync<ParticipantPlayStateDto>(
            "/quizzes/participant/play-state");
        Assert.NotNull(playState);
        Assert.NotNull(playState.CurrentQuestion);

        var correctOptionId = playState.CurrentQuestion.Options
            .First(o => o.Text == "Correct A")
            .Id;

        var answerResponse = await participantClient.PostAsJsonAsync(
            "/quizzes/participant/answer",
            new SubmitAnswerRequestDto { AnswerOptionId = correctOptionId });
        Assert.Equal(HttpStatusCode.NoContent, answerResponse.StatusCode);

        await _client.PostAsync($"/quizzes/{created.Id}/questions/close", null);
        await _client.PostAsync($"/quizzes/{created.Id}/questions/advance", null);
        await _client.PostAsync($"/quizzes/{created.Id}/questions/close", null);
        await _client.PostAsync($"/quizzes/{created.Id}/questions/advance", null);

        var finalState = await participantClient.GetFromJsonAsync<ParticipantPlayStateDto>(
            "/quizzes/participant/play-state");
        Assert.NotNull(finalState);
        Assert.Equal(QuizPhaseDto.Completed, finalState.Phase);
        Assert.NotNull(finalState.Leaderboard);
    }

    private async Task<QuizSummaryResponseDto> CreateQuizAsync(string? title = null)
    {
        var response = await _client.PostAsJsonAsync(
            "/quizzes",
            TestDataSeeder.CreateValidQuizRequest(title ?? "Test quiz"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<QuizSummaryResponseDto>())!;
    }

    private async Task<QuizAdminDetailResponseDto> GetAdminStateAsync(int quizId)
    {
        var response = await _client.GetAsync($"/quizzes/{quizId}/admin");
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<QuizAdminDetailResponseDto>())!;
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
