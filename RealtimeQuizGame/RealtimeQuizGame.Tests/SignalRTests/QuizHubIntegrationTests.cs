using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using RealtimeQuizGame.Shared.Models;
using RealtimeQuizGame.Shared.SignalR.HubInterfaces;
using RealtimeQuizGame.SignalR.Services;
using RealtimeQuizGame.Tests.Infrastructure;

namespace RealtimeQuizGame.Tests.SignalRTests;

public class QuizHubIntegrationTests : IDisposable
{
    private readonly QuizWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public QuizHubIntegrationTests()
    {
        _factory = new QuizWebApplicationFactory(nameof(QuizHubIntegrationTests));
        _client = _factory.CreateClient();
    }

    // Résztvevő tokennel a QuizHub kapcsolat létrejön.
    [Fact]
    public async Task Participant_WithToken_CanConnect()
    {
        var (quizId, token, _) = await CreateQuizWithParticipantAsync();

        await using var hub = _factory.CreateHubConnection(token);
        await hub.StartAsync();

        Assert.Equal(HubConnectionState.Connected, hub.State);
        await hub.InvokeAsync("LeaveQuizGroup", quizId);
    }

    // Token nélkül a hub kapcsolat elutasításra kerül.
    [Fact]
    public async Task Anonymous_CannotConnect()
    {
        await using var hub = _factory.CreateHubConnection();

        await Assert.ThrowsAsync<HttpRequestException>(() => hub.StartAsync());
    }

    // Bejelentkezett tanár user JWT-vel csatlakozhat a hubhoz.
    [Fact]
    public async Task Admin_WithToken_CanConnect()
    {
        var quizId = await CreateQuizAsTeacherAsync();
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);

        var authToken = _client.DefaultRequestHeaders.Authorization!.Parameter;
        await using var hub = _factory.CreateHubConnection(authToken);
        await hub.StartAsync();

        Assert.Equal(HubConnectionState.Connected, hub.State);
        await hub.InvokeAsync("LeaveQuizGroup", quizId);
    }

    // Tanár a saját kvíz csoportjában megkapja a PlayStateUpdated üzenetet.
    [Fact]
    public async Task Admin_JoinQuizGroup_ReceivesPlayStateUpdated_FromService()
    {
        var quizId = await CreateQuizAsTeacherAsync();
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);

        var authToken = _client.DefaultRequestHeaders.Authorization!.Parameter;
        await using var hub = _factory.CreateHubConnection(authToken);
        await hub.StartAsync();

        var received = new List<int>();
        hub.On<int>(nameof(IQuizClient.PlayStateUpdated), id => received.Add(id));

        await hub.InvokeAsync("JoinQuizGroup", quizId);

        var notifications = _factory.Services.GetRequiredService<IQuizNotificationService>();
        await notifications.NotifyPlayStateUpdatedAsync(quizId);

        await Task.Delay(500);

        Assert.Contains(quizId, received);
    }

    // Csoportba lépés után a szolgáltatás PlayStateUpdated eseményt küld.
    [Fact]
    public async Task JoinQuizGroup_ReceivesPlayStateUpdated_FromService()
    {
        var (quizId, token) = await CreateStartedQuizWithParticipantAsync();

        await using var hub = _factory.CreateHubConnection(token);
        await hub.StartAsync();

        var received = new List<int>();
        hub.On<int>(nameof(IQuizClient.PlayStateUpdated), id => received.Add(id));

        await hub.InvokeAsync("JoinQuizGroup", quizId);

        var notifications = _factory.Services.GetRequiredService<IQuizNotificationService>();
        await notifications.NotifyPlayStateUpdatedAsync(quizId);

        await Task.Delay(500);

        Assert.Contains(quizId, received);
    }

    // Kérdés megnyitása HTTP-n keresztül PlayStateUpdated SignalR üzenetet vált ki.
    [Fact]
    public async Task JoinQuizGroup_ReceivesPlayStateUpdated_FromHttp()
    {
        var (quizId, token, pin) = await CreateQuizWithParticipantAsync();

        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        await _client.PostAsync($"/quizzes/{quizId}/start", null);

        await using var hub = _factory.CreateHubConnection(token);
        await hub.StartAsync();

        var received = new List<int>();
        hub.On<int>(nameof(IQuizClient.PlayStateUpdated), id => received.Add(id));
        await hub.InvokeAsync("JoinQuizGroup", quizId);

        await _client.PostAsJsonAsync(
            $"/quizzes/{quizId}/questions/open",
            new OpenQuestionRequestDto { QuestionIndex = 0 });

        await Task.Delay(500);

        Assert.Contains(quizId, received);
        Assert.NotEqual(default, pin);
    }

    // JoinQuizGroup nélkül nem érkezik PlayStateUpdated üzenet.
    [Fact]
    public async Task NotJoined_DoesNotReceivePlayStateUpdated()
    {
        var (quizId, token) = await CreateStartedQuizWithParticipantAsync();

        await using var hub = _factory.CreateHubConnection(token);
        await hub.StartAsync();

        var received = new List<int>();
        hub.On<int>(nameof(IQuizClient.PlayStateUpdated), id => received.Add(id));

        var notifications = _factory.Services.GetRequiredService<IQuizNotificationService>();
        await notifications.NotifyPlayStateUpdatedAsync(quizId);

        await Task.Delay(500);

        Assert.Empty(received);
    }

    // LeaveQuizGroup után nem jön több PlayStateUpdated.
    [Fact]
    public async Task LeaveQuizGroup_StopsReceiving()
    {
        var (quizId, token) = await CreateStartedQuizWithParticipantAsync();

        await using var hub = _factory.CreateHubConnection(token);
        await hub.StartAsync();
        await hub.InvokeAsync("JoinQuizGroup", quizId);
        await hub.InvokeAsync("LeaveQuizGroup", quizId);

        var received = new List<int>();
        hub.On<int>(nameof(IQuizClient.PlayStateUpdated), id => received.Add(id));

        var notifications = _factory.Services.GetRequiredService<IQuizNotificationService>();
        await notifications.NotifyPlayStateUpdatedAsync(quizId);

        await Task.Delay(500);

        Assert.Empty(received);
    }

    // A csoport tagjai megkapják a QuizSessionReset eseményt.
    [Fact]
    public async Task QuizSessionReset_SendsToGroup()
    {
        var (quizId, token) = await CreateStartedQuizWithParticipantAsync();

        await using var hub = _factory.CreateHubConnection(token);
        await hub.StartAsync();
        await hub.InvokeAsync("JoinQuizGroup", quizId);

        var received = new List<int>();
        hub.On<int>(nameof(IQuizClient.QuizSessionReset), id => received.Add(id));

        var notifications = _factory.Services.GetRequiredService<IQuizNotificationService>();
        await notifications.NotifyQuizSessionResetAsync(quizId);

        await Task.Delay(500);

        Assert.Contains(quizId, received);
    }

    private async Task<int> CreateQuizAsTeacherAsync()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var createResponse = await _client.PostAsJsonAsync(
            "/quizzes",
            TestDataSeeder.CreateValidQuizRequest("SignalR quiz"));
        createResponse.EnsureSuccessStatusCode();
        var quiz = (await createResponse.Content.ReadFromJsonAsync<QuizSummaryResponseDto>())!;
        return quiz.Id;
    }

    private async Task<(int QuizId, string Token, string Pin)> CreateQuizWithParticipantAsync()
    {
        var quizId = await CreateQuizAsTeacherAsync();
        var pin = await GetQuizPinAsync(quizId);
        var (_, token) = await _factory.JoinQuizAsync(_client, pin);
        return (quizId, token, pin);
    }

    private async Task<string> GetQuizPinAsync(int quizId)
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        var admin = await _client.GetFromJsonAsync<QuizAdminDetailResponseDto>($"/quizzes/{quizId}/admin");
        return admin!.Pin;
    }

    private async Task<(int QuizId, string Token)> CreateStartedQuizWithParticipantAsync()
    {
        var (quizId, token, _) = await CreateQuizWithParticipantAsync();
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);
        await _client.PostAsync($"/quizzes/{quizId}/start", null);
        return (quizId, token);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
