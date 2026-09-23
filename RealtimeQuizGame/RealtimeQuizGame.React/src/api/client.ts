import type {
  CreateQuizRequestDto,
  JoinQuizResponseDto,
  LoginResponseDto,
  PaginatedResultDto,
  ParticipantPlayStateDto,
  QuizAdminDetailResponseDto,
  QuizSummaryResponseDto,
  UserResponseDto,
} from './types'

const API_BASE = '/api'

const USER_AUTH_KEY = 'rqg_user_auth'
const PARTICIPANT_TOKEN_KEY = 'rqg_participant_token'

export interface StoredUserAuth {
  userId: string
  authToken: string
  refreshToken: string
}

export function loadUserAuth(): StoredUserAuth | null {
  try {
    const raw = localStorage.getItem(USER_AUTH_KEY)
    if (!raw) return null
    return JSON.parse(raw) as StoredUserAuth
  } catch {
    return null
  }
}

export function saveUserAuth(auth: StoredUserAuth | null) {
  if (!auth) localStorage.removeItem(USER_AUTH_KEY)
  else localStorage.setItem(USER_AUTH_KEY, JSON.stringify(auth))
}

export function getParticipantToken(): string | null {
  return sessionStorage.getItem(PARTICIPANT_TOKEN_KEY)
}

export function setParticipantToken(token: string | null) {
  if (!token) sessionStorage.removeItem(PARTICIPANT_TOKEN_KEY)
  else sessionStorage.setItem(PARTICIPANT_TOKEN_KEY, token)
}

async function readApiError(res: Response): Promise<string> {
  try {
    const j = (await res.json()) as { detail?: string; title?: string }
    if (j.detail) return j.detail
    if (j.title) return j.title
  } catch {
    /* ignore */
  }
  return res.statusText || 'Ismeretlen hiba'
}

type AuthMode = 'user' | 'participant' | 'none'

async function request<T>(
  path: string,
  init: RequestInit & { auth?: AuthMode } = {},
): Promise<T> {
  const { auth = 'none', headers: initHeaders, ...rest } = init
  const headers = new Headers(initHeaders)
  if (rest.body != null && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json')
  }

  if (auth === 'user') {
    const u = loadUserAuth()
    if (u?.authToken) headers.set('Authorization', `Bearer ${u.authToken}`)
  } else if (auth === 'participant') {
    const t = getParticipantToken()
    if (t) headers.set('Authorization', `Bearer ${t}`)
  }

  const res = await fetch(`${API_BASE}${path}`, { ...rest, headers })

  if (res.status === 401 && auth === 'user') {
    const refreshed = await tryRefreshToken()
    if (refreshed) {
      headers.set('Authorization', `Bearer ${refreshed.authToken}`)
      const retry = await fetch(`${API_BASE}${path}`, { ...rest, headers })
      if (!retry.ok) throw new Error(await readApiError(retry))
      if (retry.status === 204) return undefined as T
      return (await retry.json()) as T
    }
  }

  if (!res.ok) throw new Error(await readApiError(res))
  if (res.status === 204) return undefined as T
  return (await res.json()) as T
}

async function tryRefreshToken(): Promise<StoredUserAuth | null> {
  const u = loadUserAuth()
  if (!u?.refreshToken) return null
  try {
    const res = await fetch(`${API_BASE}/users/refresh`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(u.refreshToken),
    })
    if (!res.ok) {
      saveUserAuth(null)
      return null
    }
    const body = (await res.json()) as LoginResponseDto
    const next: StoredUserAuth = {
      userId: body.userId,
      authToken: body.authToken,
      refreshToken: body.refreshToken,
    }
    saveUserAuth(next)
    return next
  } catch {
    saveUserAuth(null)
    return null
  }
}

export async function registerUser(body: {
  name: string
  email: string
  password: string
}): Promise<UserResponseDto> {
  return request<UserResponseDto>('/users', { method: 'POST', body: JSON.stringify(body) })
}

export async function loginUser(body: {
  email: string
  password: string
}): Promise<LoginResponseDto> {
  return request<LoginResponseDto>('/users/login', {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export async function logoutUser(): Promise<void> {
  await request<void>('/users/logout', { method: 'POST', auth: 'user' })
}

export async function fetchCurrentUser(): Promise<UserResponseDto> {
  return request<UserResponseDto>('/users/me', { auth: 'user' })
}

export async function listMyQuizzes(
  page = 1,
  pageSize = 10,
): Promise<PaginatedResultDto<QuizSummaryResponseDto>> {
  const params = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  })
  return request<PaginatedResultDto<QuizSummaryResponseDto>>(`/quizzes/mine?${params}`, {
    auth: 'user',
  })
}

export async function listJoinableQuizzes(
  page = 1,
  pageSize = 10,
): Promise<PaginatedResultDto<QuizSummaryResponseDto>> {
  const params = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  })
  return request<PaginatedResultDto<QuizSummaryResponseDto>>(
    `/quizzes/participant/joinable?${params}`,
    { auth: 'none' },
  )
}

export async function createQuiz(body: CreateQuizRequestDto): Promise<QuizSummaryResponseDto> {
  return request<QuizSummaryResponseDto>('/quizzes', {
    method: 'POST',
    body: JSON.stringify(body),
    auth: 'user',
  })
}

export async function updateQuiz(
  id: number,
  body: CreateQuizRequestDto,
): Promise<QuizSummaryResponseDto> {
  return request<QuizSummaryResponseDto>(`/quizzes/${id}`, {
    method: 'PUT',
    body: JSON.stringify(body),
    auth: 'user',
  })
}

export async function getQuizAdmin(id: number): Promise<QuizAdminDetailResponseDto> {
  return request<QuizAdminDetailResponseDto>(`/quizzes/${id}/admin`, { auth: 'user' })
}

export async function startQuiz(id: number): Promise<void> {
  await request<void>(`/quizzes/${id}/start`, { method: 'POST', auth: 'user' })
}

export async function openQuestion(id: number, questionIndex: number): Promise<void> {
  await request<void>(`/quizzes/${id}/questions/open`, {
    method: 'POST',
    body: JSON.stringify({ questionIndex }),
    auth: 'user',
  })
}

export async function closeQuestion(id: number): Promise<void> {
  await request<void>(`/quizzes/${id}/questions/close`, { method: 'POST', auth: 'user' })
}

export async function advanceQuiz(id: number): Promise<void> {
  await request<void>(`/quizzes/${id}/questions/advance`, { method: 'POST', auth: 'user' })
}

export async function joinQuiz(
  pin: string,
  displayName: string,
  linkUserToken?: string,
): Promise<JoinQuizResponseDto> {
  const headers = new Headers({ 'Content-Type': 'application/json' })
  if (linkUserToken) headers.set('Authorization', `Bearer ${linkUserToken}`)
  const res = await fetch(`${API_BASE}/quizzes/participant/join`, {
    method: 'POST',
    headers,
    body: JSON.stringify({ pin, displayName }),
  })
  if (!res.ok) throw new Error(await readApiError(res))
  return (await res.json()) as JoinQuizResponseDto
}

export async function getParticipantPlayState(): Promise<ParticipantPlayStateDto> {
  return request<ParticipantPlayStateDto>('/quizzes/participant/play-state', {
    auth: 'participant',
  })
}

export async function submitParticipantAnswer(answerOptionId: number): Promise<void> {
  await request<void>('/quizzes/participant/answer', {
    method: 'POST',
    body: JSON.stringify({ answerOptionId }),
    auth: 'participant',
  })
}
