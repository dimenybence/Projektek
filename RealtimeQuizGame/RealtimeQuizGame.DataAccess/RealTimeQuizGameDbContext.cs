using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealtimeQuizGame.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeQuizGame.DataAccess
{
    public class RealTimeQuizGameDbContext : IdentityDbContext<User, UserRole, string>
    {
        public DbSet<Quiz> Quizzes { get; set; } = null!;
        public DbSet<Question> QuizQuestions { get; set; } = null!;
        public DbSet<AnswerOption> QuizAnswerOptions { get; set; } = null!;
        public DbSet<QuizParticipation> QuizParticipations { get; set; } = null!;
        public DbSet<QuizParticipantAnswer> QuizParticipantAnswers { get; set; } = null!;

        public RealTimeQuizGameDbContext(DbContextOptions<RealTimeQuizGameDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Quiz>(e =>
            {
                e.ToTable("Quizzes");
                e.HasIndex(q => q.Pin).IsUnique();
                e.HasOne(q => q.Owner)
                    .WithMany()
                    .HasForeignKey(q => q.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Question>(e =>
            {
                e.HasOne(q => q.Quiz)
                    .WithMany(qz => qz.Questions)
                    .HasForeignKey(q => q.QuizId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AnswerOption>(e =>
            {
                e.HasOne(o => o.Question)
                    .WithMany(q => q.Options)
                    .HasForeignKey(o => o.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<QuizParticipation>(e =>
            {
                e.HasOne(p => p.Quiz)
                    .WithMany(q => q.Participations)
                    .HasForeignKey(p => p.QuizId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(p => p.AppUser)
                    .WithMany()
                    .HasForeignKey(p => p.AppUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<QuizParticipantAnswer>(e =>
            {
                e.HasIndex(a => new { a.ParticipationId, a.QuestionId }).IsUnique();
                e.HasOne(a => a.Participation)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(a => a.ParticipationId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(a => a.Question)
                    .WithMany()
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.NoAction);
                e.HasOne(a => a.AnswerOption)
                    .WithMany()
                    .HasForeignKey(a => a.AnswerOptionId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
