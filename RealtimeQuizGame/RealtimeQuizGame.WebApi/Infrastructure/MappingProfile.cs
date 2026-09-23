using RealtimeQuizGame.DataAccess.Models;
using RealtimeQuizGame.Shared.Models;
using AutoMapper;

namespace RealtimeQuizGame.WebApi.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRequestDto, User>(MemberList.Source)
                .ForSourceMember(src => src.Password, opt => opt.DoNotValidate())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<User, UserResponseDto>(MemberList.Destination);

            CreateMap<AnswerOption, QuizAnswerOptionAdminDto>(MemberList.Destination);

            CreateMap<Question, QuizQuestionAdminDto>(MemberList.Destination)
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options.OrderBy(o => o.Id)));

            CreateMap<Quiz, QuizAdminDetailResponseDto>(MemberList.Destination)
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions.OrderBy(q => q.Id)))
                .ForMember(dest => dest.RunStatus, opt => opt.MapFrom(src => MapRunStatus(src.RunStatus)))
                .ForMember(dest => dest.Phase, opt => opt.MapFrom(src => MapPhase(src.Phase)))
                .ForMember(dest => dest.ServerUtcNow, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.RemainingSeconds, opt => opt.MapFrom<AdminRemainingSecondsResolver>());

            CreateMap<Quiz, QuizSummaryResponseDto>(MemberList.Destination)
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions.Count))
                .ForMember(dest => dest.RunStatus, opt => opt.MapFrom(src => MapRunStatus(src.RunStatus)))
                .ForMember(dest => dest.Phase, opt => opt.MapFrom(src => MapPhase(src.Phase)));
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

    public class AdminRemainingSecondsResolver : IValueResolver<Quiz, QuizAdminDetailResponseDto, int?>
    {
        public int? Resolve(Quiz source, QuizAdminDetailResponseDto destination, int? destMember, ResolutionContext context)
        {
            if (source.Phase != QuizPhase.QuestionOpen || source.CurrentQuestionIndex == null || source.QuestionStartedAtUtc == null)
            {
                return null;
            }

            var ordered = source.Questions.OrderBy(q => q.Id).ToList();
            if (ordered.Count == 0 || source.CurrentQuestionIndex.Value < 0 || source.CurrentQuestionIndex.Value >= ordered.Count)
            {
                return null;
            }

            var current = ordered[source.CurrentQuestionIndex.Value];
            var limit = current.TimeLimitSeconds ?? source.DefaultSecondsPerQuestion;
            var deadline = source.QuestionStartedAtUtc.Value.AddSeconds(limit);
            return (int)Math.Max(0, (deadline - DateTime.UtcNow).TotalSeconds);
        }
    }
}
