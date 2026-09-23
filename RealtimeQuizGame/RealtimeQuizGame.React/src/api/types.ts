export const QuizRunStatus = {
  NotInProgress: 0,
  InProgress: 1,
} as const

export type QuizRunStatusDto = (typeof QuizRunStatus)[keyof typeof QuizRunStatus]

export const QuizPhase = {
  Lobby: 0,
  QuestionOpen: 1,
  QuestionResults: 2,
  Completed: 3,
} as const

export type QuizPhaseDto = (typeof QuizPhase)[keyof typeof QuizPhase]

export function isQuizEditable(
  runStatus: QuizRunStatusDto,
  phase: QuizPhaseDto,
): boolean {
  return (
    runStatus === QuizRunStatus.NotInProgress &&
    (phase === QuizPhase.Lobby || phase === QuizPhase.Completed)
  )
}

export function isQuizFinished(runStatus: QuizRunStatusDto, phase: QuizPhaseDto): boolean {
  return runStatus === QuizRunStatus.NotInProgress && phase === QuizPhase.Completed
}

export function isQuizNotStarted(runStatus: QuizRunStatusDto, phase: QuizPhaseDto): boolean {
  return runStatus === QuizRunStatus.NotInProgress && phase === QuizPhase.Lobby
}

export function quizDisplayStatus(runStatus: QuizRunStatusDto, phase: QuizPhaseDto): string {
  if (runStatus === QuizRunStatus.InProgress) return 'Folyamatban'
  if (phase === QuizPhase.Completed) return 'Befejezve'
  return 'Nem indult'
}

export interface PaginatedResultDto<T> {
  total: number
  pageSize: number
  currentPage: number
  totalPages: number
  items: T[]
}

export interface QuizSummaryResponseDto {
  id: number
  title: string
  pin: string
  runStatus: QuizRunStatusDto
  phase: QuizPhaseDto
  currentQuestionIndex: number | null
  questionCount: number
  createdAtUtc: string
}

export interface QuizAnswerOptionAdminDto {
  id: number
  text: string
  isCorrect: boolean
}

export interface QuizQuestionAdminDto {
  id: number
  text: string
  timeLimitSeconds: number | null
  options: QuizAnswerOptionAdminDto[]
}

export interface QuizAdminDetailResponseDto {
  id: number
  title: string
  pin: string
  runStatus: QuizRunStatusDto
  phase: QuizPhaseDto
  currentQuestionIndex: number | null
  defaultSecondsPerQuestion: number
  questionStartedAtUtc: string | null
  remainingSeconds: number | null
  serverUtcNow: string
  questions: QuizQuestionAdminDto[]
}

export interface ParticipantOptionDto {
  id: number
  text: string
}

export interface ParticipantQuestionDto {
  id: number
  text: string
  options: ParticipantOptionDto[]
}

export interface QuestionResultOptionDto {
  id: number
  text: string
  answerCount: number
  isCorrect: boolean
}

export interface LeaderboardEntryDto {
  rank: number
  displayName: string
  correctAnswerCount: number
  isCurrentParticipant: boolean
}

export interface ParticipantPlayStateDto {
  quizId: number
  quizTitle: string
  runStatus: QuizRunStatusDto
  phase: QuizPhaseDto
  currentQuestionIndex: number | null
  remainingSeconds: number | null
  serverUtcNow: string
  currentQuestion: ParticipantQuestionDto | null
  resultBreakdown: QuestionResultOptionDto[] | null
  selectedAnswerOptionId: number | null
  hasAnsweredCurrentQuestion: boolean
  leaderboard: LeaderboardEntryDto[] | null
}

export interface LoginResponseDto {
  userId: string
  authToken: string
  refreshToken: string
}

export interface UserResponseDto {
  id: string
  name: string
  email: string
}

export interface JoinQuizResponseDto {
  participationToken: string
  quizId: number
  quizTitle: string
}

export interface CreateAnswerOptionDto {
  text: string
  isCorrect: boolean
}

export interface CreateQuestionDto {
  text: string
  timeLimitSeconds?: number | null
  options: CreateAnswerOptionDto[]
}

export interface CreateQuizRequestDto {
  title: string
  defaultSecondsPerQuestion: number
  questions: CreateQuestionDto[]
}
