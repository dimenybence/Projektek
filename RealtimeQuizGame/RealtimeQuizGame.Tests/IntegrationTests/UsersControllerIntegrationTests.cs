using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using RealtimeQuizGame.Shared.Models;
using RealtimeQuizGame.Tests.Infrastructure;

namespace RealtimeQuizGame.Tests.IntegrationTests;

public class UsersControllerIntegrationTests : IDisposable
{
    private readonly QuizWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UsersControllerIntegrationTests()
    {
        _factory = new QuizWebApplicationFactory(nameof(UsersControllerIntegrationTests));
        _client = _factory.CreateClient();
    }

    // Érvényes adatokkal a regisztráció 201 Created választ ad vissza.
    [Fact]
    public async Task Register_ValidUser_ReturnsCreated()
    {
        var request = new UserRequestDto
        {
            Name = "New User",
            Email = $"newuser-{Guid.NewGuid():N}@test.quiz",
            Password = "Test123!",
        };

        var response = await _client.PostAsJsonAsync("/users", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        Assert.NotNull(user);
        Assert.Equal(request.Email, user.Email);
    }

    // Helyes e-mail és jelszó esetén a bejelentkezés JWT tokent ad vissza.
    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync(
            "/users/login",
            new LoginRequestDto
            {
                Email = TestDataSeeder.TeacherEmail,
                Password = TestDataSeeder.TeacherPassword,
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var login = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        Assert.NotNull(login);
        Assert.False(string.IsNullOrEmpty(login.AuthToken));
    }

    // Hibás jelszó esetén 403 Forbidden a válasz.
    [Fact]
    public async Task Login_InvalidPassword_ReturnsForbidden()
    {
        var response = await _client.PostAsJsonAsync(
            "/users/login",
            new LoginRequestDto
            {
                Email = TestDataSeeder.TeacherEmail,
                Password = "WrongPassword1!",
            });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
    }

    // Token nélkül a /users/me végpont 401 Unauthorized.
    [Fact]
    public async Task GetMe_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Bejelentkezés után a /users/me visszaadja a bejelentkezett felhasználót.
    [Fact]
    public async Task GetMe_AfterLogin_ReturnsUser()
    {
        await _factory.LoginAsync(_client, TestDataSeeder.TeacherEmail, TestDataSeeder.TeacherPassword);

        var response = await _client.GetAsync("/users/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
        Assert.NotNull(user);
        Assert.Equal(TestDataSeeder.TeacherEmail, user.Email);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
