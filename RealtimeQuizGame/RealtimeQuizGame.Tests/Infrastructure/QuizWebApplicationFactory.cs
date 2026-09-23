using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealtimeQuizGame.DataAccess;
using RealtimeQuizGame.Shared.Models;
using RealtimeQuizGame.WebApi;

namespace RealtimeQuizGame.Tests.Infrastructure;

internal class QuizWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    private readonly string _databaseName;
    private bool _disposed;

    public QuizWebApplicationFactory(string databaseName = "TestQuizDatabase")
    {
        _databaseName = databaseName;
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
    }

    public HttpClient CreateAuthenticatedClient() => CreateClient();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<RealTimeQuizGameDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<RealTimeQuizGameDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            var sp = services.BuildServiceProvider();
            TestDataSeeder.SeedAsync(sp).GetAwaiter().GetResult();
        });
    }

    public HubConnection CreateHubConnection(string? participantToken = null)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost/QuizHub", options =>
            {
                options.HttpMessageHandlerFactory = _ => Server.CreateHandler();
                if (participantToken != null)
                {
                    options.AccessTokenProvider = () => Task.FromResult(participantToken)!;
                }
            })
            .AddJsonProtocol(protocol =>
            {
                protocol.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .Build();

        return connection;
    }

    public async Task<string> LoginAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync(
            "/users/login",
            new LoginRequestDto { Email = email, Password = password });
        response.EnsureSuccessStatusCode();

        var login = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        if (string.IsNullOrEmpty(login?.AuthToken))
        {
            throw new InvalidOperationException("Login failed — missing auth token.");
        }

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login.AuthToken);

        return login.AuthToken;
    }

    public async Task<(JoinQuizResponseDto Join, string ParticipantToken)> JoinQuizAsync(
        HttpClient client,
        string pin,
        string displayName = "Test Player")
    {
        var response = await client.PostAsJsonAsync(
            "/quizzes/participant/join",
            new JoinQuizRequestDto { Pin = pin, DisplayName = displayName });
        response.EnsureSuccessStatusCode();

        var join = await response.Content.ReadFromJsonAsync<JoinQuizResponseDto>();
        if (join == null || string.IsNullOrEmpty(join.ParticipationToken))
        {
            throw new InvalidOperationException("Join failed — missing participation token.");
        }

        return (join, join.ParticipationToken);
    }

    public HttpClient CreateParticipantClient(string participationToken)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", participationToken);
        return client;
    }

    public new void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RealTimeQuizGameDbContext>();
        db.Database.EnsureDeleted();

        base.Dispose();
    }
}
