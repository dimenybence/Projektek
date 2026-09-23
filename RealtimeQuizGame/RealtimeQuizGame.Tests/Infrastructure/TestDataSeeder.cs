using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealtimeQuizGame.DataAccess;
using RealtimeQuizGame.DataAccess.Models;
using RealtimeQuizGame.Shared.Models;

namespace RealtimeQuizGame.Tests.Infrastructure;

public static class TestDataSeeder
{
    public const string TeacherEmail = "teacher@test.quiz";
    public const string TeacherPassword = "Test123!";
    public const string OtherEmail = "other@test.quiz";
    public const string OtherPassword = "Test123!";
    public const string CompletedQuizPin = "DONE01";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RealTimeQuizGameDbContext>();
        await db.Database.EnsureCreatedAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new UserRole("User"));
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        await EnsureUserAsync(userManager, TeacherEmail, "Test Teacher", TeacherPassword);
        await EnsureUserAsync(userManager, OtherEmail, "Other User", OtherPassword);

        if (!await db.Quizzes.AnyAsync(q => q.Pin == CompletedQuizPin))
        {
            var teacher = await userManager.FindByEmailAsync(TeacherEmail);
            var completedQuiz = new Quiz
            {
                Title = "Completed seed quiz",
                Pin = CompletedQuizPin,
                OwnerId = teacher!.Id,
                RunStatus = QuizRunStatus.NotInProgress,
                Phase = QuizPhase.Completed,
                DefaultSecondsPerQuestion = 30,
                CreatedAtUtc = DateTime.UtcNow.AddHours(-1),
            };
            completedQuiz.Questions.Add(new Question
            {
                Text = "Seed question",
                Options =
                {
                    new AnswerOption { Text = "Wrong", IsCorrect = false },
                    new AnswerOption { Text = "Right", IsCorrect = true },
                },
            });
            db.Quizzes.Add(completedQuiz);
            await db.SaveChangesAsync();
        }
    }

    public static CreateQuizRequestDto CreateValidQuizRequest(string title = "Integration test quiz") =>
        new()
        {
            Title = title,
            DefaultSecondsPerQuestion = 30,
            Questions =
            [
                new CreateQuestionDto
                {
                    Text = "Question 1?",
                    Options =
                    [
                        new CreateAnswerOptionDto { Text = "Wrong A", IsCorrect = false },
                        new CreateAnswerOptionDto { Text = "Correct A", IsCorrect = true },
                        new CreateAnswerOptionDto { Text = "Wrong B", IsCorrect = false },
                    ],
                },
                new CreateQuestionDto
                {
                    Text = "Question 2?",
                    Options =
                    [
                        new CreateAnswerOptionDto { Text = "Wrong C", IsCorrect = false },
                        new CreateAnswerOptionDto { Text = "Correct C", IsCorrect = true },
                    ],
                },
            ],
        };

    private static async Task EnsureUserAsync(
        UserManager<User> userManager,
        string email,
        string name,
        string password)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            return;
        }

        var user = new User
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Name = name,
            RefreshToken = Guid.NewGuid(),
        };

        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "User");
    }
}
