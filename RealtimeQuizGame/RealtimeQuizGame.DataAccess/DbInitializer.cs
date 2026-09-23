using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealtimeQuizGame.DataAccess.Models;

namespace RealtimeQuizGame.DataAccess
{
    public static class DbInitializer
    {
        private static readonly (string Title, string Pin, (string Text, string Correct, string Wrong1, string? Wrong2)[] Questions)[]
            TestQuizzes =
            [
                (
                    "Matematika — alapok",
                    "QZ0001",
                    [
                        ("Mennyi a 12 × 8?", "96", "84", "108"),
                        ("Egy derékszögű háromszög befogói 3 cm és 4 cm. Mekkora az átfogó?", "5 cm", "7 cm", "6 cm"),
                    ]),
                (
                    "Irodalom — magyar klasszikusok",
                    "QZ0002",
                    [
                        ("Ki írta a „Szeptember végén” című verset?", "Kosztolányi Dezső", "Petőfi Sándor", "Ady Endre"),
                        ("Melyik mű Petőfi Sándor nemzeti dalának szövegét adta?", "Nemzeti dal", "A puszta fia", "János vitéz"),
                    ]),
                (
                    "Történelem — Magyarország",
                    "QZ0003",
                    [
                        ("Melyik évben volt a magyarországi forradalom és szabadságharc kezdete?", "1848", "1526", "1956"),
                        ("Melyik csatában veszített súlyosan a középkori Magyar Királyság?", "Mohácsnál", "Nándorfehérvárnál", "Pákozdnál"),
                    ]),
                (
                    "Biológia — sejt és öröklődés",
                    "QZ0004",
                    [
                        ("Melyik sejtorganellum a sejt energiatermelésének fő helye?", "Mitokondrium", "Riboszóma", "Sejtmag"),
                        ("Melyik nukleinsav hordozza az öröklési információt a legtöbb élőlényben?", "DNS", "RNS", "ATP"),
                    ]),
                (
                    "Fizika — mechanika és energia",
                    "QZ0005",
                    [
                        ("Melyik fizikai nagyság mértékegysége a newton?", "Erő", "Energia", "Teljesítmény"),
                        ("Üres térben kb. mekkora a fény sebessége?", "300 000 km/s", "340 m/s", "1500 m/s"),
                    ]),
                (
                    "Kémia — alapfogalmak",
                    "QZ0006",
                    [
                        ("Mi a víz kémiai képlete?", "H₂O", "CO₂", "NaCl"),
                        ("Melyik elem jelölése az arany a periódusos rendszerben?", "Au", "Ag", "Ar"),
                    ]),
                (
                    "Informatika — hálózat és adat",
                    "QZ0007",
                    [
                        ("Melyik protokollot használja leginkább a böngésző a weboldalak letöltésére?", "HTTP/HTTPS", "FTP", "SMTP"),
                        ("Hány bitből áll egy byte?", "8", "4", "16"),
                    ]),
                (
                    "Zene — elmélet és történet",
                    "QZ0008",
                    [
                        ("Hány hangjegyet tartalmaz egy oktáv a szokásos nyugati rendszerben?", "8", "7", "12"),
                        ("Ki komponálta a IX. szimfóniát („Örömóda”)?", "Beethoven", "Mozart", "Bach"),
                    ]),
                (
                    "Sport — általános tudnivalók",
                    "QZ0009",
                    [
                        ("Labdarúgásban hány játékos van egy csapatban a pályán?", "11", "9", "10"),
                        ("Melyik sportágban van a Wimbledon-torna?", "Tenisz", "Golf", "Krikett"),
                    ]),
                (
                    "Filmművészet — alapok",
                    "QZ0010",
                    [
                        ("Melyik díjat adják a legjobb amerikai filmet elismerő évente?", "Oscar-díj", "César-díj", "Palme d'Or"),
                        ("Ki rendezte a „Pulp Fiction” című filmet?", "Quentin Tarantino", "Steven Spielberg", "Christopher Nolan"),
                    ]),
                (
                    "Filozófia — görög és modern",
                    "QZ0011",
                    [
                        ("Ki volt az athéni akadémia alapítója az ókori Görögországban?", "Platón", "Arisztotelész", "Szókratész"),
                        ("Ki mondta: „Gondolkodom, tehát vagyok”?", "Descartes", "Kant", "Hume"),
                    ]),
                (
                    "Gazdaságtan — alapfogalmak",
                    "QZ0012",
                    [
                        ("Mit jelent a GDP rövidítés?", "Gazdasági teljesítmény mutatója", "Árfolyam", "Kamat"),
                        ("Mi történik általában magas infláció mellett a pénz vásárlóerejével?", "Csökken", "Nő", "Változatlan"),
                    ]),
                (
                    "Nyelvtan — magyar nyelv",
                    "QZ0013",
                    [
                        ("Melyik szó ige a mondatban: „A diák olvas egy könyvet.”?", "olvas", "könyv", "diák"),
                        ("Hány magánhangzó van a magyar ábécében?", "14", "10", "26"),
                    ]),
                (
                    "Művészettörténet — reneszánsz",
                    "QZ0014",
                    [
                        ("Ki festette a „Mona Lisát”?", "Leonardo da Vinci", "Michelangelo", "Rafael"),
                        ("Melyik városban található a Sixtusi kápolna mennyezetfreskója (Ádám teremtése)?", "Róma", "Firenze", "Velence"),
                    ]),
                (
                    "Astronómia — Naprendszer",
                    "QZ0015",
                    [
                        ("Melyik bolyó a legközelebb a Naphoz?", "Merkúr", "Vénusz", "Mars"),
                        ("Mi a Naprendszerünk legnagyobb bolygója?", "Jupiter", "Szaturnusz", "Neptunusz"),
                    ]),
                (
                    "Logika — következtetés",
                    "QZ0016",
                    [
                        ("Ha minden A egy B, és minden B egy C, akkor minden A mit következik?", "C", "B", "Sem A, sem C"),
                        ("Melyik állítás ellentmondásos?", "Az ajtó nyitva van és az ajtó zárva van.", "Ma esik az eső.", "A négyzetnek négy oldala van."),
                    ]),
                (
                    "Etika — alapelvek",
                    "QZ0017",
                    [
                        ("Mit nevezünk aranyszabálynak az etikában?", "Úgy bánj másokkal, ahogy velük bánni szeretnéd", "Mindig mondd az igazat", "A cél szentesíti az eszközt"),
                        ("Ki fogalmazta meg a kategorikus imperatívus elméletét?", "Kant", "Mill", "Arisztotelész"),
                    ]),
                (
                    "Környezettan — klíma",
                    "QZ0018",
                    [
                        ("Melyik gáz járul hozzá leginkább az üvegházhatáshoz?", "Szén-dioxid", "Oxigén", "Nitrogén"),
                        ("Mi védi a Földet az ultraibolya sugárzás egy részétől?", "Ózonréteg", "Nitrogén", "Esztratoszféra"),
                    ]),
                (
                    "Statisztika — alapok",
                    "QZ0019",
                    [
                        ("Mi a medián egy adathalmazban?", "A középső érték rendezés után", "Az összeg osztva n-nel", "A leggyakoribb érték"),
                        ("Egy dobókockával mi a valószínűsége, hogy hatost dobunk?", "1/6", "1/3", "1/2"),
                    ]),
                (
                    "Programozás — alapok",
                    "QZ0020",
                    [
                        ("Melyik kulcsszóval deklarálunk egy változót C# nyelven?", "var vagy típusnév", "define", "let"),
                        ("Mit jelent az OOP rövidítés?", "Objektumorientált programozás", "Operációs optimalizált folyamat", "Open output protocol"),
                    ]),
            ];

        public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RealTimeQuizGameDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();

            await context.Database.MigrateAsync(cancellationToken);

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new UserRole("User"));
            }

            if (await context.Users.AnyAsync(cancellationToken))
            {
                return;
            }

            var teacher = new User
            {
                UserName = "tanar@quiz.elte.hu",
                Email = "tanar@quiz.elte.hu",
                EmailConfirmed = true,
                Name = "Minta Tanár",
                RefreshToken = Guid.NewGuid(),
            };

            await userManager.CreateAsync(teacher, "Quiz123!");
            await userManager.AddToRoleAsync(teacher, "User");

            var student = new User
            {
                UserName = "diak@quiz.elte.hu",
                Email = "diak@quiz.elte.hu",
                EmailConfirmed = true,
                Name = "Minta Diák",
                RefreshToken = Guid.NewGuid(),
            };

            await userManager.CreateAsync(student, "Quiz123!");
            await userManager.AddToRoleAsync(student, "User");

            var baseTime = DateTime.UtcNow;

            for (var i = 0; i < TestQuizzes.Length; i++)
            {
                var (title, pin, questions) = TestQuizzes[i];
                context.Quizzes.Add(CreateSampleQuiz(
                    teacher.Id,
                    title,
                    pin,
                    QuizRunStatus.NotInProgress,
                    QuizPhase.Lobby,
                    30,
                    baseTime,
                    questions));
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        private static Quiz CreateSampleQuiz(
            string ownerId,
            string title,
            string pin,
            QuizRunStatus runStatus,
            QuizPhase phase,
            int defaultSeconds,
            DateTime createdAtUtc,
            params (string Text, string Correct, string Wrong1, string? Wrong2)[] questions)
        {
            var quiz = new Quiz
            {
                Title = title,
                Pin = pin,
                OwnerId = ownerId,
                RunStatus = runStatus,
                Phase = phase,
                DefaultSecondsPerQuestion = defaultSeconds,
                CreatedAtUtc = createdAtUtc,
            };

            foreach (var (text, correct, wrong1, wrong2) in questions)
            {
                var question = new Question { Text = text };
                question.Options.Add(new AnswerOption { Text = wrong1, IsCorrect = false });
                question.Options.Add(new AnswerOption { Text = correct, IsCorrect = true });
                if (wrong2 != null)
                {
                    question.Options.Add(new AnswerOption { Text = wrong2, IsCorrect = false });
                }

                quiz.Questions.Add(question);
            }

            return quiz;
        }
    }
}
